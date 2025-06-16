using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using StarmyKnife.Core.Contracts.Services;
using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;
using StarmyKnife.UserControls.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.ViewModels;

public abstract class SinglePluginPageViewModelBase<TPlugin> : BindableBase, INotifyDataErrorInfo where TPlugin : IPlugin
{
    private IEventAggregator _eventAggregator;
    private ErrorsContainer<string> _errors;
    private IPluginLoaderService _pluginLoader;
    private ObservableCollection<PluginHost> _availablePlugins;
    private PluginHost _selectedPlugin;
    private PluginParameterBoxViewModel _pluginBox;

    public SinglePluginPageViewModelBase(IPluginLoaderService pluginLoader, IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        _errors = new ErrorsContainer<string>(OnErrorChanged);
        _pluginLoader = pluginLoader;

        _availablePlugins = new ObservableCollection<PluginHost>(_pluginLoader.GetPlugins<TPlugin>());
        _selectedPlugin = _availablePlugins.FirstOrDefault();
        _pluginBox = new PluginParameterBoxViewModel(_selectedPlugin, eventAggregator);

        DeletePluginBoxCommand = new DelegateCommand<PluginParameterBoxViewModel>(DeletePluginBox);
    }

    public bool HasErrors
    {
        get
        {
            return _errors.HasErrors;
        }
    }

    public ObservableCollection<PluginHost> AvailablePlugins => _availablePlugins;

    public PluginHost SelectedPlugin
    {
        get { return _selectedPlugin; }
        set
        {
            SetProperty(ref _selectedPlugin, value);
            if (SelectedPlugin != null)
            {
                PluginBox = new PluginParameterBoxViewModel(SelectedPlugin, _eventAggregator);
                OnSelectedPluginChanged();
            }
        }
    }

    public PluginParameterBoxViewModel PluginBox
    {
        get { return _pluginBox; }
        set { SetProperty(ref _pluginBox, value); }
    }
 
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public DelegateCommand<PluginParameterBoxViewModel> DeletePluginBoxCommand { get; }

    protected ErrorsContainer<string> Errors => _errors;

    protected IEventAggregator EventAggregator => _eventAggregator;

    public IEnumerable GetErrors(string propertyName)
    {
        return _errors.GetErrors(propertyName);
    }

    protected virtual void OnSelectedPluginChanged()
    {
        // can override
    }

    private void OnErrorChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void DeletePluginBox(PluginParameterBoxViewModel box)
    {
        // do nothing
    }
}
