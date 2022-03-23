using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace CSO.Resource.Theme.CSO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class Menu : ResourceDictionary
    {
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {

            MenuItem menuItem = Tool.UIHelper.GetAscendantByType((Visual)sender, typeof(MenuItem)) as MenuItem;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                // Put long-running operation here
                menuItem.RaiseEvent(new RoutedEventArgs(MenuItem.CheckedEvent));
            }));

            (menuItem.Parent as MenuItem).IsSubmenuOpen = false;
            //menu.IsOpen = false;
        }
    }
}
