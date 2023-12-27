using System.IO;
using Newtonsoft.Json;
using Tsukuru.Core.Translations.Data;

namespace Tsukuru.Core.Translations;

public class TranslationProjectSerializer : ITranslationProjectSerializer
{
    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    /// <inheritdoc />
    public TranslatorProjectSchema Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<TranslatorProjectSchema>(
            value: json,
            settings: _settings);
    }

    /// <inheritdoc />
    public string Serialize(TranslatorProjectSchema project)
    {
        return JsonConvert.SerializeObject(project, _settings);
    }

    /// <inheritdoc />
    public void SerializeToDisk(string fileName, TranslatorProjectSchema project)
    {
        var json = Serialize(project);
        
        File.WriteAllText(fileName, json);
    }
}