namespace Tsukuru.Maps.Compiler
{
    public class VvisCompilationSettings : ICompilationSettings
    {
        public bool Fast { get; set; }

        public bool LowPriority { get; set; }

        public string OtherArguments { get; set; }

        public string GetArguments()
        {
            return string.Format("{0} {1} {2}",
                Fast ? "-fast" : string.Empty,
                LowPriority ? "-low" : string.Empty,
                OtherArguments);
        }
    }
}
