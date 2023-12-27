using System.IO;
using Chiaki;

namespace Tsukuru.Maps.Compiler.Business;

public class MapCompileSessionInfo
{
    private static MapCompileSessionInfo _instance;

    public string MapName { get; set; }

    public FileInfo InputVmfFile { get; set; }

    public FileInfo GeneratedVmfFile { get; set; }

    public string GeneratedFileNameNoExtension => GeneratedVmfFile.FullName.TrimEnd(".vmf");

    public FileInfo GeneratedBspFile => new FileInfo(GeneratedFileNameNoExtension + ".bsp");

    public static MapCompileSessionInfo Instance => _instance ??= new MapCompileSessionInfo();

    public static void Clear()
    {
        _instance = new MapCompileSessionInfo();
    }
}