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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSO.VO;
namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for OrderUI.xaml
    /// </summary>
    public partial class OrderUI : BaseUI
    {
        public OrderUI()
        {
            InitializeComponent();
        }

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            OrderForm.Main = Main;
            OrderGrid.Main = Main;
        }

        private void OrderForm_SearchClicked(object sender, EventArgs e)
        {
            ToggleSearchPanel();
        }
        private void PanelOverlay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Hide Panel
            if (PanelOverlay.Opacity == 0.4)
            {
                ToggleSearchPanel();
            }
            OrderForm.Restore();
        }

        #region Helper

        private void ToggleSearchPanel()
        {
            if (PanelSearch.Visibility == Visibility.Collapsed)
            {
                // Show Panel
                Storyboard animatePanel = Application.Current.Resources["SBShowBottom"] as Storyboard;
                animatePanel.Begin(PanelSearch);
                animatePanel = Application.Current.Resources["SBOverlayFadeIn"] as Storyboard;
                animatePanel.Begin(PanelOverlay);
                OrderGrid.FillGrid();
            }
            else
            {
                // Hide Panel
                Storyboard animatePanel = Application.Current.Resources["SBHideBottom"] as Storyboard;
                animatePanel.Begin(PanelSearch);
                animatePanel = Application.Current.Resources["SBOverlayFadeOut"] as Storyboard;
                animatePanel.Begin(PanelOverlay);

            }
        }

        #endregion

        private void OrderGrid_RowDoubleClicked(object sender, DataChangedEventArgs e)
        {
            if (e.Entity as OrderVO != null)
            {
                OrderForm.FillForm(e.Entity as OrderVO);

            }
            else
            {
                OrderForm.FillForm(null);
            }
            ToggleSearchPanel();
        }
    }
}
