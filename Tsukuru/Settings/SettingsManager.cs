using Newtonsoft.Json;
using System;
using System.IO;

namespace Tsukuru.Settings
{
    internal class SettingsManager
    {
        private static readonly FileInfo _settingsPath = GetSettingsFilePath();

        public static SettingsManifest Manifest { get; private set; }

        static SettingsManager()
        {
            Load();
        }

        public static void Load()
        {
            if (!_settingsPath.Exists)
            {
                Manifest = new SettingsManifest();
            }
            else
            {
                try
                {
                    using (var stream = _settingsPath.OpenText())
                    {
                        var data = stream.ReadToEnd();

                        Manifest = JsonConvert.DeserializeObject<SettingsManifest>(data);
                    }
                }
                catch (Exception ex)
                {
                    Manifest = new SettingsManifest();
                }
            }
        }

        public static void Save()
        {
            EnsureDirectoryExists();

            var serialised = JsonConvert.SerializeObject(Manifest);

            File.WriteAllText(_settingsPath.FullName, serialised);
        }

        private static FileInfo GetSettingsFilePath()
        {
            return new FileInfo(Path.Combine(GetLocalAppDataDirectory(), "GEMINIDevelopments", "Tsukuru", "settings.json"));
        }

        private static string GetLocalAppDataDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        private static void EnsureDirectoryExists()
        {
            _settingsPath.Refresh();

            if (!_settingsPath.Directory.Exists)
            {
                _settingsPath.Directory.Create();
            }
        }
    }
}