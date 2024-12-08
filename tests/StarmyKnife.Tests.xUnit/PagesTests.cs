using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.Configuration;

using Moq;

using Prism.Regions;

using StarmyKnife.Contracts.Services;
using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Core.Services;
using StarmyKnife.Models;
using StarmyKnife.Services;
using StarmyKnife.Tests.xUnit.Helpers;
using StarmyKnife.ViewModels;

using Unity;
using Unity.Injection;

using Xunit;

namespace StarmyKnife.Tests.XUnit;

public class PagesTests
{
    private readonly IUnityContainer _container;

    public PagesTests()
    {
        _container = TestUtility.GetConfiguredUnityContainer();
    }

    // TODO: Add tests for functionality you add to ChainConverterViewModel.
    [Fact]
    public void TestChainConverterViewModelCreation()
    {
        var vm = _container.Resolve<ChainConverterViewModel>();
        Assert.NotNull(vm);
    }

    // TODO: Add tests for functionality you add to GeneratorViewModel.
    [Fact]
    public void TestGeneratorViewModelCreation()
    {
        var vm = _container.Resolve<GeneratorViewModel>();
        Assert.NotNull(vm);
    }

    // TODO: Add tests for functionality you add to PrettyValidatorViewModel.
    [Fact]
    public void TestPrettyValidatorViewModelCreation()
    {
        var vm = _container.Resolve<PrettyValidatorViewModel>();
        Assert.NotNull(vm);
    }

    // TODO: Add tests for functionality you add to SettingsViewModel.
    [Fact]
    public void TestSettingsViewModelCreation()
    {
        var vm = _container.Resolve<SettingsViewModel>();
        Assert.NotNull(vm);
    }
}
