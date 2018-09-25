namespace Tsukuru.Maps.Compiler
{
    public class VbspCompilationSettings : ICompilationSettings
    {
        public bool OnlyEntities { get; set; }

        public bool OnlyProps { get; set; }

        public bool NoDetailEntities { get; set; }

        public bool NoWaterBrushes { get; set; }

        public bool LowPriority { get; set; }

        public bool KeepStalePackedData { get; set; }

        public string OtherArguments { get; set; }

        public string GetArguments()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}",
                OnlyEntities ? "-onlyents" : string.Empty,
                OnlyProps ? "-onlyprops" : string.Empty,
                NoDetailEntities ? "-nodetail" : string.Empty,
                NoWaterBrushes ? "-nowater" : string.Empty,
                LowPriority ? "-low" : string.Empty,
                KeepStalePackedData ? "-keepstalezip" : string.Empty,
                OtherArguments);
        }
    }
}
