using CommandLine;

namespace Tsukuru.Cli;

[Verb("validate-translation", HelpText = "Validates a translation project.")]
public class TranslationValidationOptions
{
    [Option('f', "file", Required = true, HelpText = "Project file to be validated")]
    public string ProjectFilePath { get; set; }
}