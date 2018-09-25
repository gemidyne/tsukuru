using System.IO;

namespace Tsukuru.Maps.Compiler
{
    public static class VmfFileCopier
    {
        public static string CopyFile(string sourceVMF, string destinationFileName)
        {
            var destinationFullPath = Path.Combine(Path.GetDirectoryName(sourceVMF), "generated");

            if (!Directory.Exists(destinationFullPath))
            {
                Directory.CreateDirectory(destinationFullPath);
            }

            var destinationFullFile = Path.Combine(destinationFullPath, destinationFileName + ".vmf");

            File.Copy(sourceVMF, destinationFullFile, true);

            return destinationFullFile;
        }
    }
}
