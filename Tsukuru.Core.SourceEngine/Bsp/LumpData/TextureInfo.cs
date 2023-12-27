using System.Numerics;
using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TextureInfo
{
    public Vector4 TextureVecsS;
    public Vector4 TextureVecsT;
    public Vector4 LightmapVecsS;
    public Vector4 LightmapVecsT;
    public int Flags;
    public int TextureData;
}