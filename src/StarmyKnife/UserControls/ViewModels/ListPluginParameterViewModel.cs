using Prism.Events;
using Prism.Navigation;
using StarmyKnife.Core.Plugins;
using StarmyKnife.Core.Plugins.Internal;
using StarmyKnife.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StarmyKnife.UserControls.ViewModels
{
    public class ListPluginParameterViewModel : PluginParameterViewModelBase
    {
        private ListPluginParameter _parameter;
        private int _selectedIndex;

        public IEnumerable<ListItem> Items => _parameter.Items;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _parameter.SelectedIndex = value;
                SetProperty(ref _selectedIndex, value);
                _eventAggregator.GetEvent<PluginParameterChangedEvent>().Publish();
            }
        }

        public ListPluginParameterViewModel(ListPluginParameter parameter, IEventAggregator eventAggregator) : base(parameter, eventAggregator)
        {
            _parameter = parameter;
            _selectedIndex = _parameter.SelectedIndex;
        }
    }
}
