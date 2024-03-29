﻿using System.Numerics;
using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
public struct Plane
{
    public Vector3 Normal;
    public float Dist;
    public int Type;
}