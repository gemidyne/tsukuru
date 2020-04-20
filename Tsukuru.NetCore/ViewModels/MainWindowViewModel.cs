using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Chiaki;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace Tsukuru.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly object _door = new object();
        private readonly ObservableCollection<IApplicationContentView> _pages;

        public ReadOnlyObservableCollection<IApplicationContentView> Pages { get; }

        public ICollectionView PagesCollectionView { get; }

        public ICollectionView PagesInSelectedGroupCollectionView { get; }

        private IApplicationContentView _selectedPage;

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
                    }

                    Set(() => SelectedPage, ref _selectedPage, value);

                    PagesInSelectedGroupCollectionView.Refresh();
                    RaisePropertyChanged(nameof(SelectedNavigationGroup));
                }
            }
        }

        private readonly ObservableCollection<EShellNavigationPage> _navigationGroups;

        public ReadOnlyObservableCollection<EShellNavigationPage> NavigationGroups { get; }

        public ICollectionView NavigationGroupsCollectionView { get; }

        public EShellNavigationPage SelectedNavigationGroup
        {
            get => _selectedPage.Group;
            set
            {
                SelectedPage = Pages.FirstOrDefault(p => p.Group == value);
                PagesInSelectedGroupCollectionView.Refresh();
            }
        }

        public MainWindowViewModel()
        {
            _pages = new ObservableCollection<IApplicationContentView>(CreateAllPages().ToList());
            Pages = new ReadOnlyObservableCollection<IApplicationContentView>(_pages);
            PagesCollectionView = CollectionViewSource.GetDefaultView(Pages);
            PagesCollectionView.Filter = FilterPages;
            PagesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(IApplicationContentView.Group)));
            PagesInSelectedGroupCollectionView = new CollectionViewSource { Source = Pages }.View;
            PagesInSelectedGroupCollectionView.Filter = FilterPagesInSelectedGroup;
            SelectedPage = Pages.FirstOrDefault();

            _navigationGroups = new ObservableCollection<EShellNavigationPage>(Enum<EShellNavigationPage>.GetValues());
            NavigationGroups = new ReadOnlyObservableCollection<EShellNavigationPage>(_navigationGroups);
            NavigationGroupsCollectionView = CollectionViewSource.GetDefaultView(NavigationGroups);
            NavigationGroupsCollectionView.Filter = FilterNavigationGroups;
        }

        public void NavigateToPage<T>()
            where T : IApplicationContentView
        {
            var page = Pages.OfType<T>().SingleOrDefault();

            if (page == null)
            {
                return;
            }

            SelectedPage = page;
        }

        private IEnumerable<IApplicationContentView> CreateAllPages()
        {
            // SourcePawn Compiler
            yield return new SourcePawn.ViewModels.SettingsViewModel();
            yield return new SourcePawn.ViewModels.SourcePawnCompileViewModel();
            yield return new SourcePawn.ViewModels.PostBuildActionsViewModel();

            yield return new Maps.Compiler.ViewModels.GameInfoViewModel();
            yield return new Maps.Compiler.ViewModels.MapSettingsViewModel();
            yield return new Maps.Compiler.ViewModels.VbspCompilationSettingsViewModel();
            yield return new Maps.Compiler.ViewModels.VvisCompilationSettingsViewModel();
            yield return new Maps.Compiler.ViewModels.VradCompilationSettingsViewModel();
            yield return new Maps.Compiler.ViewModels.ResourcePackingViewModel();
            yield return new Maps.Compiler.ViewModels.TemplatingSettingsViewModel();
            yield return new Maps.Compiler.ViewModels.BspRepackViewModel();
            yield return new Maps.Compiler.ViewModels.PostCompileActionsViewModel();
            yield return new Maps.Compiler.ViewModels.CompileConfirmationViewModel();
            yield return new Maps.Compiler.ViewModels.ResultsViewModel();

            yield return new Translator.ViewModels.TranslatorImportViewModel();
            yield return new Translator.ViewModels.TranslatorExportViewModel();

            yield return new OptionsViewModel();
            yield return new AboutViewModel();
        }

        private bool FilterPages(object item)
        {
            var page = (IApplicationContentView)item;


            return true;
        }

        private bool FilterPagesInSelectedGroup(object item)
        {
            var page = (IApplicationContentView)item;

            if (SelectedPage == null)
                return false;

            return page.Group == SelectedPage.Group && FilterPages(page);
        }

        private bool FilterNavigationGroups(object item)
        {
            var group = (EShellNavigationPage)item;

            return true;
        }
    }
}