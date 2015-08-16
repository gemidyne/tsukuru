using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Tsukuru.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                return;
            }

            string file = string.Join("", args);

            Version ver;

            GetVersionFile(Path.GetDirectoryName(file), out ver);

            Console.WriteLine("File to be rewritten: {0}", file);
            Console.WriteLine("Version: {0}", ver);
            RewriteSourceFile(file, ver);
        }

        private static void RewriteSourceFile(string file, Version version)
        {
            var fileTxt = new StringBuilder();

            foreach (string line in File.ReadAllLines(file))
            {
                fileTxt.AppendLine(line.Replace("{BBVersion}", version.ToString()));
            }

            File.WriteAllText(file, fileTxt.ToString());
        }

        private static void GetVersionFile(string workingDirectory, out Version version)
        {
            string versionFile = Path.Combine(workingDirectory, "version");
            version = new Version(1, 0, 0, 0);

	        if (!File.Exists(versionFile))
	        {
		        return;
	        }

	        string text = File.ReadAllText(versionFile);

	        if (Version.TryParse(text, out version))
	        {
		        int newMinor = version.Minor;
		        int newBuild = version.Build;
		        int newRev = version.Revision;

		        version = new Version(version.Major, newMinor, newBuild, newRev);
	        }
	        else
	        {
		        version = new Version(1, 0, 0, 0);
	        }
        }
    }
}
