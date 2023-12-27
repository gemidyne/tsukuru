using System;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData.GameLumps;

[Flags]
public enum StaticPropFlags : byte
{
    Fades = 1,
    UseLightingOrigin = 2,
    NoDraw = 4,
    IgnoreNormals = 8,
    NoShadow = 0x10,
    Unused = 0x20,
    NoPerVertexLighting = 0x40,
    NoSelfShadowing = 0x80
}