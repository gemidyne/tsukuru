using System;
using System.IO;

namespace Tsukuru.Maps.Compiler.Business
{
    internal abstract class BaseVProjectStep : ICompileStep
    {
        private string _gameMapsFolderPath;

        public string VProject => VProjectHelper.Path;

        public string SdkToolsPath
        {
            get
            {
                var parent = Directory.GetParent(VProject);
                return parent.FullName;
            }
        }

        public string GameMapsFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_gameMapsFolderPath))
                {
                    _gameMapsFolderPath = Path.Combine(VProject, "maps");

                    if (!Directory.Exists(_gameMapsFolderPath))
                    {
                        throw new DirectoryNotFoundException("Unable to determine game maps folder. Ensure your VProject is the game folder and not the root folder. (Example: \"SteamApps\\Common\\Team Fortress 2\\tf\")");
                    }
                }

                return _gameMapsFolderPath;
            }
        }

        public abstract string StepName { get; }

        public abstract bool Run(ILogReceiver log);
    }
}