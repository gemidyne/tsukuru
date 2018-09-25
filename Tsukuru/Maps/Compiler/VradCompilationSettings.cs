namespace Tsukuru.Maps.Compiler
{
    public class VradCompilationSettings : ICompilationSettings
    {
        public bool LDR { get; set; }

        public bool HDR { get; set; }

        public bool Fast { get; set; }

        public bool Final { get; set; }

        public bool StaticPropLighting { get; set; }

        public bool StaticPropPolys { get; set; }

        public bool TextureShadows { get; set; }

        public bool LowPriority { get; set; }

        public string OtherArguments { get; set; }

        public string GetArguments()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                HDR && !LDR ? "-hdr" : "",
                HDR && LDR ? "-both" : "",
                !HDR && LDR ? "-ldr" : "",
                Fast && !Final ? "-fast" : "",
                Final && !Fast ? "-final" : "",
                StaticPropLighting ? "-StaticPropLighting" : "",
                StaticPropPolys ? "-StaticPropPolys" : "",
                TextureShadows ? "-TextureShadows" : "",
                LowPriority ? "-low" : "",
                OtherArguments);
        }
    }
}
