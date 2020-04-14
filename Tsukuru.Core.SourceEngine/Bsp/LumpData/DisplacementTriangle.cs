using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DisplacementTriangle
    {
        public ushort Tags;
    }
}
