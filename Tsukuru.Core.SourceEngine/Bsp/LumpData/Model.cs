﻿using System.Numerics;
using System.Runtime.InteropServices;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData;

[StructLayout(LayoutKind.Sequential, Size = 48, Pack = 1)]
public struct Model
{
    public Vector3 BoundingBoxFrom;
    public Vector3 BoundingBoxTo;
    public Vector3 Origin;
    public int HeadNode;
    public int FirstFace;
    public int FacesCount;
}