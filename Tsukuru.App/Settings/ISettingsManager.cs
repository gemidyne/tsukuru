namespace Tsukuru.Settings;

public interface ISettingsManager
{
    SettingsManifest Manifest { get; }
    void Load();
    void Save();
}