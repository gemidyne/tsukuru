using System.Collections.Generic;
using Chiaki;

namespace Tsukuru.Translator.Data
{
    internal class SourceModLanguageList : Singleton<SourceModLanguageList>
    {
        private readonly string[] _codes = new[]
        {
            "en",
            "ar",
            "pt",
            "bg",
            "cze",
            "da",
            "nl",
            "fi",
            "fr",
            "de",
            "el",
            "he",
            "hu",
            "it",
            "jp",
            "ko",
            "lv",
            "lt",
            "no",
            "pl",
            "pt_p",
            "ro",
            "ru",
            "chi",
            "sk",
            "es",
            "sv",
            "zho",
            "th",
            "tr",
            "ua",
        };

        public IReadOnlyList<string> Languages => _codes;
    }
}
