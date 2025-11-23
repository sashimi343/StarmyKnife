using StarmyKnife.Core.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;

namespace StarmyKnife.UserControls.Views
{
    /// <summary>
    /// PartialSearchComboBox.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginListComboBox : ComboBox
    {
        private TextBox _textBox = null;
        private Popup _popUp = null;

        // デバウンス用（Task.Delay + CancellationTokenSource）
        private CancellationTokenSource _debounceCts;
        private const int DebounceMilliseconds = 300;

        // 正規化済みの現在の検索クエリ（Filter はこれを参照する）
        private string _currentQueryNormalized = string.Empty;

        public PluginListComboBox()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 同一 Loaded が複数回呼ばれても二重登録しない
            if (_textBox != null)
            {
                return;
            }

            _textBox = this.Template.FindName("PART_EditableTextBox", this) as TextBox;
            _popUp = this.Template.FindName("PART_Popup", this) as Popup;

            if (_textBox != null)
            {
                // ItemsSource に CollectionView を設定
                var collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                if (collectionView != null)
                {
                    collectionView.Filter = FilterPredicate;
                }

                _textBox.TextChanged += OnTextBoxTextChanged;

                _textBox.GotFocus += delegate
                {
                    if (!_popUp?.IsOpen ?? false)
                    {
                        _popUp.IsOpen = true;
                    }
                };

                _textBox.LostFocus += delegate
                {
                    if (_popUp?.IsOpen ?? false)
                    {
                        _popUp.IsOpen = false;
                    }
                };
            }
        }

        private async void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = _textBox?.Text ?? string.Empty;

            // 既存の待機があるならキャンセル
            _debounceCts?.Cancel();
            _debounceCts?.Dispose();
            _debounceCts = new CancellationTokenSource();
            var token = _debounceCts.Token;

            // 入力があるときにポップアップを開く（即時）
            if (!string.IsNullOrEmpty(text) && !(_popUp?.IsOpen ?? false))
            {
                _popUp.IsOpen = true;
            }

            // 空文字ならすぐに検索結果をリセット（ユーザーの操作感向上）
            if (string.IsNullOrEmpty(text))
            {
                // キャンセル不要、すぐに反映
                _currentQueryNormalized = string.Empty;
                Dispatcher.Invoke(() =>
                {
                    var collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                    collectionView.Refresh();
                });
                return;
            }

            try
            {
                // デバウンス待機（キャンセル可能）
                await Task.Delay(DebounceMilliseconds, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            // キャンセルされていなければ正規化して UI スレッドで反映
            var normalized = SearchTextNormalizer.Normalize(text);
            _currentQueryNormalized = normalized;

            // UI スレッドで Items.Refresh を呼び出す
            Dispatcher.Invoke(() =>
            {
                var collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                collectionView.Refresh();
            });
        }

        private bool FilterPredicate(object obj)
        {
            if (string.IsNullOrEmpty(_currentQueryNormalized))
            {
                return true;
            }

            var item = obj as PluginHost;
            return item?.SearchText.Contains(_currentQueryNormalized) ?? false;
        }
    }
}
