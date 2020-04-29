using System;
using System.IO;
using Newtonsoft.Json;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler
{
    internal static class SettingsImporter
    {
        public static bool Import(FileInfo file)
        {
            if (!file.Exists)
            {
                return false;
            }

            using (var stream = file.OpenText())
            {
                try
                {
                    var json = stream.ReadToEnd();

                    var settings = JsonConvert.DeserializeObject<MapCompilerSettings>(json);

                    SettingsManager.Manifest.MapCompilerSettings = settings;
                    SettingsManager.Save();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
