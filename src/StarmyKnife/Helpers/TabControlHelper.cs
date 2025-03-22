using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StarmyKnife.Helpers
{
    public static class TabControlHelper
    {
        public const string ClearFocusOnTabChangePropertyName = "ClearFocusOnTabChange";

        public static readonly DependencyProperty ClearFocusOnTabChangeProperty =
            DependencyProperty.RegisterAttached(
                ClearFocusOnTabChangePropertyName,
                typeof(bool),
                typeof(TabControlHelper),
                new PropertyMetadata(false, OnClearFocusOnTabChangeChanged));

        public static bool GetClearFocusOnTabChange(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClearFocusOnTabChangeProperty);
        }

        public static void SetClearFocusOnTabChange(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearFocusOnTabChangeProperty, value);
        }

        private static void OnClearFocusOnTabChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabControl tabControl)
            {
                if ((bool)e.NewValue)
                {
                    tabControl.SelectionChanged += TabControl_SelectionChanged;
                }
                else
                {
                    tabControl.SelectionChanged -= TabControl_SelectionChanged;
                }
            }
        }

        private static void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                Keyboard.ClearFocus();
            }
        }
    }
}
