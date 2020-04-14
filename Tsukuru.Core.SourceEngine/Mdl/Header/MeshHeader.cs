using System.Numerics;
using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Mdl.Header
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshHeader
    {
        public int Material;
        public int ModelOffset;
        public int VertexCount;
        public int VertexOffset;
        public int FlexCount;
        public int FlexOffset;
        public int MaterialType;
        public int MaterialParam;
        public int MeshId;
        public Vector3 Center;
        public MeshVertexData VertexData;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] Unused;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshVertexData
    {
        public int ModelVertexData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] LevelOfDetailVertexCount;
    }
}
