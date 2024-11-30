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
    public class NumericPluginParameterViewModel : PluginParameterViewModelBase
    {
        private NumericPluginParameter _parameter;
        private decimal _value;

        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                _parameter.Value = value;
                SetProperty(ref _value, value);
                _eventAggregator.GetEvent<PluginParameterChangedEvent>().Publish();
            }
        }

        public NumericPluginParameterViewModel(NumericPluginParameter parameter, IEventAggregator eventAggregator) : base(parameter, eventAggregator)
        {
            _parameter = parameter;
            Value = parameter.Value;
        }
    }
}
