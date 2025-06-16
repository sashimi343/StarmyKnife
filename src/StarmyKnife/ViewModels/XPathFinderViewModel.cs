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
    private string _xpath;
    private ObservableCollection<string> _searchResults;

    public XPathFinderViewModel()
    {
        SelectedPathType = PathType.XPath;
        InputXml = string.Empty;
        XPath = string.Empty;
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

    public string XPath
    {
        get => _xpath;
        set => SetProperty(ref _xpath, value);
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
            var results = searcher.FindAllNodes(XPath);

            SearchResults.Clear();
            foreach (var result in results)
            {
                SearchResults.Add(result);
            }
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
}
