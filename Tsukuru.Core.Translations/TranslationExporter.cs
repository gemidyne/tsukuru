﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chiaki;
using SteamKit2;
using Tsukuru.Schemas.Translations;

namespace Tsukuru.Core.Translations;

public class TranslationExporter
{
    private readonly ITranslationProjectSerializer _translationProjectSerializer;
    private FileInfo _projectFilePath;
    private TranslatorProjectSchema _project;

    public TranslationExporter(
        ITranslationProjectSerializer translationProjectSerializer)
    {
        _translationProjectSerializer = translationProjectSerializer;
    }

    public void Load(FileInfo projectFilePath)
    {
        _projectFilePath = projectFilePath;

        string json = File.ReadAllText(_projectFilePath.FullName);

        LoadString(json);
    }

    public void LoadString(string json)
    {
        _project = _translationProjectSerializer.Deserialize(json);
    }

    public void ExportToFileSystem()
    {
        var englishTxtFile = new FileInfo(
            _projectFilePath
                .DirectoryName
                .AppendIfNeeded(Path.DirectorySeparatorChar) + _project.OutputFileName);

        if (englishTxtFile.Directory != null && !englishTxtFile.Directory.Exists)
        {
            englishTxtFile.Directory.Create();
        }

        GenerateExportEnglish()
            .SaveToFile(englishTxtFile.FullName, asBinary: false);

        foreach (var language in SourceModLanguages.Codes.Where(x => x != "en"))
        {
            DoLanguageExportToFileSystem(language);
        }
    }

    public Dictionary<string, KeyValue> GetLanguageExports()
    {
        var result = new Dictionary<string, KeyValue>
        {
            ["en"] = GenerateExportEnglish()
        };

        foreach (var language in SourceModLanguages.Codes.Where(x => x != "en"))
        {
            var kv = GenerateExportForLanguage(language);

            if (kv == null)
            {
                continue;
            }

            result[language] = kv;
        }

        return result;
    }

    private KeyValue GenerateExportEnglish()
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

        return root;
    }

    private void DoLanguageExportToFileSystem(string languageCode)
    {
        var kv = GenerateExportForLanguage(languageCode);

        if (kv == null)
        {
            return;
        }

        var outputFile = new FileInfo(_projectFilePath.DirectoryName.AppendIfNeeded(Path.DirectorySeparatorChar) + languageCode.AppendIfNeeded(Path.DirectorySeparatorChar) + _project.OutputFileName);

        if (outputFile.Directory != null && !outputFile.Directory.Exists)
        {
            outputFile.Directory.Create();
        }

        kv.SaveToFile(outputFile.FullName, asBinary: false);
    }

    private KeyValue GenerateExportForLanguage(string languageCode)
    {
        if (_project.Phrases.Select(x => x.Translations.ContainsKey(languageCode) && !string.IsNullOrWhiteSpace(x.Translations[languageCode])).All(x => !x))
        {
            // If all text for this language is not set, don't output file
            return null;
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
                ? phrase.Translations[languageCode] ?? phrase.EnglishText
                : phrase.EnglishText;

            kv.Children.Add(new KeyValue(languageCode, value));

            root.Children.Add(kv);
        }

        return root;
    }
}