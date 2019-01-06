using System.Numerics;
using System.Runtime.InteropServices;

namespace Tsukuru.SourceEngineTools.Bsp.LumpData
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TextureData
    {
        public Vector3 Reflectivity;
        public int NameStringTableId;
        public int Width;
        public int Height;
        public int ViewWidth;
        public int viewHeight;
    }
}
