using Tsukuru.Core.SourceEngine;
using Tsukuru.Steam;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class GameInfoViewModel : ViewModelBaseWithValidation, IApplicationContentView
    {
        private static readonly object _door = new object();
        private bool _isLoading;
        private string _gameInfo;
        private string _vProject;

        public string Name => "Game information";

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

        public string GameInfo
        {
            get => _gameInfo;
            set => Set(() => GameInfo, ref _gameInfo, value);
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
                GameInfo = GameHelper.GetGameInfo();
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