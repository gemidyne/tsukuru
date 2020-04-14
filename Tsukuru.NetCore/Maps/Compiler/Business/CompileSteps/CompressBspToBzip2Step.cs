using System.IO;
using ICSharpCode.SharpZipLib.BZip2;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class CompressBspToBzip2Step : ICompileStep
    {
        public string StepName => "Compress BSP to BZip2";

        public bool Run(ILogReceiver log)
        {
            if (!MapCompileSessionInfo.Instance.GeneratedBspFile.Exists)
            {
                log.WriteLine("BZ2", $"No file to compress at {MapCompileSessionInfo.Instance.GeneratedBspFile.FullName}");
                return false;
            }

            using (var input = MapCompileSessionInfo.Instance.GeneratedBspFile.OpenRead())
            using (var output = File.Create(MapCompileSessionInfo.Instance.GeneratedBspFile.FullName + ".bz2"))
            {
                log.WriteLine("BZ2", "Compressing...");
                BZip2.Compress(input, output, true, 4096);
            }

            return true;
        }
    }
}
