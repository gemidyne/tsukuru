namespace Tsukuru.Schemas.Translations;

public interface ITranslationProjectSerializer
{
    /// <summary>
    /// Deserializes a project from a json string
    /// </summary>
    /// <returns>
    /// Deserialized project schema if valid, otherwise null on error. 
    /// </returns>
    TranslatorProjectSchema Deserialize(string json);
    
    /// <summary>
    /// Serializes a project to JSON for saving to disk
    /// </summary>
    string Serialize(TranslatorProjectSchema project);

    /// <summary>
    /// Serializes a project to disk
    /// </summary>
    void SerializeToDisk(string fileName, TranslatorProjectSchema project);
}