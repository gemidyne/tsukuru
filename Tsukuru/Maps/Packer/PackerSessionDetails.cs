using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Tsukuru.Maps.Packer
{
    public class PackerSessionDetails
    {
        /// <summary>
        /// Path to the game folder.
        /// </summary>
        public string GamePath { get; set; }

        /// <summary>
        /// Path to the map file.
        /// </summary>
        public string MapFile { get; set; }

        public List<string> FoldersToPackIn { get; set; }

        public string FileListFile => Path.Combine(Path.GetDirectoryName(MapFile), Path.GetFileNameWithoutExtension(MapFile) + "-filelist.txt");

        public static PackerSessionDetails FromXmlFile(string filePath)
        {
            var document = new XmlDocument();
            var result = new PackerSessionDetails();
            document.Load(filePath);

            var gamePath = document.SelectSingleNode("/packer/gamePath");
            var mapFile = document.SelectSingleNode("/packer/mapFile");
            var folders = document.SelectNodes("/packer/foldersToPackIn/folder");

            if (gamePath != null)
            {
                result.GamePath = gamePath.InnerText;
                result.GamePath = result.GamePath.Trim();
            }

            if (mapFile != null)
            {
                result.MapFile = mapFile.InnerText;
                result.MapFile = result.MapFile.Trim();
            }

            if (folders != null)
            {
                result.FoldersToPackIn = new List<string>();
                foreach (XmlNode node in folders)
                {
                    var text = node.InnerText;
                    text = text.Trim();
                    result.FoldersToPackIn.Add(text);
                }
            }

            return result;
        }
    }
}
