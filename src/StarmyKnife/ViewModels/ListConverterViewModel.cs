using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

using Prism.Mvvm;

using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Models;
using StarmyKnife.PluginInterfaces;
using StarmyKnife.Events;
using StarmyKnife.Helpers;
using StarmyKnife.Models;
using StarmyKnife.UserControls.ViewModels;
using System.Windows.Media;

namespace StarmyKnife.ViewModels;

public class ListConverterViewModel : BindableBase, INotifyDataErrorInfo
{
    private static readonly string[] ClipboardTextDelimiters = new[] { "\r\n", "\r", "\n" };
    private const int ErrorCountLimit = 3;

    private readonly IEventAggregator _eventAggregator;
    private readonly ErrorsContainer<string> _errors;
    private readonly IPluginLoaderService _pluginLoader;
    private readonly UserSettings _userSettings;
    private readonly ObservableCollection<PluginHost> _availablePlugins;
    private readonly ObservableCollection<PluginParameterBoxViewModel> _pluginBoxes;
    private PluginHost _selectedPlugin;
    private readonly ObservableCollection<ListConverterItem> _items;

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

        _items = new ObservableCollection<ListConverterItem>();

        MoveUpPluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(MoveUpPluginBox);
        MoveDownPluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(MoveDownPluginBox);
        DeletePluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(DeletePluginBox);
        AddPluginCommand = new DelegateCommand(AddPluginBox);
        ResetPluginBoxCommand = new DelegateCommand(ResetPluginBox);
        SetInputFromClipboardCommand = new DelegateCommand(SetInputFromClipboard);
        CopyOutputToClipboardCommand = new DelegateCommand(CopyOutputToClipboard);
        ConvertAllCommand = new DelegateCommand(ConvertAll);
        ClearInputCommand = new DelegateCommand(ClearItems);
    }

    public FontFamily IOFontFamily => _userSettings.IOFontFamily;
    public int IOFontSize => _userSettings.IOFontSize;

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

    public ObservableCollection<ListConverterItem> Items
    {
        get
        {
            return _items;
        }
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
        ClearItems();
        var clipboardText = Clipboard.GetText();

        if (string.IsNullOrEmpty(clipboardText))
        {
            return;
        }

        var clipboardLines = clipboardText.Split(ClipboardTextDelimiters, StringSplitOptions.None);

        foreach (var line in clipboardLines)
        {
            Items.Add(new ListConverterItem(line));
        }
    }

    private void CopyOutputToClipboard()
    {
        var sb = new StringBuilder();
        foreach (var item in Items)
        {
            sb.AppendLine(item.Output);
        }

        Clipboard.SetText(sb.ToString());
    }

    private void ConvertAll()
    {
        var errorsCount = 0;
        for (var i = 0; i < Items.Count; i++)
        {
            var conversionSuccess = Convert(Items[i]);
            if (!conversionSuccess)
            {
                MessageBox.Show(BuildConversionErrorMessage(i), "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                errorsCount++;
                if (errorsCount > ErrorCountLimit)
                {
                    MessageBox.Show("Too many errors occurred. Stopping further conversions.", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                }
            }
        }
    }

    private bool Convert(ListConverterItem item)
    {
        try
        {
            var tmpOutput = item.Input;

            foreach (PluginParameterBoxViewModel box in PluginBoxes)
            {
                var plugin = (IConverter)box.Plugin;
                var parameter = box.Parameters;
                var conversionResult = plugin.Convert(tmpOutput, parameter);

                if (!conversionResult.Success)
                {
                    _errors.SetErrorsIfChanged(nameof(Items), conversionResult.Errors);
                    item.Output = "";
                    return false;
                }

                tmpOutput = conversionResult.Value;
            }

            item.Output = tmpOutput;
            _errors.ClearErrors(nameof(Items));
        }
        catch (Exception ex)
        {
            _errors.SetErrorsFromException(nameof(Items), ex);
            item.Output = "";
            return false;
        }

        return true;
    }

    private string BuildConversionErrorMessage(int index)
    {
        var sb = new StringBuilder();
        sb.AppendFormat($"Error while converting {0}-th input:", index + 1);
        sb.AppendLine("");
        var errors = _errors.GetErrors(nameof(Items));
        foreach (var error in errors)
        {
            sb.AppendLine(error);
        }

        return sb.ToString();
    }

    private void ClearItems()
    {
        Items.Clear();
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
        switch (propertyName)
        {
            case nameof(_userSettings.UsePrettyValidatorAsConverter):
                LoadAvailablePlugins();
                break;
            case nameof(_userSettings.IOFontFamily):
                RaisePropertyChanged(nameof(IOFontFamily));
                break;
            case nameof(_userSettings.IOFontSize):
                RaisePropertyChanged(nameof(IOFontSize));
                break;
        }
    }

    private void LoadAvailablePlugins()
    {
        _pluginLoader.UsePrettyValidatorAsConverter = _userSettings.UsePrettyValidatorAsConverter;
        AvailablePlugins.Clear();
        AvailablePlugins.AddRange(_pluginLoader.GetPlugins<IConverter>());
    }
}
