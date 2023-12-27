using System.IO;

namespace Tsukuru.Core.Translations;

public class TranslatorEngine : ITranslatorEngine
{
    private readonly ITranslationProjectSerializer _translationProjectSerializer;

    public TranslatorEngine(
        ITranslationProjectSerializer translationProjectSerializer)
    {
        _translationProjectSerializer = translationProjectSerializer;
    }
    
    public EProjectGenerateResult ImportFromSourceMod(string translationFileName)
    {
        var file = new FileInfo(translationFileName);

        if (!file.Exists)
        {
            return EProjectGenerateResult.SourceFileNotFound;
        }

        var transformer = new TranslationImportTransformer(file, _translationProjectSerializer);

        return transformer.GenerateProject();
    }

    public void ExportToSourceMod(string translationProjectFileName)
    {
        var file = new FileInfo(translationProjectFileName);

        if (!file.Exists)
        {
            return;
        }

        var exporter = new TranslationExporter(_translationProjectSerializer);

        exporter.Load(file);
        exporter.ExportToFileSystem();
    }
}