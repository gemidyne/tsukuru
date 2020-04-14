using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tsukuru.Core.SourceEngine.Bsp;
using Tsukuru.Core.SourceEngine.Bsp.LumpData.GameLumps;
using Tsukuru.Core.SourceEngine.Mdl;

namespace Tsukuru.Core.SourceEngine
{
    public class BspDependencyAnalyser
    {
        private readonly IProgress<string> _progress;
        private readonly List<string> _customMdls = new List<string>();
        private readonly List<string> _customMaterials = new List<string>();
        private readonly List<string> _customSounds = new List<string>();
        private readonly SearchPathExplorer _pathExplorer;

        public static BspAnalysisResults Analyse(IProgress<string> progress, string vProject, string mapFileName)
        {
            return new BspDependencyAnalyser(progress, vProject).DoAnalysis(mapFileName);
        }

        private BspDependencyAnalyser(IProgress<string> progress, string vProject)
        {
            _progress = progress;
            _pathExplorer = new SearchPathExplorer(vProject);
        }

        internal BspAnalysisResults DoAnalysis(string mapBspFilePath)
        {
            Map map;

            using (var fileStream = new FileStream(mapBspFilePath, FileMode.Open))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    _progress.Report("Loading map BSP... depending on BSP file size, this may take some time.");

                    map = Map.Load(binaryReader);

                    _progress.Report("Map BSP loaded.");
                }
            }

            _progress.Report("Inspecting static props...");
            InspectStaticProps(map);

            _progress.Report("Inspecting entity data...");
            InspectEntityProps(map);

            _progress.Report("Inspecting brush textures...");
            InspectBrushTextures(map);

            _progress.Report("Inspecting model dependencies...");
            foreach (string customMdl in _customMdls.ToList())
            {
                InspectModelTextures(customMdl);
                InspectAdditionalModelFiles(customMdl);
            }

            return new BspAnalysisResults(_customMdls.Select(m => m.Replace("\\", "/")).ToArray(), _customMaterials.Select(m => m.Replace("\\", "/")).ToArray(), _customSounds.Select(m => m.Replace("\\", "/")).ToArray());
        }

        private void InspectEntityProps(Map map)
        {
            var entitiesString = map.Lumps.GetEntitiesString();
            var count = entitiesString.All.Length;

            var entities = new List<Dictionary<string, string>>();

            for (int i = 0; i < count; i++)
            {
                string part = entitiesString[i];

                var blockStart = part.IndexOf('{');
                var blockEnd = part.IndexOf('}');

                if (blockStart == -1 || blockEnd == -1)
                {
                    continue;
                }

                var block = part.Substring(blockStart, blockEnd - blockStart).Trim();
                var lines = block.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                var properties = new Dictionary<string, string>();

                foreach (var line in lines)
                {
                    var trimmedLine = line.Replace("$", "").Replace("%", "").Replace("\"", "").Replace("'", "").Trim();
                    var keyEnd = trimmedLine.IndexOf(' ');
                    if (keyEnd == -1)
                    {
                        keyEnd = trimmedLine.IndexOf('\t');

                        if (keyEnd == -1)
                        {
                            continue;
                        }
                    }
                    var key = trimmedLine.Substring(0, keyEnd).ToLower();
                    var value = trimmedLine.Substring(keyEnd + 1).Replace("\t", "");
                    properties[key] = value;
                }

                entities.Add(properties);
            }


            foreach (var entity in entities)
            {
                switch (entity["classname"])
                {
                    case "ambient_generic":
                        {
                            string sndFile = $"sound/{entity["message"]}";

                            string fileSystemPath = _pathExplorer.GetFileSystemPath(sndFile);

                            if (fileSystemPath == null)
                            {
                                continue;
                            }

                            if (_customSounds.Contains(sndFile))
                            {
                                continue;
                            }

                            _customSounds.Add(sndFile);

                            break;
                        }

                    case "prop_dynamic":
                        {
                            string modelPath = entity["model"];

                            if (modelPath.StartsWith("*"))
                            {
                                continue;
                            }

                            string fileSystemPath = _pathExplorer.GetFileSystemPath(modelPath);

                            if (fileSystemPath == null)
                            {
                                continue;
                            }

                            _customMdls.Add(modelPath);
                            break;
                        }

                    case "worldspawn":
                        {
                            string skyname = entity["skyname"];

                            string[] fileNames = SkyboxMaterialHelper.GetFilePaths(skyname).ToArray();

                            foreach (string fileName in fileNames)
                            {
                                string vmtFile = _pathExplorer.GetFileSystemPath(fileName);

                                if (vmtFile == null)
                                {
                                    continue;
                                }

                                _customMaterials.Add(fileName);

                                InspectVmtFile(vmtFile, _pathExplorer);
                            }

                            break;
                        }
                }
            }
        }

        private void InspectBrushTextures(Map map)
        {
            var textures = map.Lumps.GetTextureDataString();
            var textureIdx = map.Lumps.GetTextureDataStringTable();

            foreach (int i in textureIdx)
            {
                var texture = textures[i];

                string formatted = $"materials/{texture}.vmt";

                string vmtFile = _pathExplorer.GetFileSystemPath(formatted);

                if (vmtFile == null)
                {
                    continue;
                }

                _customMaterials.Add(formatted);

                InspectVmtFile(vmtFile, _pathExplorer);
            }
        }

        private void InspectStaticProps(Map map)
        {
            var models = map.Lumps.GetGame().GetStaticProps().Models;

            foreach (var modelPath in models)
            {
                string fileSystemPath = _pathExplorer.GetFileSystemPath(modelPath);

                if (fileSystemPath == null)
                {
                    continue;
                }

                _customMdls.Add(modelPath);
            }
        }

        private void InspectModelTextures(string modelFilePath)
        {
            string fileSystemPath = _pathExplorer.GetFileSystemPath(modelFilePath);

            if (fileSystemPath == null)
            {
                return;
            }

            using (var mdlStream = new FileStream(fileSystemPath, FileMode.Open))
            using (var mdlReader = new BinaryReader(mdlStream))
            {
                var mdl = new SourceModel(mdlReader);

                for (var i = 0; i < mdl.MaterialPaths.Length; i++)
                {
                    string path = mdl.MaterialPaths[i];
                    string filename = mdl.Materials[i];

                    string assetPath = $"materials/{path}{filename}.vmt";

                    string customMaterialPath = _pathExplorer.GetFileSystemPath(assetPath);

                    if (customMaterialPath == null)
                    {
                        continue;
                    }

                    _customMaterials.Add(assetPath);

                    InspectVmtFile(customMaterialPath, _pathExplorer);
                }
            }
        }

        private void InspectAdditionalModelFiles(string modelFilePath)
        {
            string fileSystemPath = _pathExplorer.GetFileSystemPath(modelFilePath);

            if (fileSystemPath == null)
            {
                return;
            }

            var extensions = new[]
            {
                ".dx80.vtx",
                ".dx90.vtx",
                ".sw.vtx",
                ".phy",
                ".vvd"
            };

            foreach (string extension in extensions)
            {
                string path = Path.ChangeExtension(modelFilePath, extension);

                string file = _pathExplorer.GetFileSystemPath(path);

                if (file == null)
                {
                    continue;
                }

                _customMdls.Add(path);
            }
        }

        private void InspectVmtFile(string vmtFilePath, SearchPathExplorer pathExplorer)
        {
            using (var vmtStream = new FileStream(vmtFilePath, FileMode.Open))
            using (var vmtReader = new BinaryReader(vmtStream))
            {
                var material = new SourceMaterial(vmtReader, (int)vmtStream.Length);

                foreach (string texture in material.GetTextures())
                {
                    if (texture == null)
                    {
                        continue;
                    }

                    string customTexturePath = pathExplorer.GetFileSystemPath($"materials/{texture}.vtf");

                    if (customTexturePath == null)
                    {
                        continue;
                    }

                    _customMaterials.Add($"materials/{texture}.vtf");
                }
            }
        }
    }
}
