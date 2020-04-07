using Newtonsoft.Json;

namespace Tsukuru.Translator
{
    public static class TranslationSerialization
    {
        public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };
    }
}