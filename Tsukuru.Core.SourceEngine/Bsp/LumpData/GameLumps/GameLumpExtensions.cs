namespace Tsukuru.Core.SourceEngine.Bsp.LumpData.GameLumps;

public static class GameLumpExtensions
{
    public static StaticProps GetStaticProps(this GameLump lump)
    {
        return lump.Lumps[StaticProps.IdName].Data as StaticProps;
    }
}