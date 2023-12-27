using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct Leaf
{
    public Brush.BrushContents Contents;
    public short Cluster;
    public short AreaFlags;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public short[] Mins;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public short[] Maxs;
    public ushort FirstLeafFace;
    public ushort LeafFacesCount;
    public ushort FirstLeafBrush;
    public ushort LeafBrushesCount;
    public short LeafWaterDataId;
}