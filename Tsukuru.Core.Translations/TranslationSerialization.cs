using Newtonsoft.Json;

namespace Tsukuru.Core.Translations
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