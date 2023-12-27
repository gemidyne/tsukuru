using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData.GameLumps;

public class StaticProps : LumpData
{
    public const string IdName = "sprp";

    public List<string> Models
    {
        get; private set;
    }

    public List<ushort> Leafs
    {
        get; private set;
    }

    public List<StaticProp> Props
    {
        get; private set;
    }

    public StaticProps(BinaryReader reader, long length, int version)
    {
        var startPosition = reader.BaseStream.Position;

        var modelCount = reader.ReadInt32();
        Models = new List<string>(modelCount);
        for (var i = 0; i < modelCount; i++)
        {
            var bytes = reader.ReadBytes(128);
            string entry = Encoding.ASCII.GetString(bytes);
            Models.Add(entry.Replace("\0", string.Empty).Trim());
        }

        var leafCount = reader.ReadInt32();
        Leafs = new List<ushort>(leafCount);
        for (var i = 0; i < leafCount; i++)
        {
            Leafs.Add(reader.ReadUInt16());
        }

        var remainingLength = length - reader.BaseStream.Position + startPosition;
        var propCount = reader.ReadInt32();
        Props = new List<StaticProp>(propCount);
        for (int i = 0; i < propCount; i++)
        {
            Props.Add(StaticProp.Read(reader, version));
        }
    }

    private int TryRead(BinaryReader reader, int length, int propCount)
    {
        var start = reader.BaseStream.Position;
        try
        {
            for (int i = 0; i < propCount; i++)
            {
                var pos = reader.BaseStream.Position;
                reader.BaseStream.Position += 24;
                var index = reader.ReadUInt16();
                var model = Models[Props[i].PropType];
                reader.BaseStream.Position = pos + length;
            }
            return length;
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("unsuccessful using: " + length);
            reader.BaseStream.Position = start;
            return TryRead(reader, length + 1, propCount);
        }
    }
}