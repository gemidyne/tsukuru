using Tsukuru.Properties;

namespace Tsukuru
{
    public static class SettingsManager
    {
        public static string CompilerLocation
        {
            get
            {
                return Settings.Default.CompilerLocation;
            }

            set
            {
                Settings.Default.CompilerLocation = value;
                Settings.Default.Save();
            }
        }

        public static bool ExecutePostBuildScripts
        {
            get
            {
                return Settings.Default.ExecutePostBuildScripts;
            }

            set
            {
                Settings.Default.ExecutePostBuildScripts = value;
                Settings.Default.Save();
            }
        }

        public static bool IncrementVersion
        {
            get
            {
                return Settings.Default.IncrementVersion;
            }

            set
            {
                Settings.Default.IncrementVersion = value;
                Settings.Default.Save();
            }
        }
    }
}
