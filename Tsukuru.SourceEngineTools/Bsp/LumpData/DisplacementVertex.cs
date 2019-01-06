using System.Numerics;
using System.Runtime.InteropServices;

namespace Tsukuru.SourceEngineTools.Bsp.LumpData
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DisplacementVertex
    {
        public Vector3 Vector;
        public float Distance;
        public float Alpha;
    }
}
