using Newtonsoft.Json;

namespace Tsukuru.Translator
{
    internal static class TranslationSerialization
    {
        public static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };
    }
}