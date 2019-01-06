using System.IO;
using ICSharpCode.SharpZipLib.BZip2;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
	internal class CompressBspToBzip2Step : ICompileStep
	{
		public string StepName => "Compress BSP to BZip2";

		public bool Run(ILogReceiver log)
		{
			string fileName = MapCompileSessionInfo.Instance.GeneratedBspFile;

			if (!File.Exists(fileName))
			{
				log.WriteLine("CompressBspToBzip2Step", $"No file to compress at {fileName}");
				return false;
			}

			using (var input = File.OpenRead(fileName))
			using (var output = File.Create(fileName + ".bz2"))
			{
				log.WriteLine("CompressBspToBzip2Step", "Compressing...");
				BZip2.Compress(input, output, true, 4096);
			}

			return true;
		}
	}
}
