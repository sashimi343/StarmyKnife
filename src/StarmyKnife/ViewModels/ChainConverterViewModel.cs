using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Events;
using StarmyKnife.Helpers;
using StarmyKnife.Models;
using StarmyKnife.UserControls.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StarmyKnife.ViewModels;

public class ChainConverterViewModel : BindableBase, INotifyDataErrorInfo
{
    private readonly UserSettings _userSettings;
    private readonly IEventAggregator _eventAggregator;
    private readonly ErrorsContainer<string> _errors;
    private readonly IPluginLoaderService _pluginLoader;
    private readonly ObservableCollection<PluginHost> _availablePlugins;
    private readonly ObservableCollection<PluginParameterBoxViewModel> _pluginBoxes;
    private PluginHost _selectedPlugin;
    private string _input;
    private string _output;
    private bool _autoConvertEnabled;

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

    public bool AutoConvertEnabled
    {
        get { return _autoConvertEnabled; }
        set { SetProperty(ref _autoConvertEnabled, value); }
    }

    public bool ClickOutputToCopy => _userSettings.ClickOutputToCopy;

    public ObservableCollection<PluginHost> AvailablePlugins
    {
        get { return _availablePlugins; }
    }

    public ObservableCollection<PluginParameterBoxViewModel> PluginBoxes
    {
        get { return _pluginBoxes; }
    }

    public PluginHost SelectedPlugin
    {
        get { return _selectedPlugin; }
        set { SetProperty(ref _selectedPlugin, value); }
    }

    public DelegateCommand<PluginParameterBoxViewModel> MoveUpPluginBoxCommand { get; }
    public DelegateCommand<PluginParameterBoxViewModel> MoveDownPluginBoxCommand { get; }
    public DelegateCommand<PluginParameterBoxViewModel> DeletePluginBoxCommand { get; }
    public DelegateCommand AddPluginCommand { get; }
    public DelegateCommand ConvertCommand { get; }
    public DelegateCommand CheckAutoConvertCommand { get; }
    public DelegateCommand ClearInputCommand { get; }
    public DelegateCommand ResetPluginBoxCommand { get; }

    public bool HasErrors
    {
        get { return _errors.HasErrors; }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public ChainConverterViewModel(UserSettings userSettings, IPluginLoaderService pluginLoader, IEventAggregator eventAggregator)
    {
        _userSettings = userSettings;
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<PluginParameterBoxChangedEvent>().Subscribe(CheckAutoConvert);
        _eventAggregator.GetEvent<UserSettingsChangedEvent>().Subscribe(OnUserSettingsChanged);
        _errors = new ErrorsContainer<string>(OnErrorsChanged);
        _pluginLoader = pluginLoader;

        _availablePlugins = new ObservableCollection<PluginHost>();
        LoadAvailablePlugins();
        _pluginBoxes = new ObservableCollection<PluginParameterBoxViewModel>();

        MoveUpPluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(MoveUpPluginBox);
        MoveDownPluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(MoveDownPluginBox);
        DeletePluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(DeletePluginBox);
        AddPluginCommand = new DelegateCommand(AddPluginBox);
        ConvertCommand = new DelegateCommand(Convert);
        CheckAutoConvertCommand = new DelegateCommand(CheckAutoConvert);
        ClearInputCommand = new DelegateCommand(ClearInput);
        ResetPluginBoxCommand = new DelegateCommand(ResetPluginBox);

        AutoConvertEnabled = _userSettings.EnableAutoConvertByDefault;
    }

    public IEnumerable GetErrors(string propertyName)
    {
        return _errors.GetErrors(propertyName);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void AddPluginBox()
    {
        if (SelectedPlugin != null)
        {
            var newPluginBox = new PluginParameterBoxViewModel(SelectedPlugin, _eventAggregator)
            {
                IsDeletable = true,
                IsMovable = true
            };
            PluginBoxes.Add(newPluginBox);
            UpdatePluginBoxMovability();
            CheckAutoConvert();

            SelectedPlugin = null;
        }
    }

    private void MoveUpPluginBox(PluginParameterBoxViewModel box)
    {
        int index = PluginBoxes.IndexOf(box);
        if (index > 0)
        {
            PluginBoxes.Move(index, index - 1);
            UpdatePluginBoxMovability();
            CheckAutoConvert();
        }
    }

    private void MoveDownPluginBox(PluginParameterBoxViewModel box)
    {
        int index = PluginBoxes.IndexOf(box);
        if (index < PluginBoxes.Count - 1)
        {
            PluginBoxes.Move(index, index + 1);
            UpdatePluginBoxMovability();
            CheckAutoConvert();
        }
    }

    private void DeletePluginBox(PluginParameterBoxViewModel box)
    {
        PluginBoxes.Remove(box);
        UpdatePluginBoxMovability();
        CheckAutoConvert();
    }

    private void Convert()
    {
        if (string.IsNullOrEmpty(Input))
        {
            Output = "";
            _errors.ClearErrors(nameof(Input));
            return;
        }

        try
        {
            var tmpOutput = Input;

            foreach (PluginParameterBoxViewModel box in PluginBoxes)
            {
                var plugin = (IConverter)box.Plugin;
                var parameter = box.Parameters;
                var conversionResult = plugin.Convert(tmpOutput, parameter);

                if (!conversionResult.Success)
                {
                    _errors.SetErrorsIfChanged(nameof(Input), conversionResult.Errors);
                    Output = "";
                    return;
                }

                tmpOutput = conversionResult.Value;
            }

            Output = tmpOutput;
            _errors.ClearErrors(nameof(Input));
        }
        catch (Exception ex)
        {
            _errors.SetErrorsFromException(nameof(Input), ex);
            Output = "";
        }

    }

    private void CheckAutoConvert()
    {
        if (AutoConvertEnabled)
        {
            Convert();
        }
    }

    private void ClearInput()
    {
        Input = "";
        _errors.ClearErrors(nameof(Input));
    }

    private void ResetPluginBox()
    {
        PluginBoxes.Clear();
        CheckAutoConvert();
    }

    private void OnUserSettingsChanged(string propertyName)
    {
        if (propertyName == nameof(_userSettings.ClickOutputToCopy))
        {
            RaisePropertyChanged(nameof(ClickOutputToCopy));
        }

        if (propertyName == nameof(_userSettings.UsePrettyValidatorAsConverter))
        {
            LoadAvailablePlugins();
        }
    }

    private void UpdatePluginBoxMovability()
    {
        for (int i = 0; i < PluginBoxes.Count; i++)
        {
            var box = PluginBoxes[i];
            box.CanMoveUp = i > 0;
            box.CanMoveDown = i < PluginBoxes.Count - 1;
        }
    }

    private void LoadAvailablePlugins()
    {
        _pluginLoader.UsePrettyValidatorAsConverter = _userSettings.UsePrettyValidatorAsConverter;
        AvailablePlugins.Clear();
        AvailablePlugins.AddRange(_pluginLoader.GetPlugins<IConverter>());
    }
}
