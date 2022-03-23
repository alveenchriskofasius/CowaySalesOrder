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
    /// Interaction logic for CustomerUI.xaml
    /// </summary>
    public partial class CustomerUI : BaseUI
    {
        PopupUI _popup;
        public CustomerUI()
        {
            InitializeComponent();
        }

        private void CustomerGrid_RowDoubleClicked(object sender, DataChangedEventArgs e)
        {
            CustomerVO customer = e.Entity as CustomerVO;
            CustomerFormUI customerForm = new CustomerFormUI();
            _popup = new PopupUI();
            _popup.Main = this.Main;
            customerForm.Main = _popup.Main;
            customerForm.DataChanged += CustomerForm_DataChanged;
            _popup.Title = "Master Pelanggan | Form Pelanggan";
            _popup.PanelMain.Children.Add(customerForm);

            if (customer != null)
            {
                customerForm.FillForm(customer);
            }
            else
            {
                customerForm.FillForm(null);
            }

            _popup.Main.ShowOverlay();
            _popup.ShowDialog();
        }

        private void CustomerForm_DataChanged(object sender, EventArgs e)
        {
            if (_popup != null)
            {
                CustomerGrid.FillGrid();
                _popup.Close();
            }
        }

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            CustomerGrid.Main = Main;
        }

    }
}
