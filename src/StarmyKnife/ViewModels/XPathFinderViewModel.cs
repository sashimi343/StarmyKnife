using MaterialDesignThemes.Wpf;

using Prism.Commands;
using Prism.Mvvm;

using StarmyKnife.Core.Contracts.Models;
using StarmyKnife.Core.Models;
using StarmyKnife.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace StarmyKnife.ViewModels;

public class XPathFinderViewModel : BindableBase
{
    private const int PopupDisplaySeconds = 2;

    private PathType _selectedPathType;
    private string _inputXml;
    private string _selectedXPath;
    private ObservableCollection<string> _xPaths;
    private ObservableCollection<string> _searchResults;

    private readonly XPathSearchTextHistories _xPathSearchTextHistories;
    private readonly JsonPathSearchTextHistories _jsonPathSearchTextHistories;

    public XPathFinderViewModel(UserSettings userSettings)
    {
        _xPathSearchTextHistories = new XPathSearchTextHistories(userSettings);
        _jsonPathSearchTextHistories = new JsonPathSearchTextHistories(userSettings);

        XPaths = new ObservableCollection<string>();
        SelectedPathType = PathType.XPath;
        InputXml = string.Empty;
        SearchResults = new ObservableCollection<string>();

        SearchCommand = new DelegateCommand(Search);
    }

    public PathType SelectedPathType
    {
        get => _selectedPathType;
        set
        {
            SetProperty(ref _selectedPathType, value);
            RaisePropertyChanged(nameof(InputTypeName));
            ReloadSearchHistories();
        }
    }

    public string InputTypeName => SelectedPathType switch
    {
        PathType.XPath => "XML",
        PathType.JSONPath => "JSON",
        _ => throw new NotImplementedException(),
    };

    public string InputXml
    {
        get => _inputXml;
        set => SetProperty(ref _inputXml, value);
    }

    public string SelectedXPath
    {
        get => _selectedXPath;
        set => SetProperty(ref _selectedXPath, value);
    }

    public ObservableCollection<string> XPaths
    {
        get => _xPaths;
        set => SetProperty(ref _xPaths, value);
    }

    public ObservableCollection<string> SearchResults
    {
        get => _searchResults;
        set => SetProperty(ref _searchResults, value);
    }

    public DelegateCommand SearchCommand { get; }
    public DelegateCommand CopyToClipboardCommand { get; }

    private void Search()
    {
        var searcher = GetPathSearcher();
        if (!searcher.TryLoadInput(InputXml, out var error))
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            SearchResults.Clear();
            return;
        }

        try
        {
            var results = searcher.FindAllNodes(SelectedXPath);

            SearchResults.Clear();
            foreach (var result in results)
            {
                SearchResults.Add(result);
            }
            SaveSearchHistory();
            ReloadSearchHistories();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error while searching data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            SearchResults.Clear();
        }
    }

    private IPathSearcher GetPathSearcher()
    {
        return SelectedPathType switch
        {
            PathType.XPath => new XPathSearcher(),
            PathType.JSONPath => new JSONPathSearcher(),
            _ => throw new NotImplementedException(),
        };
    }

    private void SaveSearchHistory()
    {
        GetHistoryInstance().Push(SelectedXPath);
    }

    private void ReloadSearchHistories()
    {
        XPaths.Clear();
        XPaths.AddRange(GetHistoryInstance().GetAll());
    }

    private HistoriesBase GetHistoryInstance()
    {
        return SelectedPathType switch
        {
            PathType.XPath => _xPathSearchTextHistories,
            PathType.JSONPath => _jsonPathSearchTextHistories,
            _ => throw new NotImplementedException(),
        };
    }
}
