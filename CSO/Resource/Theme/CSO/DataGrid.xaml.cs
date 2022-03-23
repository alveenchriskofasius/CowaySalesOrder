using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CSO.Resource.Theme.CSO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class DataGrid : ResourceDictionary
    {
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Begin editing when drop down button is clicked
            System.Windows.Controls.DataGrid dataGrid = (System.Windows.Controls.DataGrid)Tool.UIHelper.GetParent((Visual)e.Source, typeof(System.Windows.Controls.DataGrid));
            dataGrid.BeginEdit();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (sender as System.Windows.Controls.TextBox);

            string decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // duplicate decimal separator, eg .. -> means user is starting to type the decimal, remove duplicates, and move caret accordingly
            if (textBox.Text.IndexOf(decimalSeparator + decimalSeparator) > 0)
            {
                string text = textBox.Text;
                text = text.Replace(decimalSeparator + decimalSeparator, decimalSeparator);
                textBox.Text = text;
                textBox.CaretIndex = textBox.Text.IndexOf(decimalSeparator) + 1;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
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

            System.Windows.Controls.TextBox textBox = (sender as System.Windows.Controls.TextBox);
            if (e.Key == Key.Back)
            {
                if (textBox.CaretIndex > 0)
                {
                    if (textBox.Text[textBox.CaretIndex - 1] + "" == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator ||
                        textBox.Text[textBox.CaretIndex - 1] + "" == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    {
                        if (textBox.Text[0] + "" == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator)
                            return;
                        textBox.CaretIndex = textBox.CaretIndex - 1;
                        e.Handled = true;
                    }
                }
            }
            if (e.Key == Key.Delete)
            {
                if (textBox.CaretIndex < textBox.Text.Length && textBox.CaretIndex > 0)
                {
                    if (textBox.Text[textBox.CaretIndex - 1] + "" == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator ||
                        textBox.Text[textBox.CaretIndex - 1] + "" == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    {
                        if (textBox.Text[0] + "" == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator)
                            return;
                        textBox.CaretIndex = textBox.CaretIndex + 1;
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
