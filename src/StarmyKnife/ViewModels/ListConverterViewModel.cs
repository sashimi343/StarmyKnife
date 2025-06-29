using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

using Prism.Mvvm;

using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Events;
using StarmyKnife.Helpers;
using StarmyKnife.Models;
using StarmyKnife.UserControls.ViewModels;

namespace StarmyKnife.ViewModels;

public class ListConverterViewModel : BindableBase, INotifyDataErrorInfo
{
    private static readonly string[] ClipboardTextDelimiters = new[] { "\r\n", "\r", "\n" };

    private readonly IEventAggregator _eventAggregator;
    private readonly ErrorsContainer<string> _errors;
    private readonly IPluginLoaderService _pluginLoader;
    private readonly UserSettings _userSettings;
    private readonly ObservableCollection<PluginHost> _availablePlugins;
    private readonly ObservableCollection<PluginParameterBoxViewModel> _pluginBoxes;
    private PluginHost _selectedPlugin;
    private ObservableCollection<StringItem> _inputItems;
    private ObservableCollection<StringItem> _outputItems;

    public ListConverterViewModel(IPluginLoaderService pluginLoader, IEventAggregator eventAggregator, UserSettings userSettings)
    {
        _eventAggregator = eventAggregator;
        _errors = new ErrorsContainer<string>(OnErrorsChanged);
        _pluginLoader = pluginLoader;
        _userSettings = userSettings;
        _eventAggregator.GetEvent<UserSettingsChangedEvent>().Subscribe(OnUserSettingsChanged);

        _availablePlugins = new ObservableCollection<PluginHost>();
        LoadAvailablePlugins();
        _pluginBoxes = new ObservableCollection<PluginParameterBoxViewModel>();

        _inputItems = new ObservableCollection<StringItem>();
        _outputItems = new ObservableCollection<StringItem>();

        MoveUpPluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(MoveUpPluginBox);
        MoveDownPluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(MoveDownPluginBox);
        DeletePluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(DeletePluginBox);
        AddPluginCommand = new DelegateCommand(AddPluginBox);
        ResetPluginBoxCommand = new DelegateCommand(ResetPluginBox);
        SetInputFromClipboardCommand = new DelegateCommand(SetInputFromClipboard);
        CopyOutputToClipboardCommand = new DelegateCommand(CopyOutputToClipboard);
        ConvertAllCommand = new DelegateCommand(ConvertAll);
        ClearInputCommand = new DelegateCommand(ClearInput);
    }

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

    public ObservableCollection<StringItem> InputItems
    {
        get { return _inputItems; }
        set { SetProperty(ref _inputItems, value); }
    }

    public ObservableCollection<StringItem> OutputItems
    {
        get { return _outputItems; }
        set { SetProperty(ref _outputItems, value); }
    }

    public DelegateCommand<PluginParameterBoxViewModel> MoveUpPluginBoxCommand { get; }
    public DelegateCommand<PluginParameterBoxViewModel> MoveDownPluginBoxCommand { get; }
    public DelegateCommand<PluginParameterBoxViewModel> DeletePluginBoxCommand { get; }
    public DelegateCommand AddPluginCommand { get; }
    public DelegateCommand ResetPluginBoxCommand { get; }
    public DelegateCommand SetInputFromClipboardCommand { get; }
    public DelegateCommand CopyOutputToClipboardCommand { get; }
    public DelegateCommand ConvertAllCommand { get; }
    public DelegateCommand ClearInputCommand { get; }

    public bool HasErrors
    {
        get { return _errors.HasErrors; }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

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
                IsMovable = true,
            };
            PluginBoxes.Add(newPluginBox);
            UpdatePluginBoxMovability();

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
        }
    }

    private void MoveDownPluginBox(PluginParameterBoxViewModel box)
    {
        int index = PluginBoxes.IndexOf(box);
        if (index < PluginBoxes.Count - 1)
        {
            PluginBoxes.Move(index, index + 1);
            UpdatePluginBoxMovability();
        }
    }

    private void DeletePluginBox(PluginParameterBoxViewModel box)
    {
        PluginBoxes.Remove(box);
    }

    private void ResetPluginBox()
    {
        PluginBoxes.Clear();
    }

    private void SetInputFromClipboard()
    {
        ClearInput();
        var clipboardText = Clipboard.GetText();

        if (string.IsNullOrEmpty(clipboardText))
        {
            return;
        }

        var clipboardLines = clipboardText.Split(ClipboardTextDelimiters, StringSplitOptions.None);

        foreach (var line in clipboardLines)
        {
            InputItems.Add(line);
        }
    }

    private void CopyOutputToClipboard()
    {
        var sb = new StringBuilder();
        foreach (var output in OutputItems)
        {
            sb.AppendLine(output.Value);
        }

        Clipboard.SetText(sb.ToString());
    }

    private void ConvertAll()
    {
        PrepareOutputItems();
        for (var i = 0; i < OutputItems.Count; i++)
        {
            var conversionSuccess = Convert(i);
            if (!conversionSuccess)
            {
                MessageBox.Show(BuildConversionErrorMessage(i), "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                break;
            }
        }
    }

    private void PrepareOutputItems()
    {
        OutputItems.Clear();
        for (int i = 0; i < InputItems.Count; i++)
        {
            OutputItems.Add("");
        }
    }

    private bool Convert(int index)
    {
        var input = InputItems[index].Value;

        if (string.IsNullOrEmpty(input))
        {
            OutputItems[index] = "";
            _errors.ClearErrors(nameof(InputItems));
            return true;
        }

        try
        {
            var tmpOutput = input;

            foreach (PluginParameterBoxViewModel box in PluginBoxes)
            {
                var plugin = (IConverter)box.Plugin;
                var parameter = box.Parameters;
                var conversionResult = plugin.Convert(tmpOutput, parameter);

                if (!conversionResult.Success)
                {
                    _errors.SetErrorsIfChanged(nameof(InputItems), conversionResult.Errors);
                    OutputItems[index] = "";
                    return false;
                }

                tmpOutput = conversionResult.Value;
            }

            OutputItems[index] = tmpOutput;
            _errors.ClearErrors(nameof(input));
        }
        catch (Exception ex)
        {
            _errors.SetErrorsFromException(nameof(InputItems), ex);
            OutputItems[index] = "";
            return false;
        }

        return true;
    }

    private string BuildConversionErrorMessage(int index)
    {
        var sb = new StringBuilder();
        sb.AppendFormat($"Error while converting {0}-th input:", index + 1);
        sb.AppendLine("");
        var errors = _errors.GetErrors(nameof(InputItems));
        foreach (var error in errors)
        {
            sb.AppendLine(error);
        }

        return sb.ToString();
    }

    private void ClearInput()
    {
        InputItems.Clear();
        _errors.ClearErrors();
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

    private void OnUserSettingsChanged(string propertyName)
    {
        if (propertyName == nameof(_userSettings.UsePrettyValidatorAsConverter))
        {
            LoadAvailablePlugins();
        }
    }

    private void LoadAvailablePlugins()
    {
        _pluginLoader.UsePrettyValidatorAsConverter = _userSettings.UsePrettyValidatorAsConverter;
        AvailablePlugins.Clear();
        AvailablePlugins.AddRange(_pluginLoader.GetPlugins<IConverter>());
    }
}
