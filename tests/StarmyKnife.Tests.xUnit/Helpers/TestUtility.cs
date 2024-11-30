using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using StarmyKnife.Contracts.Services;
using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Core.Services;
using StarmyKnife.Models;
using StarmyKnife.Services;

namespace StarmyKnife.Tests.xUnit.Helpers
{
    internal static class TestUtility
    {
        public static IUnityContainer GetConfiguredUnityContainer()
        {
            var container = new UnityContainer();
            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof(IPlugin).Assembly));
            var compositionContainer = new CompositionContainer(catalog);

            // Debug
            container.AddExtension(new Diagnostic());

            container.RegisterType<IRegionManager, RegionManager>();
            container.RegisterType<IEventAggregator, EventAggregator>();

            // Enable Shift-JIS and other code pages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Core Services
            container.RegisterType<IFileService, FileService>();
            container.RegisterSingleton<IPluginLoaderService, PluginLoaderService>();
            container.RegisterFactory(typeof(PluginLoaderService), nameof(PluginLoaderService), (c, t, n) =>
            {
                var service = new PluginLoaderService();
                compositionContainer.ComposeParts(service);
                return service;
            });

            // App Services
            container.RegisterType<IThemeSelectorService, ThemeSelectorService>();
            container.RegisterType<ISystemService, SystemService>();
            container.RegisterType<IPersistAndRestoreService, PersistAndRestoreService>();
            container.RegisterType<IApplicationInfoService, ApplicationInfoService>();
            container.RegisterType<IAppPropertiesWrapper, AppPropertiesWrapper>();

            // Configuration
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddJsonFile("appsettings.json")
                .Build();
            var appConfig = configuration
                .GetSection(nameof(AppConfig))
                .Get<AppConfig>();

            // Register configurations to IoC
            container.RegisterInstance(configuration);
            container.RegisterInstance(appConfig);
            container.RegisterType<UserSettings>();

            container.RegisterInstance<IAppPropertiesWrapper>(new MockAppPropertiesWrapper());

            return container;
        }
    }
}
