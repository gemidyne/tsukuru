namespace Tsukuru.SourcePawn
{
    public class CompilationMessage
    {
        public int? FirstLine { get; set; }
        public int LastLine { get; set; }

        public string FileName { get; set; }
        public string Prefix { get; set; }
        public string Message { get; set; }

        public string RawLine { get; set; }

        public string LineNumberDisplay => FirstLine.HasValue 
            ? FirstLine + " - " + LastLine 
            : LastLine.ToString();
    }
}
