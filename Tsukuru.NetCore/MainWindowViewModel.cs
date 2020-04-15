using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Chiaki;
using GalaSoft.MvvmLight;
using Tsukuru.ViewModels;

namespace Tsukuru
{
    public class MainWindowViewModel : ViewModelBase
    {
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
            _pages = new ObservableCollection<IApplicationContentView>(CreateAllPages());
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

        private IEnumerable<IApplicationContentView> CreateAllPages()
        {
            yield return new SourcePawn.ViewModels.SettingsViewModel();
            yield return new SourcePawn.ViewModels.SourcePawnCompileViewModel();
            yield return new SourcePawn.ViewModels.PostBuildActionsViewModel();
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