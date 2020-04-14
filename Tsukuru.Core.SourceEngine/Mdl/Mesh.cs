using System.IO;
using Tsukuru.Core.SourceEngine.Mdl.Header;

namespace Tsukuru.Core.SourceEngine.Mdl
{
    public class Mesh
    {
        public MeshHeader Header
        { get; private set; }

        public Mesh(MeshHeader header, BinaryReader reader)
        {
            Header = header;
            var start = reader.BaseStream.Position;
        }

        public override string ToString()
        {
            return $"Mesh";
        }
    }
}
