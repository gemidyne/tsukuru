using System;
using System.IO;
using Newtonsoft.Json;

namespace Tsukuru.Settings;

internal class SettingsManager : ISettingsManager
{
    private static readonly FileInfo _settingsPath = GetSettingsFilePath();
    private static readonly object _door = new object();

    public SettingsManifest Manifest { get; private set; }

    public SettingsManager()
    {
        Load();
    }

    public void Load()
    {
        if (App.IsInDesignMode)
        {
            Manifest = new SettingsManifest();
            return;
        }

        lock (_door)
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
                catch (Exception)
                {
                    Manifest = new SettingsManifest();
                }
            }
        }
    }

    public void Save()
    {
        if (App.IsInDesignMode)
        {
            Manifest = new SettingsManifest();
            return;
        }

        lock (_door)
        {
            EnsureDirectoryExists();

            var serialised = JsonConvert.SerializeObject(Manifest, Formatting.Indented);

            try
            {
                File.WriteAllText(_settingsPath.FullName, serialised);
            }
            catch (Exception)
            {
                // Ignore for now
            }
        }
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