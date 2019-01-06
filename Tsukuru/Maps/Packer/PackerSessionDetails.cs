using System.Collections.Generic;
using System.IO;

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

        public List<string> CompleteFoldersToAdd { get; set; }

		public List<string> IntelligentFoldersToAdd { get; set; }

        public string FileListFile => Path.Combine(Path.GetDirectoryName(MapFile), Path.GetFileNameWithoutExtension(MapFile) + "-filelist.txt");
    }
}
