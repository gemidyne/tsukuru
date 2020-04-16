using System.Media;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Steam;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class CompileConfirmationViewModel : ViewModelBase
    {
        public RelayCommand MapCompileCommand { get; }

        public RelayCommand LaunchMapCommand { get; }

        public CompileConfirmationViewModel()
        {
            MapCompileCommand = new RelayCommand(DoMapCompile);
            LaunchMapCommand = new RelayCommand(DoMapLaunch);
        }

        private async void DoMapCompile()
        {
            await Task.Run(() =>
            {
                MapCompileInitialiser.Execute(this);
            });

            SystemSounds.Asterisk.Play();
        }

        private void DoMapLaunch()
        {
            SteamHelper.LaunchAppWithMap(MapCompileSessionInfo.Instance.MapName);

#warning  TODO Update to use something else
            //await DialogHost.Show(new ProgressSpinner(), async delegate (object sender, DialogOpenedEventArgs args)
            //{
            //    await Task.Delay(5000);
            //    args.Session.Close(false);
            //});
        }


    }
}
