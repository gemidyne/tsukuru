using System.Runtime.InteropServices;

namespace Tsukuru.SourceEngineTools.Mdl.Header
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Header2
    {
        public int SrcBoneTransformCount;
        public int SrcBoneTransformIndex;
        public int IllumPositionAttachmentIndex;
        public float FlMaxEyeDeflection;
        public int LinearBoneIndex;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public int[] Unknown;                
    }
}
