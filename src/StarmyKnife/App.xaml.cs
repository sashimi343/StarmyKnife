using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.Configuration;

using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;

using StarmyKnife.Constants;
using StarmyKnife.Contracts.Services;
using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Core.Services;
using StarmyKnife.Models;
using StarmyKnife.Services;
using StarmyKnife.ViewModels;
using StarmyKnife.Views;

using Unity;

namespace StarmyKnife;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview
// For docs about using Prism in WPF see https://prismlibrary.com/docs/wpf/introduction.html

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : PrismApplication
{
    private string[] _startUpArgs;

    public App()
    {
    }

    protected override Window CreateShell()
        => Container.Resolve<ShellWindow>();

    protected override async void OnInitialized()
    {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.RestoreData();

        var themeSelectorService = Container.Resolve<IThemeSelectorService>();
        themeSelectorService.InitializeTheme();

        // Enable Shift-JIS and other code pages
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Register external plugins
        var appConfig = Container.Resolve<AppConfig>();
        RegisterExternalPlugins(appConfig.PluginsDirectory);

        base.OnInitialized();
        await Task.CompletedTask;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _startUpArgs = e.Args;
        base.OnStartup(e);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Core Services
        containerRegistry.Register<IFileService, FileService>();
        containerRegistry.RegisterSingleton<IPluginLoaderService, PluginLoaderService>();

        // App Services
        containerRegistry.Register<IApplicationInfoService, ApplicationInfoService>();
        containerRegistry.Register<ISystemService, SystemService>();
        containerRegistry.Register<IPersistAndRestoreService, PersistAndRestoreService>();
        containerRegistry.Register<IThemeSelectorService, ThemeSelectorService>();
        containerRegistry.Register<IAppPropertiesWrapper, AppPropertiesWrapper>();

        // Views
        containerRegistry.RegisterForNavigation<XPathFinderPage, XPathFinderViewModel>(PageKeys.XPathFinder);
        containerRegistry.RegisterForNavigation<CsqlPage, CsqlViewModel>(PageKeys.Csql);
        containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>(PageKeys.Settings);
        containerRegistry.RegisterForNavigation<ExceditPage, ExceditViewModel>(PageKeys.Excedit);
        containerRegistry.RegisterForNavigation<PrettyValidatorPage, PrettyValidatorViewModel>(PageKeys.PrettyValidator);
        containerRegistry.RegisterForNavigation<GeneratorPage, GeneratorViewModel>(PageKeys.Generator);
        containerRegistry.RegisterForNavigation<ChainConverterPage, ChainConverterViewModel>(PageKeys.ChainConverter);
        containerRegistry.RegisterForNavigation<ShellWindow, ShellViewModel>();

        // Configuration
        var configuration = BuildConfiguration();
        var appConfig = configuration
            .GetSection(nameof(AppConfig))
            .Get<AppConfig>();

        // Register configurations to IoC
        containerRegistry.RegisterInstance<IConfiguration>(configuration);
        containerRegistry.RegisterInstance<AppConfig>(appConfig);
        containerRegistry.Register<UserSettings>();
    }

    private IConfiguration BuildConfiguration()
    {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        return new ConfigurationBuilder()
            .SetBasePath(appLocation)
            .AddJsonFile("appsettings.json")
            .AddCommandLine(_startUpArgs)
            .Build();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.PersistData();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }

    private void RegisterExternalPlugins(string pluginsDir)
    {
        var pluginLoaderService = Container.Resolve<IPluginLoaderService>();

        if (!Directory.Exists(pluginsDir))
        {
            return;
        }

        var pluginDllFiles = Directory.GetFiles(pluginsDir, "*.dll", SearchOption.TopDirectoryOnly);

        foreach (var pluginDllFile in pluginDllFiles)
        {
            var assembly = Assembly.LoadFrom(pluginDllFile);
            var types = assembly.GetTypes();

            if (types.Any(t => t is IPlugin))
            {
                pluginLoaderService.LoadPlugins(assembly);
            }
        }
    }
}
