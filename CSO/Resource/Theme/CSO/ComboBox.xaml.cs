using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public partial class ComboBox : ResourceDictionary
    {
        private void Global_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Ignore certain keys (F11, modifiers: CTRL, SHIFT, ALT)
            if (e.Key == Key.F11 || e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                return;
            }

            // Special key handler for combobox
            if (sender.GetType() == typeof(ComboBox))
            {
                System.Windows.Controls.ComboBox comboBox = sender as System.Windows.Controls.ComboBox;
                if (e.Key == Key.Enter)
                {
                    // close drop down as user press Enter
                    comboBox.IsDropDownOpen = false;
                    e.Handled = true;
                }
                else
                {
                    // drop down combo as user starts typing
                    comboBox.IsDropDownOpen = true;
                }
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

        private void ComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (sender.GetType() == typeof(ComboBox))
            {
                Console.WriteLine("Key Up ");
                System.Windows.Controls.ComboBox comboBox = sender as System.Windows.Controls.ComboBox;
                if (e.Key == Key.Down && !comboBox.IsDropDownOpen)
                {
                    // drop down combo as user press down
                    comboBox.IsDropDownOpen = true;
                    e.Handled = true;
                }

            }

        }

    }
}
