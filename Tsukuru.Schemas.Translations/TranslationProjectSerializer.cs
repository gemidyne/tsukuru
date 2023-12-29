using Newtonsoft.Json;

namespace Tsukuru.Schemas.Translations;

public class TranslationProjectSerializer : ITranslationProjectSerializer
{
    private readonly JsonSerializerSettings _settings = new()
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