﻿using Newtonsoft.Json;

namespace Tsukuru.Core.Translations.Data;

public class IntegerFormatArgument : IFormatArgument
{
    [JsonProperty("description")]
    public string Description { get; set; }

    public string Render(int index)
    {
        return $"{{{index}:i}}";
    }
}