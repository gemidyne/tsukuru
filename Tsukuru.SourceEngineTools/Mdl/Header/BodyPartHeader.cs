using System.Runtime.InteropServices;

namespace Tsukuru.SourceEngineTools.Mdl.Header
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BodyPartHeader
    {
        public int NameIndex;
        public int ModelCount;
        public int Base;
        public int ModelOffset;
    }
}
