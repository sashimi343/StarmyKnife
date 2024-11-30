using Prism.Commands;
using Prism.Mvvm;
using StarmyKnife.Core.Models;
using StarmyKnife.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace StarmyKnife.ViewModels;

public class XPathFinderViewModel : BindableBase
{
    private string _inputXml;
    private string _sourceFilePath;
    private string _xpath;
    private ObservableCollection<string> _searchResults;

    public XPathFinderViewModel()
    {
        InputXml = string.Empty;
        SourceFilePath = string.Empty;
        XPath = string.Empty;
        SearchResults = new ObservableCollection<string>();

        SearchCommand = new DelegateCommand(Search);
    }

    public string InputXml
    {
        get => _inputXml;
        set => SetProperty(ref _inputXml, value);
    }

    public string SourceFilePath
    {
        get => _sourceFilePath;
        set => SetProperty(ref _sourceFilePath, value);
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

    private void Search()
    {
        var searcher = new XPathSearcher();
        if (!searcher.TryLoadXml(InputXml, out var error))
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            SearchResults.Clear();
            return;
        }

        var results = searcher.FindAllNodes(XPath);

        SearchResults.Clear();
        foreach (var result in results)
        {
            SearchResults.Add(result);
        }
    }
}
