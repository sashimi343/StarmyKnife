﻿using System.Windows.Input;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using StarmyKnife.Contracts.Services;
using StarmyKnife.Models;

namespace StarmyKnife.ViewModels;

// TODO: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
public class SettingsViewModel : BindableBase, INavigationAware
{
    private readonly AppConfig _appConfig;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ISystemService _systemService;
    private readonly IApplicationInfoService _applicationInfoService;

    private readonly UserSettings _userSettings;
    private AppTheme _theme;
    private string _versionDescription;

    private ICommand _setThemeCommand;
    private ICommand _privacyStatementCommand;

    public AppTheme Theme
    {
        get { return _theme; }
        set { SetProperty(ref _theme, value); }
    }

    public string VersionDescription
    {
        get { return _versionDescription; }
        set { SetProperty(ref _versionDescription, value); }
    }

    public bool EnableAutoConvertByDefault
    {
        get { return _userSettings.EnableAutoConvertByDefault; }
        set
        {
            _userSettings.EnableAutoConvertByDefault = value;
            RaisePropertyChanged();
        }
    }

    public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new DelegateCommand<string>(OnSetTheme));

    public ICommand PrivacyStatementCommand => _privacyStatementCommand ?? (_privacyStatementCommand = new DelegateCommand(OnPrivacyStatement));

    public SettingsViewModel(AppConfig appConfig, UserSettings userSettings, IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService)
    {
        _appConfig = appConfig;
        _userSettings = userSettings;
        _themeSelectorService = themeSelectorService;
        _systemService = systemService;
        _applicationInfoService = applicationInfoService;
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
        Theme = _themeSelectorService.GetCurrentTheme();
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    private void OnSetTheme(string themeName)
    {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        _themeSelectorService.SetTheme(theme);
    }

    private void OnPrivacyStatement()
        => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

    public bool IsNavigationTarget(NavigationContext navigationContext)
        => true;
}
