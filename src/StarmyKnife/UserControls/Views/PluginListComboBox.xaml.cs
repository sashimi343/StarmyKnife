﻿using StarmyKnife.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StarmyKnife.UserControls.Views
{
    /// <summary>
    /// PartialSearchComboBox.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginListComboBox : ComboBox
    {
        private TextBox _textBox = null;
        private Popup _popUp = null;

        public PluginListComboBox()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                _textBox = this.Template.FindName("PART_EditableTextBox", this) as TextBox;
                _popUp = this.Template.FindName("PART_Popup", this) as Popup;

                if (_textBox != null)
                {
                    _textBox.TextChanged += delegate
                    {
                        this.Items.Filter = null;

                        if (!_popUp.IsOpen && string.IsNullOrEmpty(_textBox.Text))
                        {
                            this.Items.Filter += obj =>
                            {
                                return true;
                            };

                            return;
                        }

                        this.Items.Filter += obj =>
                        {
                            if (!(obj is PluginHost))
                            {
                                return true;
                            }

                            var item = obj as PluginHost;
                            if (item.Name.Contains(_textBox.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }

                            return false;
                        };

                        _popUp.IsOpen = true;
                    };

                    _textBox.GotFocus += delegate
                    {
                        if (!_popUp.IsOpen)
                        {
                            _popUp.IsOpen = true;
                        }
                    };
                }
            };
        }
    }
}
