using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace StarmyKnife.Behaviors
{
    public class CopyToClipboardOnClickBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(CopyToClipboardOnClickBehavior),
                new PropertyMetadata(true, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj)
            => (bool)obj.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject obj, bool value)
            => obj.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBox textBox)
            {
                var behaviors = Interaction.GetBehaviors(textBox);

                var existing = behaviors.FirstOrDefault(b => b is CopyToClipboardOnClickBehavior) as CopyToClipboardOnClickBehavior;

                if ((bool)e.NewValue)
                {
                    if (existing == null)
                    {
                        behaviors.Add(new CopyToClipboardOnClickBehavior());
                    }
                }
                else
                {
                    if (existing != null)
                    {
                        behaviors.Remove(existing);
                    }
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = AssociatedObject;
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectAll();

                Clipboard.SetText(textBox.Text);

                e.Handled = true;
            }
        }
    }
}
