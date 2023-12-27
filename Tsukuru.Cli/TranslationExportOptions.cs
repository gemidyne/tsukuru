using CommandLine;

namespace Tsukuru.Cli;

[Verb("export-translations", HelpText = "Exports a translation project to SourceMod files.")]
public class TranslationExportOptions
{
    [Option('f', "file", Required = true, HelpText = "Project file to be exported")]
    public string ProjectFilePath { get; set; }
}