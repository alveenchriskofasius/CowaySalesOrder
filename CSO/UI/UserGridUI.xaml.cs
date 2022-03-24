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
using CSO.Proxy;
namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for UserGridUI.xaml
    /// </summary>
    public partial class UserGridUI : BaseUI
    {
        public UserGridUI()
        {
            InitializeComponent();
        }

        #region Event declarations
        public event EventHandler<DataChangedEventArgs> SelectionChange;
        protected virtual void OnSelectionChange(DataChangedEventArgs e)
        {
            EventHandler<DataChangedEventArgs> handler = SelectionChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
        private void GridUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserVO user = GridUser.SelectedItem as UserVO;
            if (user != null)
            {
                OnSelectionChange(new DataChangedEventArgs(user));
            }
        }

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }


        public void FillGrid()
        {
            try
            {
                GridUser.ItemsSource = UserProxy.Data();
            }
            catch (Exception e)
            {

                Main.ShowMessage("Gagal tarik data user", e.Message, Message.Error);
            }
        }
    }
}
