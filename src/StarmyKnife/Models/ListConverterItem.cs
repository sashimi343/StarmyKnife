using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarmyKnife.PluginInterfaces;

namespace StarmyKnife.Models
{
    public class ListConverterItem : BindableBase
    {
        private string _input;
        private string _output;

        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        public string Output
        {
            get => _output;
            set => SetProperty(ref _output, value);
        }

        public ListConverterItem()
        {
            _input = string.Empty;
            _output = string.Empty;
        }

        public ListConverterItem(string input)
        {
            _input = input;
            _output = string.Empty;
        }
    }
}
