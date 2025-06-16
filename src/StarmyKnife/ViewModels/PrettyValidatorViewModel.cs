using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Events;
using StarmyKnife.Models;
using StarmyKnife.UserControls.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace StarmyKnife.ViewModels;

public class PrettyValidatorViewModel : SinglePluginPageViewModelBase<IPrettyValidator>
{
    private readonly UserSettings _userSettings;
    private string _input;
    private string _output;
    private bool _modeValidateOnlyChecked;
    private bool _modePrettifyChecked;
    private bool _modeMinifyChecked;
    private bool _canPrettify;
    private bool _canMinify;

    public PrettyValidatorViewModel(IPluginLoaderService pluginLoader, IEventAggregator eventAggregator, UserSettings userSettings) : base(pluginLoader, eventAggregator)
    {
        _userSettings = userSettings;
        _input = "";
        _output = "";

        ExecCommand = new DelegateCommand(Exec);

        EventAggregator.GetEvent<UserSettingsChangedEvent>().Subscribe(OnUserSettingsChanged);

        OnSelectedPluginChanged();
    }

    public string Input
    {
        get { return _input; }
        set { SetProperty(ref _input, value); }
    }

    public string Output
    {
        get { return _output; }
        set { SetProperty(ref _output, value); }
    }

    public bool ModeValidateOnlyChecked
    {
        get { return _modeValidateOnlyChecked; }
        set { SetProperty(ref _modeValidateOnlyChecked, value); }
    }

    public bool ModePrettifyChecked
    {
        get { return _modePrettifyChecked; }
        set { SetProperty(ref _modePrettifyChecked, value); }
    }

    public bool ModeMinifyChecked
    {
        get { return _modeMinifyChecked; }
        set { SetProperty(ref _modeMinifyChecked, value); }
    }

    public bool CanPrettify
    {
        get { return _canPrettify; }
        set { SetProperty(ref _canPrettify, value); }
    }

    public bool CanMinify
    {
        get { return _canMinify; }
        set { SetProperty(ref _canMinify, value); }
    }

    public bool ClickOutputToCopy => _userSettings.ClickOutputToCopy;

    public DelegateCommand ExecCommand { get; }

    protected override void OnSelectedPluginChanged()
    {
        ResetRunModeList((IPrettyValidator)SelectedPlugin.Plugin);
    }

    private void Exec()
    {
        if (!ModeValidateOnlyChecked && !ModePrettifyChecked && !ModeMinifyChecked)
        {
            return;
        }

        var isValid = Validate();

        if (!isValid)
        {
            return;
        }

        if (ModePrettifyChecked)
        {
            Prettify();
        }
        else if (ModeMinifyChecked)
        {
            Minify();
        }
    }

    private void ResetRunModeList(IPrettyValidator selectedPrettyValidator)
    {
        CanPrettify = selectedPrettyValidator.CanPrettify;
        CanMinify = selectedPrettyValidator.CanMinify;

        ModeValidateOnlyChecked = true;
        ModePrettifyChecked = false;
        ModeMinifyChecked = false;
    }

    private bool Validate()
    {
        var plugin = (IPrettyValidator)PluginBox.Plugin;
        var validationResult = plugin.Validate(Input, PluginBox.Parameters);

        if (validationResult.Success)
        {
            Output = Properties.Resources.PrettyValidator_SyntaxOK;
        }
        else
        {
            Output = ToBulletList(validationResult.Errors);
        }

        return validationResult.Success;
    }

    private void Prettify()
    {
        var plugin = (IPrettyValidator)PluginBox.Plugin;
        var result = plugin.Prettify(Input, PluginBox.Parameters);

        if (result.Success)
        {
            Output = result.Value;
        }
        else
        {
            Output = ToBulletList(result.Errors);
        }
    }

    private void Minify()
    {
        var plugin = (IPrettyValidator)PluginBox.Plugin;
        var result = plugin.Minify(Input, PluginBox.Parameters);

        if (result.Success)
        {
            Output = result.Value;
        }
        else
        {
            Output = ToBulletList(result.Errors);
        }
    }

    private string ToBulletList(IEnumerable<string> items)
    {
        var result = "";
        foreach (var item in items)
        {
            result += $"{Properties.Resources.Common_BulletMark}{item}{Environment.NewLine}";
        }

        return result;
    }

    private void OnUserSettingsChanged(string propertyName)
    {
        if (propertyName == nameof(UserSettings.ClickOutputToCopy))
        {
            RaisePropertyChanged(nameof(ClickOutputToCopy));
        }
    }
}
