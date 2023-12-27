using System.IO;
using Humanizer;
using ICSharpCode.SharpZipLib.BZip2;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps;

internal class CompressBspToBzip2Step : ICompileStep
{
    public string StepName => "Compress BSP to BZip2";

    public bool Run(ResultsLogContainer log)
    {
        if (!MapCompileSessionInfo.Instance.GeneratedBspFile.Exists)
        {
            log.AppendLine("BZ2", $"No file to compress at {MapCompileSessionInfo.Instance.GeneratedBspFile.FullName}");
            return false;
        }
        
        long bspSize = MapCompileSessionInfo.Instance.GeneratedBspFile.Length;
        log.AppendLine("Info", $"BSP file size is: {bspSize.Bytes().ToString()}");

        var bz2 = new FileInfo(MapCompileSessionInfo.Instance.GeneratedBspFile.FullName + ".bz2");

        using (var input = MapCompileSessionInfo.Instance.GeneratedBspFile.OpenRead())
        using (var output = bz2.Create())
        {
            log.AppendLine("BZ2", "Compressing...");
            BZip2.Compress(input, output, true, 4096);
        }
        
        long bz2Size = bz2.Length;
        
        log.AppendLine("Info", $"BZ2 file size is: {bz2.Length.Bytes().ToString()}");
        
        log.AppendLine("Info", $"BZ2 version saves roughly {(bspSize - bz2Size).Bytes().ToString()}");

        return true;
    }
}