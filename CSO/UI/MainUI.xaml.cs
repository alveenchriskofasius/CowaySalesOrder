using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CSO.Proxy;
using CSO.VO;

namespace CSO.UI
{
    /// <summary>   
    /// Interaction logic for MainUI.xaml
    /// </summary>

    public partial class MainUI : Window, IBaseUI
    {

        //private UserVO _user = new UserVO();
        public bool IsDirty { get; set; }

        private string _error = "";
        private string _connectedTo = "";


        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Message), typeof(MainUI), new PropertyMetadata(Message.Normal));

        public Message Type
        {
            get { return (Message)this.GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public MainUI(BaseUI userControl = null)
        {
            InitializeComponent();
            this.Left = (System.Windows.SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (System.Windows.SystemParameters.PrimaryScreenHeight - this.Height) / 2;
            if (userControl != null)
            {
                userControl.Main = this;
                Title = userControl.Title + _connectedTo;
                PanelMain.Children.Add(userControl);
            }

        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            // display username & icon
            SetProfile();
            SetMenuVisibility();
        }
        private void SetMenuVisibility()
        {
            //Customer.Visibility = UserProxy.CurrentUser.Role("Admin") || UserProxy.CurrentUser.Role("System") ? Visibility.Visible : Visibility.Collapsed;
            Transaction.Visibility = Order.Visibility = User.Visibility = !UserProxy.CurrentUser.Role("Sales") && !UserProxy.CurrentUser.Role("CT") ? Visibility.Visible : Visibility.Collapsed;

        }

        private void Main_Unloaded(object sender, RoutedEventArgs e)
        {
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem.Name == "Logout")
            {
                LoginUI loginUI = new LoginUI();
                loginUI.Show();
                this.Close();
            }
            else
            {
                string userControlName = menuItem.Name;

                BaseUI userControl = UserControl(userControlName);
                // assign parent
                userControl.Main = this;
                Title = userControl.Title;
                PanelMain.Children.Clear();
                PanelMain.Children.Add(userControl);
            }
        }
        private void Menu_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            string userControlName = menuItem.Name;

            BaseUI userControl = UserControl(userControlName);
            // assign parent
            userControl.Main = this;
            new MainUI(userControl).Show();
        }

        public void ShowMessage(string message, Message type = Message.Normal)
        {
            ShowMessage(message, "", type);
        }

        public void ShowMessage(string message, string error)
        {
            ShowMessage(message, error, Message.Error);
        }

        public void ShowMessage(string message, string error, Message type)
        {
            LabelMessage.Content = message;
            Type = type;
            _error = error;

            string imageSource = "";

            switch (type)
            {
                case Message.Normal:
                    imageSource = @"/Resource/Image/icon-checkmark-32.png";
                    break;
                case Message.Error:
                    imageSource = @"/Resource/Image/icon-cross-mark-32.png";
                    break;
                case Message.Info:
                    imageSource = @"/Resource/Image/icon-info-32.png";
                    break;
            }

            MessageIcon.Source = new BitmapImage(new Uri(imageSource, UriKind.Relative));

            // If type normal, info - show panel briefly then hide
            // If type error - show but don't hide
            Storyboard animatePanel = Resources[type != Message.Error ? "SBShowHideMessage" : "SBShowMessage"] as Storyboard;
            animatePanel.Begin(PanelMessage);

        }

        public bool IsLoading { get; set; }

        public string LoadingText
        {
            set
            {
                LabelLoading.Content = value;
            }
        }

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

            Point center;

            try
            {
                center = panel.TransformToAncestor(this).Transform(new Point(panel.ActualWidth / 2, panel.ActualHeight / 2));
                center.X -= PanelLoading.Width / 2;
                center.Y -= PanelLoading.Height / 2;
            }
            catch
            {
                // ignore transform error, just use Main's center
                center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            }


            PanelLoading.Margin = new Thickness(center.X, center.Y, 0, 0);

            Storyboard animatePanel = Resources["SBShowLoading"] as Storyboard;
            animatePanel.Begin(PanelLoading, true);

        }

        public void HideLoading()
        {
            Storyboard animatePanel = Resources["SBHideLoading"] as Storyboard;
            animatePanel.Begin(PanelLoading);
            IsLoading = false;
        }

        public void ShowOverlay()
        {
            Storyboard animatePanel = Resources["SBShowOverlay"] as Storyboard;
            animatePanel.Begin(PanelOverlay);
        }

        public void HideOverlay()
        {
            Storyboard animatePanel = Resources["SBHideOverlay"] as Storyboard;
            animatePanel.Begin(PanelOverlay);
        }
        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            Button actionButton = sender as Button;

            switch (actionButton.Name)
            {
                case "ViewError":
                    MessageBox.Show(_error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

            }

            // Hide Panel
            Storyboard animatePanel = Resources["SBHideMessage"] as Storyboard;
            animatePanel.Begin(PanelMessage);

        }
#if DEBUG
        private static bool fullScreen = false;
#else
        private static bool fullScreen = true;
#endif
        private void Main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (!fullScreen)
                {
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Normal;
                }

                fullScreen = !fullScreen;
            }
        }


        public void SetProfile()
        {
            LabelUserName.Content = UserProxy.CurrentUser.FullName;
            ImageProfile.Source = new BitmapImage(new Uri(UserProxy.CurrentUser.Gender == "P" ? @"/Resource/Image/icon-user-female2-50.png" : @"/Resource/Image/icon-user-male2-50.png", UriKind.Relative));
        }

        private BaseUI UserControl(string name)
        {
            BaseUI userControl = new BaseUI();
            switch (name)
            {
                case "Customer":
                    userControl = new CustomerUI();
                    break;
                case "Order":
                    userControl = new OrderUI();
                    break;
                case "User":
                    userControl = new UserUI();
                    break;
                case "Dashboard":
                    userControl = new DashboardUI();
                    break;
            }

            return userControl;

        }

    }
}
