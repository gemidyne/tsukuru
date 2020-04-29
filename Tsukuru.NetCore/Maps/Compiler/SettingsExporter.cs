using System;
using System.IO;
using Newtonsoft.Json;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler
{
    internal static class SettingsExporter
    {
        public static bool Export(FileInfo file)
        {
            try
            {
                var json = JsonConvert.SerializeObject(SettingsManager.Manifest.MapCompilerSettings, Formatting.Indented);

                File.WriteAllText(file.FullName, json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}