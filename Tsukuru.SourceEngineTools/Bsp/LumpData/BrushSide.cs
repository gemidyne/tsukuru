using System.Runtime.InteropServices;

namespace Tsukuru.SourceEngineTools.Bsp.LumpData
{
    [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
    public struct BrushSide
    {
        public ushort PlaneNumber;
        public short TextureInfo;
        public short DisplacementInfo;
        [MarshalAs(UnmanagedType.U1)]
        public bool IsBevelPlane;
    }
}
