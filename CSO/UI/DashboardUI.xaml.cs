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
    /// Interaction logic for DashboardUI.xaml
    /// </summary>
    public partial class DashboardUI : BaseUI
    {
        public DashboardUI()
        {
            InitializeComponent();
        }

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            DashboardSelector.Main = Main;
            DashboardDetail.Main = Main;
        }

        private void TogglePanel()
        {
            if (PanelForm.Visibility == Visibility.Collapsed)
            {
                // Show Panel
                Storyboard animatePanel = Application.Current.Resources["SBPanelFadeIn"] as Storyboard;
                animatePanel.Begin(PanelForm);
                animatePanel = Application.Current.Resources["SBOverlayFadeIn"] as Storyboard;
                animatePanel.Begin(PanelOverlay);

            }
            else
            {
                // Hide Panel
                Storyboard animatePanel = Application.Current.Resources["SBPanelFadeOut"] as Storyboard;
                animatePanel.Begin(PanelForm);
                animatePanel = Application.Current.Resources["SBOverlayFadeOut"] as Storyboard;
                animatePanel.Begin(PanelOverlay);
            }
        }
        private void PanelOverlay_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Hide Panel
            if (PanelOverlay.Opacity == 0.4)
            {
                TogglePanel();
            }
        }
        private void DashboardSelector_RowDoubleClicked(object sender, DataChangedEventArgs e)
        {

            TogglePanel();
            if (e.Entity as OrderVO != null)
            {
                DashboardDetail.FillDetail(e.Entity as OrderVO);
            }
        }
    }
}
