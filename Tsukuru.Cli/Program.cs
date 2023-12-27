using CommandLine;
using Tsukuru.Cli;
using Tsukuru.Core.Translations;

return Parser
    .Default
    .ParseArguments<TranslationValidationOptions, TranslationExportOptions>(args)
    .MapResult(
        (TranslationValidationOptions options) => DoValidateTranslations(options),
        (TranslationExportOptions options) => DoExportTranslations(options),
        errs => 1);
        
static int DoValidateTranslations(TranslationValidationOptions options)
{
    var fileInfo = new FileInfo(options.ProjectFilePath);

    if (!fileInfo.Exists)
    {
        Console.WriteLine("Project file does not exist.");
        return 255;
    }

    try
    {
        var json = File.ReadAllText(fileInfo.FullName);

        var project = new TranslationProjectSerializer().Deserialize(json);

        if (project == null)
        {
            Console.WriteLine("Project file could not be deserialised. Please check for any formatting errors.");
            return 255;
        }
        
        Console.WriteLine($"Project file loaded successfully: {project.ProjectName} by {project.Author} ({project.Website})");
        
        Console.WriteLine($"{project.Languages.Count} languages in project");
        Console.WriteLine($"{project.Phrases.Count} phrases in project");

        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception thrown during project file deserialisation");
        Console.WriteLine(ex);
        return 255;
    }
}

static int DoExportTranslations(TranslationExportOptions options)
{
    var fileInfo = new FileInfo(options.ProjectFilePath);

    if (!fileInfo.Exists)
    {
        Console.WriteLine("Project file does not exist.");
        return 255;
    }

    try
    {
        new TranslatorEngine(new TranslationProjectSerializer()).ExportToSourceMod(fileInfo.FullName);

        Console.WriteLine("Project file exported successfully");

        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception thrown during project file export");
        Console.WriteLine(ex);
        return 255;
    }
}