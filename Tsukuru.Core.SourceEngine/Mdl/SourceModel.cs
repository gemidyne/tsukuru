using System.IO;
using Tsukuru.Core.SourceEngine.Mdl.Header;

namespace Tsukuru.Core.SourceEngine.Mdl
{
    public class SourceModel 
    {
        public Header.Header Header
        {
            get; private set;
        }
        public Header2 Header2
        {
            get; private set;
        }

        public string[] Materials
        {
            get; private set;
        }

        public string[] MaterialPaths
        {
            get; private set;
        }

        public Texture[] Textures
        {
            get; private set;
        }

        public BodyPart[] BodyParts
        {
            get; private set;
        }

        public SourceModel(BinaryReader reader)
        {
            var start = reader.BaseStream.Position;
            Header = reader.ReadStruct<Header.Header>();
            var postHeader = reader.BaseStream.Position;

            reader.BaseStream.Position = Header.StudioHDR2Index;
            Header2 = reader.ReadStruct<Header2>();

            reader.BaseStream.Position = start + Header.TextureDirOffset;
            MaterialPaths = new string[Header.TextureDirCount];
            for (var i = 0; i < Header.TextureDirCount; i++)
            {
                var offset = reader.ReadInt32();
                var pos = reader.BaseStream.Position;
                reader.BaseStream.Position = start + offset;
                var path = reader.ReadNullTerminatedAsciiString();
                MaterialPaths[i] = path;
                reader.BaseStream.Position = pos;
            }

            reader.BaseStream.Position = start + Header.TextureOffset;
            Textures = new Texture[Header.TextureCount];
            Materials = new string[Header.TextureCount];
            for (var i = 0; i < Textures.Length; i++)
            {
                var pos = reader.BaseStream.Position;
                Textures[i] = reader.ReadStruct<Texture>();
                reader.BaseStream.Position = pos + Textures[i].NameOffset;
                Materials[i] = reader.ReadNullTerminatedAsciiString();
                reader.BaseStream.Position = pos;
            }

            reader.BaseStream.Position = start + Header.BodyPartOffset;
            BodyParts = new BodyPart[Header.BodyPartCount];
            for (var i = 0; i < Header.BodyPartCount; i++)
            {
                var bodyPartHeader = reader.ReadStruct<BodyPartHeader>();
                BodyParts[i] = new BodyPart(bodyPartHeader, reader);
            }

            reader.BaseStream.Position = start + Header.TextureOffset;
        }

        public override string ToString()
        {
            return $"SourceModel {Header.Name} with {Textures.Length} Textures and {BodyParts.Length} BodyParts";
        }
    }
}
