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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSO.VO;
namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for UserUI.xaml
    /// </summary>
    public partial class UserUI : BaseUI
    {
        public UserUI()
        {
            InitializeComponent();
        }

        private void UserGrid_SelectionChange(object sender, DataChangedEventArgs e)
        {
            UserVO user = e.Entity as UserVO;

            UserVO users = UserGrid.GridUser.SelectedItem as UserVO;

            try
            {
                if (UserGrid.GridUser.ItemsSource == null)
                {

                }
                else
                {
                    UserForm.FillForm(user);
                    if (users != null)
                    {
                        List<RoleVO> roles = UserForm.ListboxRole.ItemsSource as List<RoleVO>;
                        UserForm.FillRole(roles, user.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data", ex.Message);
            }
        }

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            UserForm.Main = Main;
            UserGrid.Main = Main;
        }

        private void UserForm_DataChange(object sender, EventArgs e)
        {
            UserGrid.FillGrid();
        }
    }
}
