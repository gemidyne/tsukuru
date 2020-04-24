using Tsukuru.Core.SourceEngine;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class GameInfoViewModel : ViewModelBaseWithValidation, IApplicationContentView
    {
        private static readonly object _door = new object();
        private bool _isLoading;
        private string _gameName;
        private string _vProject;
        private string _steamAppId;

        public string Name => "Game information";

        public string Description =>
            "This page shows you the loaded game information that will be used to compile the map.";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public string VProject
        {
            get => _vProject;
            set
            {
                Set(() => VProject, ref _vProject, value);
                RaisePropertyChanged(nameof(IsVProjectSet));
                RaisePropertyChanged(nameof(IsVProjectNotSet));
            }
        }

        public bool IsVProjectSet => !IsVProjectNotSet;

        public bool IsVProjectNotSet => string.IsNullOrWhiteSpace(VProject);

        public string GameName
        {
            get => _gameName;
            set => Set(() => GameName, ref _gameName, value);
        }

        public string SteamAppId
        {
            get => _steamAppId;
            set => Set(() => SteamAppId, ref _steamAppId, value);
        }

        public GameInfoViewModel()
        {
            if (IsInDesignMode)
            {
                VProject = "??";
            }
            else
            {
                VProject = VProjectHelper.Path;
                SteamAppId = GameInfoHelper.GetAppId()?.ToString() ?? "Unknown";
                GameName = GameInfoHelper.GetGameInfo();
            }
        }

        public void Init()
        {
            lock (_door)
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    ClearValidationErrors(nameof(VProject));

                    var errors = new[]
                    {
                        "The VProject environment variable is not set. You need to set this before you can use the Map Compiler.",
                        "Your environment variable should be set to the game directory. For example: C:\\Program Files (x86)\\Steam\\steamapps\\common\\Team Fortress 2\\tf"
                    };

                    foreach (string error in errors)
                    {
                        AddValidationError(nameof(VProject), error);
                    }
                }
            }
        }
    }
}