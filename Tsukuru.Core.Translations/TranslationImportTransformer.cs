using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chiaki;
using SteamKit2;
using Tsukuru.Core.Translations.Data;

namespace Tsukuru.Core.Translations;

public class TranslationImportTransformer
{
    private readonly FileInfo _sourceFile;
    private readonly ITranslationProjectSerializer _translationProjectSerializer;
    private KeyValue _baseTranslationSet;
    private TranslatorProjectSchema _project;

    public TranslationImportTransformer(
        FileInfo sourceFile,
        ITranslationProjectSerializer translationProjectSerializer)
    {
        _sourceFile = sourceFile;
        _translationProjectSerializer = translationProjectSerializer;
    }

    public EProjectGenerateResult GenerateProject()
    {
        if (!_sourceFile.Exists)
        {
            return EProjectGenerateResult.SourceFileNotFound;
        }

        _baseTranslationSet = KeyValue.LoadAsText(_sourceFile.FullName);

        if (_baseTranslationSet == null)
        {
            return EProjectGenerateResult.BadRootTranslationFile;
        }

        if (!DoGenerateProject())
        {
            return EProjectGenerateResult.GeneralFailure;
        }

        TryPopulateOtherCultures();

        var destination = new FileInfo(_sourceFile.DirectoryName.AppendIfNeeded('\\') + "translations.tsutproj");

        _translationProjectSerializer.SerializeToDisk(destination.FullName, _project);
        
        return EProjectGenerateResult.CompleteNoErrors;
    }

    private bool DoGenerateProject()
    {
        _project = new TranslatorProjectSchema();

        _project.Languages.Add(new Language { Code = "en" });

        foreach (var item in _baseTranslationSet.Children)
        {
            var kvFormatting = item.Children.FirstOrDefault(x => x.Name == "#format");
            var kvPhrase = item.Children.First(x => x.Name != "#format");

            var phrase = new Phrase
            {
                Key = item.Name,
                EnglishText = kvPhrase.Value,
            };

            if (kvFormatting != null)
            {
                var arguments = FormatArgumentFactory.CreateFromString(kvFormatting.Value);

                if (arguments == null)
                {
                    return false;
                }

                phrase.FormatArguments.AddRange(arguments);
            }

            _project.Phrases.Add(phrase);
        }

        return true;
    }

    private void TryPopulateOtherCultures()
    {
        var files = GenerateFilesToTest().Where(x => x.Value.Exists).ToArray();

        foreach (var pair in files)
        {
            KeyValue kv = KeyValue.LoadAsText(pair.Value.FullName);

            if (kv == null)
            {
                continue;
            }

            _project.Languages.Add(new Language { Code = pair.Key });

            foreach (var item in kv.Children)
            {
                var kvPhrase = item.Children.First(x => x.Name != "#format");

                var phrase = _project.Phrases.SingleOrDefault(x => x.Key == item.Name);

                if (phrase == null)
                {
                    continue;
                }

                phrase.Translations[pair.Key] = kvPhrase.Value;
            }
        }
    }

    private IEnumerable<KeyValuePair<string, FileInfo>> GenerateFilesToTest()
    {
        string directory = _sourceFile.DirectoryName;
        string file = _sourceFile.Name;

        foreach (string language in SourceModLanguageList.Instance.Languages)
        {
            string path = directory.AppendIfNeeded('\\') + language.AppendIfNeeded('\\') + file;

            yield return new KeyValuePair<string, FileInfo>(language, new FileInfo(path));
        }
    }
}