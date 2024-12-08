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
        RegisterExternalPlugins();

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
        containerRegistry.RegisterForNavigation<ListConverterPage, ListConverterViewModel>(PageKeys.ListConverter);
        containerRegistry.RegisterForNavigation<XPathFinderPage, XPathFinderViewModel>(PageKeys.XPathFinder);
        containerRegistry.RegisterForNavigation<CsqlPage, CsqlViewModel>(PageKeys.Csql);
        containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>(PageKeys.Settings);
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
        var appLocation = AppDomain.CurrentDomain.BaseDirectory;
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
        var crashLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"crashlog_{DateTime.Now:yyyyMMddHHmmssfff}.log");

        if (WriteCrashLog(crashLogPath, e.Exception))
        {
            MessageBox.Show($"An unhandled exception occurred. The crash log has been saved to {crashLogPath}.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show("An unhandled exception occurred: " + e.Exception.ToString(), "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string[] GetFilesInPluginDirectory()
    {
        var appConfig = Container.Resolve<AppConfig>();
        var pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appConfig.PluginsDirectory);

        if (!Directory.Exists(pluginsDir))
        {
            return [];
        }

        return Directory.GetFiles(pluginsDir, "*.dll", SearchOption.TopDirectoryOnly);
    }

    private void RegisterExternalPlugins()
    {
        try
        {
            var pluginLoaderService = Container.Resolve<IPluginLoaderService>();
            var pluginDllFiles = GetFilesInPluginDirectory();

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedAssembyName = new AssemblyName(args.Name);
                var coreAssembly = typeof(IPlugin).Assembly;
                if (requestedAssembyName.Name == coreAssembly.GetName().Name)
                {
                    return coreAssembly;
                }

                return null;
            };

            foreach (var pluginDllFile in pluginDllFiles)
            {
                var assembly = Assembly.LoadFrom(pluginDllFile);
                var types = assembly.GetTypes();

                if (types.Any(t => t.IsSubclassOf(typeof(PluginBase))))
                {
                    pluginLoaderService.LoadPlugins(assembly);
                }
            }
        }
        catch (Exception)
        {
            // Ignore exceptions
        }
    }

    private bool WriteCrashLog(string crashLogPath, Exception exception)
    {
        var success = false;

        try
        {
            var sb = new StringBuilder();
            var appInfoService = Container.Resolve<IApplicationInfoService>();
            var filesInPluginsDir = GetFilesInPluginDirectory();

            sb.AppendLine("Basic Info ===============================================================");
            sb.AppendFormat("Timestamp: {0}{1}", DateTime.Now, Environment.NewLine);
            sb.AppendFormat("App version: {0}{1}", appInfoService.GetVersion(), Environment.NewLine);
            sb.AppendFormat("OS version: {0}{1}", Environment.OSVersion.VersionString, Environment.NewLine);
            sb.AppendLine("Plugins ==================================================================");
            foreach (var file in filesInPluginsDir)
            {
                sb.AppendLine(Path.GetFileName(file));
            }
            sb.AppendLine("Stack trace ==============================================================");
            sb.AppendLine(exception.ToString());

            File.WriteAllText(crashLogPath, sb.ToString());

            success = true;
        }
        catch
        {
            success = false;
        }

        return success;
    }
}
