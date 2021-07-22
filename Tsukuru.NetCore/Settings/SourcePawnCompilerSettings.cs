using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tsukuru.Settings
{
    internal class SourcePawnCompilerSettings
    {
        [JsonProperty("compilerPath")]
        public string CompilerPath { get; set; }

        [JsonProperty("execPostBuildScripts")]
        public bool ExecutePostBuildScripts { get; set; }

        [JsonProperty("versioning")]
        public bool Versioning { get; set; }

        [JsonProperty("copyToClipboard")]
        public bool CopySmxOnSuccess { get; set; }

        [JsonProperty("lastUsedFiles")]
        public List<string> LastUsedFiles { get; set; } = new List<string>();
    }
}