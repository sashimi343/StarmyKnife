using Prism.Common;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using StarmyKnife.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.UserControls.ViewModels
{
    public abstract class PluginParameterViewModelBase : BindableBase
    {
        private readonly IPluginParameter _pluginParameter;
        protected readonly IEventAggregator _eventAggregator;

        public string Name { get; }
        public string HelpText => _pluginParameter.HelpText ?? string.Empty;
        public bool HasHelpText => !string.IsNullOrWhiteSpace(_pluginParameter.HelpText);

        public PluginParameterViewModelBase(IPluginParameter parameter, IEventAggregator eventAggregator)
        {
            _pluginParameter = parameter;
            _eventAggregator = eventAggregator;
            Name = parameter.Name;
        }
    }
}
