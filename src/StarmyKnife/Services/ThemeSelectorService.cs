using System.Windows;

using ControlzEx.Theming;

using MahApps.Metro.Theming;

using StarmyKnife.Contracts.Services;
using StarmyKnife.Models;

namespace StarmyKnife.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    private const string HcDarkTheme = "pack://application:,,,/Styles/Themes/HC.Dark.Blue.xaml";
    private const string HcLightTheme = "pack://application:,,,/Styles/Themes/HC.Light.Blue.xaml";

    private const string SystemThemeRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string SystemThemeRegistryName = "AppsUseLightTheme";

    private static readonly Uri CustomDarkThemeUri = new("Styles/Themes/CustomTheme.Dark.xaml", UriKind.Relative);
    private static readonly Uri CustomLightThemeUri = new("Styles/Themes/CustomTheme.Light.xaml", UriKind.Relative);

    public ThemeSelectorService()
    {
    }

    public void InitializeTheme()
    {
        // TODO: Mahapps.Metro supports syncronization with high contrast but you have to provide custom high contrast themes
        // We've added basic high contrast dictionaries for Dark and Light themes
        // Please complete these themes following the docs on https://mahapps.com/docs/themes/thememanager#creating-custom-themes
        ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcDarkTheme), MahAppsLibraryThemeProvider.DefaultInstance));
        ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcLightTheme), MahAppsLibraryThemeProvider.DefaultInstance));

        var theme = GetCurrentTheme();
        SetTheme(theme);
    }

    public void SetTheme(AppTheme theme)
    {
        if (theme == AppTheme.Default)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
        }
        else
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
            ThemeManager.Current.SyncTheme();
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.Blue", SystemParameters.HighContrast);
        }

        ReplaceCustomThemeDictionary(theme);

        App.Current.Properties["Theme"] = theme.ToString();
    }

    public AppTheme GetCurrentTheme()
    {
        if (App.Current.Properties.Contains("Theme"))
        {
            var themeName = App.Current.Properties["Theme"].ToString();
            Enum.TryParse(themeName, out AppTheme theme);
            return theme;
        }

        return AppTheme.Default;
    }

    private void ReplaceCustomThemeDictionary(AppTheme theme)
    {
        if (theme == AppTheme.Default)
        {
            theme = GetSystemTheme();
        }

        var appResources = Application.Current.Resources.MergedDictionaries;

        var current = appResources
            .FirstOrDefault(x => x.Source != null
                              && x.Source.OriginalString.StartsWith("Themes/CustomTheme"));
        if (current != null)
        {
            appResources.Remove(current);
        }

        var newThemeUri = (theme == AppTheme.Dark)
            ? CustomDarkThemeUri
            : CustomLightThemeUri;

        appResources.Add(new ResourceDictionary { Source = newThemeUri });
    }

    private AppTheme GetSystemTheme()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SystemThemeRegistryKey);
            if (key?.GetValue(SystemThemeRegistryName) is int registryValue)
            {
                return registryValue == 1 ? AppTheme.Light : AppTheme.Dark;
            }
        }
        catch
        {
        }
        return AppTheme.Light;
    }
}
