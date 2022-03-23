using System.Windows;
using System.Windows.Media.Animation;

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for PopupUI.xaml
    /// </summary>
    public partial class PopupUI : Window
    {
        public PopupUI()
        {
            InitializeComponent();
            this.Left = (System.Windows.SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (System.Windows.SystemParameters.PrimaryScreenHeight - this.Height) / 2;

        }
        IBaseUI _main;
        public IBaseUI Main
        {
            get { return _main; }
            set
            {
                _main = value;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Main != null) Main.HideOverlay();
        }

        public bool IsLoading { get; set; }

        public void ShowLoading(Loading loading, BaseUI panel)
        {

            if (IsLoading)
            {
                return;
            }

            IsLoading = true;

            switch (loading)
            {
                case Loading.Loading:
                    LabelLoading.Content = "Loading ...";
                    break;
                case Loading.Saving:
                    LabelLoading.Content = "Saving ...";
                    break;
                case Loading.Deleting:
                    LabelLoading.Content = "Deleting ...";
                    break;
            }

            //Point center;

            //try
            //{
            //    center = panel.TransformToAncestor(this).Transform(new Point(panel.ActualWidth / 2, panel.ActualHeight / 2));
            //    center.X -= PanelLoading.Width / 2;
            //    center.Y -= PanelLoading.Height / 2;
            //}
            //catch
            //{
            //    // ignore transform error, just use Main's center
            //    center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            //}

            //PanelLoading.Margin = new Thickness(center.X, center.Y, 0, 0);

            Storyboard animatePanel = Resources["SBShowLoading"] as Storyboard;
            animatePanel.Begin(PanelLoading, true);

        }

        public void HideLoading()
        {
            Storyboard animatePanel = Resources["SBHideLoading"] as Storyboard;
            animatePanel.Begin(PanelLoading);
            IsLoading = false;
        }

    }
}
