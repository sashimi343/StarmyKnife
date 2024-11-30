using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StarmyKnife.UserControls.Views
{
    /// <summary>
    /// NumericTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class NumericTextBox : TextBox
    {
        private const string RegexIntegerDigit = @"[0-9-]";
        private const string RegexDecimalChar = @"[0-9-.]";
        private const string RegexUnnecessarySign = @"(^\.|\.$|^[-.]+$|[-.][-.]+)";

        public bool IsInteger { get; set; } = true;

        public NumericTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var inputStr = e.Text;
            var textBox = (TextBox)sender;

            if (IsInteger)
            {
                e.Handled = IsInvalidInputForInteger(inputStr, textBox);
            }
            else
            {
                e.Handled = IsInvalidInputForDecimal(inputStr, textBox);
            }
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var currentStr = textBox.Text;
            textBox.Text = Regex.Replace(currentStr, RegexUnnecessarySign, "");
        }

        private bool IsInvalidInputForInteger(string inputStr, TextBox textBox)
        {
            var currentStr = textBox.Text;

            if (!Regex.IsMatch(inputStr, RegexIntegerDigit))
            {
                return true;
            }

            if (textBox.CaretIndex != 0 && inputStr == "-")
            {
                return true;
            }

            if (textBox.SelectedText != currentStr && currentStr.Contains('-') && inputStr == "-")
            {
                return true;
            }

            return false;
        }

        private bool IsInvalidInputForDecimal(string inputStr, TextBox textBox)
        {
            var currentStr = textBox.Text;

            if (!Regex.IsMatch(inputStr, RegexDecimalChar))
            {
                return true;
            }

            if (textBox.CaretIndex != 0 && inputStr == "-")
            {
                return true;
            }

            if (textBox.SelectedText != currentStr && currentStr.Contains('-') && inputStr == "-")
            {
                return true;
            }

            if (currentStr.Contains('.') && inputStr == ".")
            {
                return true;
            }

            return false;
        }
    }
}
