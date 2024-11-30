using Prism.Events;
using StarmyKnife.Core.Plugins.Internal;
using StarmyKnife.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.UserControls.ViewModels
{
    public class FlagPluginParameterViewModel : PluginParameterViewModelBase
    {
        private FlagPluginParameter _parameter;
        private bool _isChecked;

        public bool IsChecked
        {
            get
            {
                return _parameter.Value;
            }
            set
            {
                _parameter.Value = value;
                SetProperty(ref _isChecked, value);
                _eventAggregator.GetEvent<PluginParameterChangedEvent>().Publish();
            }
        }

        public FlagPluginParameterViewModel(FlagPluginParameter parameter, IEventAggregator eventAggregator) : base(parameter, eventAggregator)
        {
            _parameter = parameter;
            IsChecked = _parameter.Value;
        }
    }
}
