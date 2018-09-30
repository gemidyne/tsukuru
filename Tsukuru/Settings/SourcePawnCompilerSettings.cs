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
    }
}