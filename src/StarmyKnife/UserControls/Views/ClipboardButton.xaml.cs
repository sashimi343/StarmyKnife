using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StarmyKnife.UserControls.Views
{
    /// <summary>
    /// ClipboardButton.xaml の相互作用ロジック
    /// </summary>
    public partial class ClipboardButton : UserControl
    {
        private readonly DispatcherTimer _popupTimer;

        public ClipboardButton()
        {
            InitializeComponent();
            this.Loaded += (s, e) => ApplyStyle();

            _popupTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _popupTimer.Tick += (s, e) =>
            {
                IsPopupOpen = false;
                _popupTimer.Stop();
            };
        }

        public static readonly DependencyProperty CopyTextProperty =
            DependencyProperty.Register(nameof(CopyText), typeof(object), typeof(ClipboardButton), new PropertyMetadata(null));

        public object CopyText
        {
            get => GetValue(CopyTextProperty);
            set => SetValue(CopyTextProperty, value);
        }

        public static readonly DependencyProperty IsLinkStyleProperty =
            DependencyProperty.Register(nameof(IsLinkStyle), typeof(bool), typeof(ClipboardButton), new PropertyMetadata(false, OnStyleChanged));

        public bool IsLinkStyle
        {
            get => (bool)GetValue(IsLinkStyleProperty);
            set => SetValue(IsLinkStyleProperty, value);
        }

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(ClipboardButton), new PropertyMetadata(false));

        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            string textToCopy;
            if (CopyText is string text)
            {
                textToCopy = text;
            }
            else if (CopyText is IEnumerable<string> list)
            {
                textToCopy = string.Join(Environment.NewLine, list);
            }
            else
            {
                textToCopy =  string.Empty;
            }

            if (!string.IsNullOrEmpty(textToCopy))
            {
                Clipboard.SetText(textToCopy);
                ShowCopiedPopup();
            }
        }

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ClipboardButton;
            control?.ApplyStyle();
        }

        private void ApplyStyle()
        {
            if (IsLinkStyle)
            {
                CopyBtn.Style = (Style)FindResource("LinkButtonStyle");
            }
            else
            {
                CopyBtn.ClearValue(Button.StyleProperty);
            }
        }

        private void ShowCopiedPopup()
        {
            IsPopupOpen = true;
            _popupTimer.Stop();
            _popupTimer.Start();
        }
    }
}
