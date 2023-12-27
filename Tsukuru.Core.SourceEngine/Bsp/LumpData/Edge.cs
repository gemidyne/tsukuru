﻿using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Edge
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public ushort[] VertexIndex;
}