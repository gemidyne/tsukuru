using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using AdonisUI.Controls;
using Chiaki;
using GalaSoft.MvvmLight;

namespace Tsukuru.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly object _door = new object();
        
        private ObservableCollection<EShellNavigationPage> _navigationGroups;
        private ICollectionView _pagesCollectionView;
        private IApplicationContentView _selectedPage;
        private ICollectionView _navigationGroupsCollectionView;

        public ReadOnlyObservableCollection<IApplicationContentView> Pages { get; }

        public ICollectionView PagesCollectionView
        {
            get => _pagesCollectionView;
            set => Set(() => PagesCollectionView, ref _pagesCollectionView, value);
        }

        public ICollectionView PagesInSelectedGroupCollectionView { get; }


        public IApplicationContentView SelectedPage
        {
            get => _selectedPage;
            set
            {
                value ??= Pages.FirstOrDefault();

                lock (_door)
                {
                    if (value != null && !value.IsLoading)
                    {
                        Task.Run(() =>
                        {
                            value.IsLoading = true;
                            value.Init();
                        }).ContinueWith((task) => value.IsLoading = false);

                        SelectedNavigationGroup = value.Group;
                    }

                    Set(() => SelectedPage, ref _selectedPage, value);

                    RaisePropertyChanged(nameof(SelectedNavigationGroup));

                    PagesInSelectedGroupCollectionView.Refresh();
                }
            }
        }

        public ObservableCollection<EShellNavigationPage> NavigationGroups
        {
            get => _navigationGroups;
            set => Set(() => NavigationGroups, ref _navigationGroups, value);
        }


        public ICollectionView NavigationGroupsCollectionView
        {
            get => _navigationGroupsCollectionView;
            set => Set(() => NavigationGroupsCollectionView, ref _navigationGroupsCollectionView, value);
        }

        public EShellNavigationPage SelectedNavigationGroup
        {
            get => _selectedPage.Group;
            set
            {
                SelectedPage = Pages.FirstOrDefault(p => p.Group == value);
                PagesInSelectedGroupCollectionView.Refresh();
                RaisePropertyChanged(nameof(SelectedNavigationGroup));
            }
        }

        public MainWindowViewModel()
        {
            var pages = new ObservableCollection<IApplicationContentView>(
                new IApplicationContentView[]
                {

                    new SourcePawn.ViewModels.SettingsViewModel(),
                    new SourcePawn.ViewModels.PostBuildActionsViewModel(),
                    new SourcePawn.ViewModels.SourcePawnCompileViewModel(),

                    new Maps.Compiler.ViewModels.ImportSettingsViewModel(),
                    new Maps.Compiler.ViewModels.ExportSettingsViewModel(),
                    new Maps.Compiler.ViewModels.GameInfoViewModel(),
                    new Maps.Compiler.ViewModels.MapSettingsViewModel(),
                    new Maps.Compiler.ViewModels.VbspCompilationSettingsViewModel(),
                    new Maps.Compiler.ViewModels.VvisCompilationSettingsViewModel(),
                    new Maps.Compiler.ViewModels.VradCompilationSettingsViewModel(),
                    new Maps.Compiler.ViewModels.ResourcePackingViewModel(),
                    new Maps.Compiler.ViewModels.TemplatingSettingsViewModel(),
                    new Maps.Compiler.ViewModels.BspRepackViewModel(),
                    new Maps.Compiler.ViewModels.PostCompileActionsViewModel(),
                    new Maps.Compiler.ViewModels.CompileConfirmationViewModel(),
                    new Maps.Compiler.ViewModels.ResultsViewModel(),

                    new Translator.ViewModels.TranslatorImportViewModel(),
                    new Translator.ViewModels.TranslatorExportViewModel(),

                    new AboutViewModel(),
                    new OptionsViewModel()
                });

            Pages = new ReadOnlyObservableCollection<IApplicationContentView>(pages);
            PagesCollectionView = CollectionViewSource.GetDefaultView(Pages);
            PagesCollectionView.Filter = _ => true;
            PagesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(IApplicationContentView.Group)));
            PagesInSelectedGroupCollectionView = new CollectionViewSource { Source = Pages }.View;
            PagesInSelectedGroupCollectionView.Filter = FilterPagesInSelectedGroup;

            NavigationGroups = new ObservableCollection<EShellNavigationPage>(Enum<EShellNavigationPage>.GetValues());
            NavigationGroupsCollectionView = CollectionViewSource.GetDefaultView(NavigationGroups);
            NavigationGroupsCollectionView.Filter = _ => true;

            NavigateToPage<SourcePawn.ViewModels.SettingsViewModel>();
        }

        public void NavigateToPage<T>()
            where T : IApplicationContentView
        {
            var page = Pages.OfType<T>().SingleOrDefault();

            if (page == null)
            {
                MessageBox.Show($"No page found of type: {typeof(T)}");
                return;
            }

            SelectedPage = page;
        }

        private bool FilterPagesInSelectedGroup(object item)
        {
            var page = (IApplicationContentView)item;

            return SelectedPage != null && page.Group == SelectedPage.Group;
        }
    }
}