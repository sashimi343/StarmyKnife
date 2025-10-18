using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

using Prism.Mvvm;

using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Events;
using StarmyKnife.Models;
using StarmyKnife.PluginInterfaces;

namespace StarmyKnife.ViewModels;

public class CounterViewModel : BindableBase
{
    private static readonly Regex PatternSplitWords = new(@"\W+", RegexOptions.Compiled);
    private static readonly Regex PatternSplitLines = new(@"\r\n|\r|\n", RegexOptions.Compiled);

    private readonly UserSettings _userSettings;
    private readonly IEventAggregator _eventAggregator;
    public FontFamily IOFontFamily => _userSettings.IOFontFamily;
    public int IOFontSize => _userSettings.IOFontSize;

    public CounterViewModel(UserSettings userSettings, IEventAggregator eventAggregator)
    {
        _userSettings = userSettings;
        _eventAggregator = eventAggregator;

        _eventAggregator.GetEvent<UserSettingsChangedEvent>().Subscribe(OnUserSettingsChanged);

        Input = "";
        AvailableEncodings = [
            PluginEncoding.UTF8,
            PluginEncoding.UTF16BE,
            PluginEncoding.UTF16LE,
            PluginEncoding.UTF32,
            PluginEncoding.ShiftJIS,
            PluginEncoding.EUCJP,
            PluginEncoding.ISO2022JP,
        ];
        SelectedEncoding = PluginEncoding.UTF8;
    }

    private string _input;
    public string Input
    {
        get { return _input; }
        set {
            SetProperty(ref _input, value);
            RaisePropertyChanged(nameof(ByteCount));
            RaisePropertyChanged(nameof(CharCount));
            RaisePropertyChanged(nameof(TextElementCount));
            RaisePropertyChanged(nameof(WordCount));
            RaisePropertyChanged(nameof(LineCount));
        }
    }

    public List<PluginEncoding> AvailableEncodings { get; }

    private PluginEncoding _selectedEncoding;
    public PluginEncoding SelectedEncoding
    {
        get { return _selectedEncoding; }
        set {
            SetProperty(ref _selectedEncoding, value);
            RaisePropertyChanged(nameof(ByteCount));
        }
    }

    private string _byteCountEncodingError;
    public string ByteCountErrorMessage
    {
        get { return _byteCountEncodingError; }
        set {
            SetProperty(ref _byteCountEncodingError, value);
            RaisePropertyChanged(nameof(ByteCountErrorMessageVisibility));
        }
    }

    public Visibility ByteCountErrorMessageVisibility => string.IsNullOrEmpty(ByteCountErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    public long ByteCount
    {
        get
        {
            long count = -1L;
            try
            {
                count = (_selectedEncoding?.Encoding ?? Encoding.Default).GetByteCount(Input);
                ByteCountErrorMessage = null;
            }
            catch (EncoderFallbackException)
            {
                ByteCountErrorMessage = Properties.Resources.Counter_InvalidEncodingErrorMessage;
            }
            finally
            {
                RaisePropertyChanged(nameof(ByteCountErrorMessage));
            }
            return count;
        }
    }

    public long CharCount
    {
        get
        {
            return Input.Length;
        }
    }

    public long TextElementCount
    {
        get
        {
            return new StringInfo(Input).LengthInTextElements;
        }
    }

    public long WordCount
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                return 0;
            }
            var wordCount = PatternSplitWords.Split(Input).Where(w => !string.IsNullOrEmpty(w)).Count();
            return wordCount;
        }
    }

    public long LineCount
    {
        get
        {
            if (string.IsNullOrEmpty(Input))
            {
                return 0;
            }
            var lines = PatternSplitLines.Split(Input);
            return lines.Length;
        }
    }

    private void OnUserSettingsChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_userSettings.IOFontFamily):
                RaisePropertyChanged(nameof(IOFontFamily));
                break;
            case nameof(_userSettings.IOFontSize):
                RaisePropertyChanged(nameof(IOFontSize));
                break;
            default:
                break;
        }
    }
}
