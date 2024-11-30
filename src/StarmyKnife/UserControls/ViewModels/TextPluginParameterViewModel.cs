using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using StarmyKnife.Core.Plugins.Internal;
using StarmyKnife.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xaml;

namespace StarmyKnife.UserControls.ViewModels
{
    public class TextPluginParameterViewModel : PluginParameterViewModelBase
    {
        private TextPluginParameter _parameter;
        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _parameter.Text = value;
                SetProperty(ref _text, value);
                _eventAggregator.GetEvent<PluginParameterChangedEvent>().Publish();
            }
        }

        public TextPluginParameterViewModel(TextPluginParameter parameter, IEventAggregator eventAggregator) : base(parameter, eventAggregator)
        {
            _parameter = parameter;
            Text = _parameter.Text;
        }
    }
}
