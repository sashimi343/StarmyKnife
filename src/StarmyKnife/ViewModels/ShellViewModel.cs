using System.Collections.ObjectModel;
using System.Windows.Input;

using MahApps.Metro.Controls;

using MaterialDesignThemes.Wpf;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using StarmyKnife.Constants;
using StarmyKnife.Properties;

namespace StarmyKnife.ViewModels;

public class ShellViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private IRegionNavigationService _navigationService;
    private HamburgerMenuItem _selectedMenuItem;
    private HamburgerMenuItem _selectedOptionsMenuItem;
    private DelegateCommand _goBackCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;
    private ICommand _menuItemInvokedCommand;
    private ICommand _optionsMenuItemInvokedCommand;

    public HamburgerMenuItem SelectedMenuItem
    {
        get { return _selectedMenuItem; }
        set { SetProperty(ref _selectedMenuItem, value); }
    }

    public HamburgerMenuItem SelectedOptionsMenuItem
    {
        get { return _selectedOptionsMenuItem; }
        set { SetProperty(ref _selectedOptionsMenuItem, value); }
    }

    public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        // See: https://pictogrammers.com/library/mdi/
        new HamburgerMenuIconItem() { Label = Resources.ShellChainConverterPage, Icon = PackIconKind.FileArrowLeftRight, Tag = PageKeys.ChainConverter },
        new HamburgerMenuIconItem() { Label = Resources.ShellGeneratorPage, Icon = PackIconKind.TextBoxPlus, Tag = PageKeys.Generator },
        new HamburgerMenuIconItem() { Label = Resources.ShellPrettyValidatorPage, Icon = PackIconKind.CodeTagsCheck, Tag = PageKeys.PrettyValidator },
        // TODO: Implement Csql
        //new HamburgerMenuIconItem() { Label = Resources.ShellCsqlPage, Icon = PackIconKind.CommaCircle, Tag = PageKeys.Csql },
        new HamburgerMenuIconItem() { Label = Resources.ShellXPathFinderPage, Icon = PackIconKind.TextSearchVariant, Tag = PageKeys.XPathFinder },
        new HamburgerMenuIconItem() { Label = Resources.ShellListConverterPage, Icon = PackIconKind.TableArrowRight, Tag = PageKeys.ListConverter },
    };

    public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuIconItem() { Label = Resources.ShellSettingsPage, Icon = PackIconKind.Settings, Tag = PageKeys.Settings }
    };

    public DelegateCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new DelegateCommand(OnGoBack, CanGoBack));

    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new DelegateCommand(OnLoaded));

    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new DelegateCommand(OnUnloaded));

    public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ?? (_menuItemInvokedCommand = new DelegateCommand(OnMenuItemInvoked));

    public ICommand OptionsMenuItemInvokedCommand => _optionsMenuItemInvokedCommand ?? (_optionsMenuItemInvokedCommand = new DelegateCommand(OnOptionsMenuItemInvoked));

    public ShellViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
    }

    private void OnLoaded()
    {
        _navigationService = _regionManager.Regions[Regions.Main].NavigationService;
        _navigationService.Navigated += OnNavigated;
        SelectedMenuItem = MenuItems.First();
    }

    private void OnUnloaded()
    {
        _navigationService.Navigated -= OnNavigated;
        _regionManager.Regions.Remove(Regions.Main);
    }

    private bool CanGoBack()
        => _navigationService != null && _navigationService.Journal.CanGoBack;

    private void OnGoBack()
        => _navigationService.Journal.GoBack();

    private void OnMenuItemInvoked()
        => RequestNavigate(SelectedMenuItem.Tag?.ToString());

    private void OnOptionsMenuItemInvoked()
        => RequestNavigate(SelectedOptionsMenuItem.Tag?.ToString());

    private void RequestNavigate(string target)
    {
        if (_navigationService.CanNavigate(target))
        {
            _navigationService.RequestNavigate(target);
        }
    }

    private void OnNavigated(object sender, RegionNavigationEventArgs e)
    {
        var item = MenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => e.Uri.ToString() == i.Tag?.ToString());
        if (item != null)
        {
            SelectedMenuItem = item;
        }
        else
        {
            SelectedOptionsMenuItem = OptionMenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => e.Uri.ToString() == i.Tag?.ToString());
        }

        GoBackCommand.RaiseCanExecuteChanged();
    }
}
