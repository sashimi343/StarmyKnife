using Prism.Common;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using StarmyKnife.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.UserControls.ViewModels
{
    public abstract class PluginParameterViewModelBase : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        public string Name { get; }

        public PluginParameterViewModelBase(IPluginParameter parameter, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Name = parameter.Name;
        }
    }
}
