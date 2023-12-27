using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData.GameLumps;

public struct StaticProp
{
    //v4+
    public Vector3 Origin;
    public Vector3 Angles;
    public ushort PropType;
    public ushort FirstLeaf;
    public ushort LeafCount;
    [MarshalAs(UnmanagedType.U1)]
    public bool Solid;
    public StaticPropFlags Flags;
    public int Skin;
    public float FadeMinDistance;
    public float FadeMaxDistance;
    public Vector3 LightingOrigin;
    //v5+
    public float ForcedFadeScale;
    //v6 & v7
    public ushort MinDxLevel;
    public ushort MaxDxLevel;
    //v8+
    public byte MinCpuLevel;
    public byte MaxCpuLevel;
    public byte MinGpuLevel;
    public byte MaxGpuLevel;
    //v7+
    public uint DiffuseModulation;
    //v10+
    public float Unknown;
    //v9+
    [MarshalAs(UnmanagedType.U4)]
    public bool DisableX360;

    public static StaticProp Read(BinaryReader reader, int version)
    {
        var start = reader.BaseStream.Position;
        var staticProp = new StaticProp();
        staticProp.Origin = reader.ReadStruct<Vector3>();
        staticProp.Angles = reader.ReadStruct<Vector3>();
        staticProp.PropType = reader.ReadUInt16();
        staticProp.FirstLeaf = reader.ReadUInt16();
        staticProp.LeafCount = reader.ReadUInt16();
        staticProp.Solid = reader.ReadByte() == 1;
        staticProp.Flags = (StaticPropFlags)reader.ReadByte();
        staticProp.Skin = reader.ReadInt32();
        staticProp.FadeMinDistance = reader.ReadSingle();
        staticProp.FadeMaxDistance = reader.ReadSingle();
        staticProp.LightingOrigin = reader.ReadStruct<Vector3>();

        if (version >= 5)
        {
            staticProp.ForcedFadeScale = reader.ReadSingle();
        }
        if (version == 6 || version == 7)
        {
            staticProp.MinDxLevel = reader.ReadUInt16();
            staticProp.MaxDxLevel = reader.ReadUInt16();
        }
        if (version >= 8)
        {
            staticProp.MinCpuLevel = reader.ReadByte();
            staticProp.MaxCpuLevel = reader.ReadByte();
            staticProp.MinGpuLevel = reader.ReadByte();
            staticProp.MaxGpuLevel = reader.ReadByte();
        }
        if (version >= 7)
        {
            staticProp.DiffuseModulation = reader.ReadUInt32();
        }
        if (version >= 10)
        {
            staticProp.Unknown = reader.ReadSingle();
        }
        if (version >= 9)
        {
            staticProp.DisableX360 = reader.ReadInt32() == 1;
        }
        var structLength = reader.BaseStream.Position - start;
        return staticProp;
    }
}