using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using StarmyKnife.Core.Models;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Core.Plugins.Internal;
using StarmyKnife.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StarmyKnife.UserControls.ViewModels
{
    public class PluginParameterBoxViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private PluginHost _pluginHost;
        private PluginParameterCollection _parameters;
        private List<PluginParameterViewModelBase> _parameterViewModels;

        public ObservableCollection<PluginParameterViewModelBase> ParametersViewModels
        {
            get
            {
                return new ObservableCollection<PluginParameterViewModelBase>(_parameterViewModels);
            }
            set
            {
                SetProperty(ref _parameterViewModels, value.ToList());
            }
        }

        public IPlugin Plugin => _pluginHost.Plugin;
        public PluginParameterCollection Parameters => _parameters;

        public string Name => _pluginHost.Name;

        public bool IsDeletable { get; set; } = false;

        public Visibility DeleteButtonVisibility
        {
            get
            {
                return IsDeletable ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public PluginParameterBoxViewModel(PluginHost pluginHost, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PluginParameterChangedEvent>().Subscribe(OnParameterChanged);
            _pluginHost = pluginHost;
            _parameters = _pluginHost.Plugin.GetParametersSchema();
            _parameterViewModels = new List<PluginParameterViewModelBase>();

            foreach (var parameter in _parameters.Values)
            {
                if (parameter is FlagPluginParameter flagParameter)
                {
                    _parameterViewModels.Add(new FlagPluginParameterViewModel(flagParameter, eventAggregator));
                }
                else if (parameter is TextPluginParameter textParameter)
                {
                    _parameterViewModels.Add(new TextPluginParameterViewModel(textParameter, eventAggregator));
                }
                else if (parameter is ListPluginParameter listParameter)
                {
                    _parameterViewModels.Add(new ListPluginParameterViewModel(listParameter, eventAggregator));
                }
                else if (parameter is NumericPluginParameter numericParameter)
                {
                    _parameterViewModels.Add(new NumericPluginParameterViewModel(numericParameter, eventAggregator));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void OnParameterChanged()
        {
           _eventAggregator.GetEvent<PluginParameterBoxChangedEvent>().Publish();
        }
    }
}
