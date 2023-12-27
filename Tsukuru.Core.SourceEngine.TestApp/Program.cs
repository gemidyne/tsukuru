using System;
using System.IO;
using System.Linq;

namespace Tsukuru.Core.SourceEngine.TestApp;

class Program : IProgress<string>
{
    static void Main(string[] args)
    {
        string vproject = VProjectHelper.Path;

        Console.WriteLine($"VPROJECT: {vproject}");

        var paths = GameInfoHelper.GetAllVpkPaths();

        Console.WriteLine("VPK Paths");

        foreach (var path in paths)
        {
            Console.WriteLine(path.FullName);
        }

        var results = BspDependencyAnalyser.Analyse(new Program(), vproject, Path.Combine(vproject, "maps\\warioware_redux_master-20200321.bsp"));

        Console.WriteLine("Custom materials:");

        foreach (string customMaterial in results.CustomMaterials.OrderBy(x => x))
        {
            Console.WriteLine($"- {customMaterial}");
        }

        Console.WriteLine("Custom models:");

        foreach (string customMdl in results.CustomModels.OrderBy(x => x))
        {
            Console.WriteLine($"- {customMdl}");
        }

        Console.WriteLine("Custom sounds:");

        foreach (string file in results.CustomSounds.OrderBy(x => x))
        {
            Console.WriteLine($"- {file}");
        }

        Console.ReadLine();
    }


    public void Report(string value)
    {
        Console.WriteLine(value);
    }
}