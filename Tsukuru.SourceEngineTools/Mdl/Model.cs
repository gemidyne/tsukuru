using System.IO;
using Tsukuru.SourceEngineTools.Mdl.Header;

namespace Tsukuru.SourceEngineTools.Mdl
{
    public class Model
    {
        public ModelHeader Header
        { get; private set; }

        public Mesh[] Meshes
        {
            get; private set;
        }

        public Model(ModelHeader header, BinaryReader reader)
        {
            this.Header = header;
            var start = reader.BaseStream.Position;
            reader.BaseStream.Position = start + header.MeshIndex;
            Meshes = new Mesh[header.MeshCount];
            for (var i = 0; i < header.MeshCount; i++)
            {
                var modelHeader = reader.ReadStruct<MeshHeader>();
                Meshes[i] = new Mesh(modelHeader, reader);
            }
        }


        public override string ToString()
        {
            return $"Model {Header.Name} with {Header.MeshCount} models";
        }
    }
}
