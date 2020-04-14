using System.IO;

namespace Tsukuru.Core.Translations
{
    public class TranslatorEngine
    {
        public EProjectGenerateResult ImportFromSourceMod(string translationFileName)
        {
            var file = new FileInfo(translationFileName);

            if (!file.Exists)
            {
                return EProjectGenerateResult.SourceFileNotFound;
            }

            var transformer = new TranslationImportTransformer(file);

            return transformer.GenerateProject();
        }

        public void ExportToSourceMod(string translationProjectFileName)
        {
            var file = new FileInfo(translationProjectFileName);

            if (!file.Exists)
            {
                return;
            }

            var exporter = new TranslationExporter();

            exporter.Load(file);
            exporter.ExportToFileSystem();
        }
    }
}
