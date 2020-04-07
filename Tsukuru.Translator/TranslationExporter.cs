using System.IO;
using System.Linq;
using Chiaki;
using Newtonsoft.Json;
using SteamKit2;
using Tsukuru.Translator.Data;

namespace Tsukuru.Translator
{
    internal class TranslationExporter
    {
        private readonly FileInfo _projectFilePath;
        private TranslatorProjectSchema _project;

        public TranslationExporter(FileInfo projectFilePath)
        {
            _projectFilePath = projectFilePath;
        }

        public void Load()
        {
            string json = File.ReadAllText(_projectFilePath.FullName);

            _project = JsonConvert.DeserializeObject<TranslatorProjectSchema>(json, TranslationSerialization.Settings);
        }

        public void Export()
        {
            DoRootExport();

            foreach (var language in SourceModLanguageList.Instance.Languages.Where(x => x != "en"))
            {
                DoLanguageExport(language);
            }
        }

        private void DoRootExport()
        {
            var root = new KeyValue("Phrases");

            foreach (var phrase in _project.Phrases)
            {
                var kv = new KeyValue(phrase.Key);

                if (phrase.FormatArguments.Any())
                {
                    var arguments = phrase.FormatArguments.Select((arg, idx) => arg.Render(idx + 1)).ToArray();

                    string combined = string.Join(",", arguments);

                    kv.Children.Add(new KeyValue("#format", combined));
                }

                kv.Children.Add(new KeyValue("en", phrase.EnglishText));

                root.Children.Add(kv);
            }

            string outputFile = _projectFilePath.DirectoryName.AppendIfNeeded('\\') + _project.OutputFileName;

            root.SaveToFile(outputFile, asBinary: false);
        }

        private void DoLanguageExport(string languageCode)
        {
            if (_project.Phrases.Select(x => x.Translations.ContainsKey(languageCode)).All(x => !x))
            {
                // If all text for this language is not set, don't output file
                return;
            }

            var root = new KeyValue("Phrases");

            foreach (var phrase in _project.Phrases)
            {
                var kv = new KeyValue(phrase.Key);

                if (phrase.FormatArguments.Any())
                {
                    var arguments = phrase.FormatArguments.Select((arg, idx) => arg.Render(idx + 1)).ToArray();

                    string combined = string.Join(",", arguments);

                    kv.Children.Add(new KeyValue("#format", combined));
                }

                string value = phrase.Translations.ContainsKey(languageCode)
                    ? phrase.Translations[languageCode]
                    : string.Empty;

                kv.Children.Add(new KeyValue(languageCode, value));

                root.Children.Add(kv);
            }

            var outputFile = new FileInfo(_projectFilePath.DirectoryName.AppendIfNeeded('\\') + languageCode.AppendIfNeeded('\\') + _project.OutputFileName);

            if (!outputFile.Directory.Exists)
            {
                outputFile.Directory.Create();
            }

            root.SaveToFile(outputFile.FullName, asBinary: false);
        }
    }
}