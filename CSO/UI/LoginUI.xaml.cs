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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CSO.Proxy;
using CSO.VO;
using Helper;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
    public partial class LoginUI : Window
    {
        public LoginUI()
        {
            InitializeComponent();
            this.Left = (System.Windows.SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (System.Windows.SystemParameters.PrimaryScreenHeight - this.Height) / 2;



#if DEBUG
            TextUsername.Text = "Admin";
            TextPassword.Password = "CSOAdmin";
#endif

        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (await UserProxy.Login(TextUsername.Text, TextPassword.Password))
                {
                    if (UserProxy.CurrentUser.Role("CT") || UserProxy.CurrentUser.Role("Person In Charge"))
                    {
                        MessageBox.Show("Role tidak bisa login", "Gagal Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MainUI mainUI = new MainUI();
                        mainUI.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Username atau Password tidak valid",
                                    "Gagal Login",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),
                                "Gagal Login",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Control textBox = sender as Control;

            textBox.Foreground = Brushes.White;
            textBox.Background = new SolidColorBrush(Color.FromRgb(175, 190, 213)); ;

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Control textBox = sender as Control;
            BrushConverter brushConverter = new BrushConverter();

            textBox.Foreground = (Brush)brushConverter.ConvertFrom("#FFA5B7D2");
            textBox.Background = (Brush)brushConverter.ConvertFrom("#FFE8F0FA");

        }

        private void TxtPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonLogin.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

    }
}
