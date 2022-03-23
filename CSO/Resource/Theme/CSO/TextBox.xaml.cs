using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSO.Resource.Theme.CSO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class TextBox : ResourceDictionary
    {
        private void Global_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Ignore certain keys (F11, modifiers: CTRL, SHIFT, ALT)
            if (e.Key == Key.F11 || e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                return;
            }

            UIElement uiElement = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uiElement.MoveFocus(
                new TraversalRequest(
                FocusNavigationDirection.Next));
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.TextBox).SelectAll();
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;

            if (textBox.SelectionLength == 0 && !textBox.IsFocused)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            textBox.SelectAll();
            e.Handled = true;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.PasswordBox).SelectAll();
        }

        private void PasswordBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (!passwordBox.IsSelectionActive && !passwordBox.IsFocused)
            {
                passwordBox.Focus();
                e.Handled = true;
            }
        }
    }
}
