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
using System.Collections.ObjectModel;

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for OrderFormUI.xaml
    /// </summary>
    public partial class OrderFormUI : BaseUI
    {
        private int _noOfErrorsOnScreen;
        OrderVO _order;
        OrderProductVO _orderProduct;
        public OrderFormUI()
        {
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                _noOfErrorsOnScreen++;
            }
            else
            {
                _noOfErrorsOnScreen--;
            }
        }
        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            _order = new OrderVO();
            _orderProduct = new OrderProductVO();
            DataContext = _order;
            ComboProduct.DataContext = TextPcs.DataContext = _orderProduct;
            FillComboProduct(ComboProduct);
            FillComboLookup(ComboPaymentType, "Type");
            FillComboLookup(ComboServicePackage, "ServicePackage");
            FillComboCustomer(ComboCustomer);
            FillComboPromotion(ComboPromotion);
            ComboProduct.IsEnabled = TextPcs.IsEnabled = _order.CustomerID > 0 && _order.ServicePackageID > 0;
            FillForm(null);
        }

        private void FillForm(OrderVO order)
        {
            if (order == null)
            {
                order = new OrderVO();
            }

            _orderProduct.SetValue(order);

            GridProduct.ItemsSource = _order.Products;
        }
        private void ComboChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            CustomerVO customer = ComboCustomer.SelectedItem as CustomerVO;
            PromotionVO promotion = ComboPromotion.SelectedItem as PromotionVO;

            switch (combo.Name)
            {

                case "ComboProduct":
                    ProductVO product = ComboProduct.SelectedItem as ProductVO;
                    if (product != null)
                    {
                        _orderProduct.ProductID = product.ID;
                        _orderProduct.ProductName = product.Name;
                        _orderProduct.Amount = product.Price;
                        _orderProduct.DiscountAmount = product.Discount;
                        _orderProduct.ProductTypeID = product.ProductTypeID;
                    }
                    break;
                case "ComboCustomer":
                    List<LookupVO> types = ComboPaymentType.ItemsSource as List<LookupVO>;
                    LookupVO type = types.FirstOrDefault(x => x.ID == customer.TypeID);
                    ComboPaymentType.SelectedItem = type;
                    _order.Products = Promotion(true, customer.TypeID, customer.TypeID == 1 ? 2000000 : 1000000);
                    break;
                case "ComboPromotion":

                    if (promotion != null && customer != null)
                    {
                        _order.Products = Promotion(true, customer.TypeID, customer.TypeID == 1 ? 2000000 : 1000000);
                    }
                    break;
                case "ComboServicePackage":
                    //clear products in the grid 
                    GridProduct.ItemsSource = _order.Products = new ObservableCollection<OrderProductVO>();
                    break;
            }
            ComboProduct.IsEnabled = TextPcs.IsEnabled = CheckBoxPromotion.IsEnabled = _order.CustomerID != 0 && _order.ServicePackageID != 0;
        }
        private ObservableCollection<OrderProductVO> Promotion(bool isPromotion = false, int typeID = 0, decimal discounts = 0)
        {
            ObservableCollection<OrderProductVO> gridProducts = GridProduct.ItemsSource as ObservableCollection<OrderProductVO>;
            List<ProductVO> products = ComboProduct.ItemsSource as List<ProductVO>;

            if (isPromotion)
            {
                foreach (OrderProductVO orderProduct in GridProduct.ItemsSource)
                {
                    if (typeID == 1)
                    {
                        orderProduct.DiscountAmount = discounts;
                    }
                    else if (typeID == 2 && orderProduct.ProductTypeID == 1)
                    {
                        orderProduct.DiscountAmount = discounts;
                    }
                    else if (typeID == 2 && orderProduct.ProductTypeID == 2)
                    {
                        orderProduct.DiscountAmount = 0;
                    }
                    orderProduct.TotalAmount = _order.IsDiscounted ? (orderProduct.Amount * orderProduct.Quantity) - discounts : orderProduct.Amount * orderProduct.Quantity;
                }
                foreach (ProductVO product in products.ToList())
                {
                    if (typeID == 1)
                    {
                        product.Discount = discounts;
                    }
                    else if (typeID == 2 && product.ProductTypeID == 1)
                    {
                        product.Discount = discounts;
                    }
                    else if (typeID == 2 && product.ProductTypeID == 2)
                    {
                        product.Discount = 0;
                    }
                }
            }
            else
            {
                foreach (OrderProductVO orderProduct in GridProduct.ItemsSource)
                {
                    orderProduct.DiscountAmount = 0;
                    orderProduct.TotalAmount = _order.IsDiscounted ? (orderProduct.Amount * orderProduct.Quantity) - discounts : orderProduct.Amount * orderProduct.Quantity;
                }
                foreach (ProductVO product in products)
                {
                    product.Discount = 0;
                }
            }
            return gridProducts;
        }
        #region Command Definitions
        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0 && GridProduct.Items.Count > 0;
            e.Handled = true;
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
            e.Handled = true;
        }
        #endregion
        private async void Save()
        {
            if (Main.IsLoading)
            {
                return;
            }

            Main.ShowLoading(Loading.Saving, this);
            try
            {
                await OrderProxy.Save(_order);
                Main.ShowMessage("Data berhasil disimpan");
                //OnDataChanged(new EventArgs());
            }
            catch (Exception e)
            {
                Main.ShowMessage("Data gagal disimpan", e.Message, Message.Error);
            }
            Main.HideLoading();
        }
        private void GridProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow gridRow = Tool.UIHelper.GetParent(e.OriginalSource as Visual, typeof(DataGridRow)) as DataGridRow;
            if (gridRow != null)
            {
                OrderProductVO orderProduct = GridProduct.SelectedItem as OrderProductVO;
                if (orderProduct == null)
                {
                    return;
                }
                _orderProduct.SetValue(orderProduct);
                _orderProduct.IsEdited = true;
                ButtonCancel.Visibility = ButtonAccept.Visibility = Visibility.Visible;
            }
        }
        private void UpdateRow(bool cancel = false)
        {
            if (!cancel)
            {
                _orderProduct.TotalAmount = _order.IsDiscounted ? _orderProduct.Amount * _orderProduct.Quantity - _orderProduct.DiscountAmount : _orderProduct.Amount * _orderProduct.Quantity;

                if (_orderProduct.IsEdited == true && GridProduct.SelectedIndex >= 0)
                {
                    _order.Products.ElementAt(GridProduct.SelectedIndex).SetValue(_orderProduct);
                }
                else
                {
                    _orderProduct.IsEdited = true;
                    _order.Products.Add(new OrderProductVO(_orderProduct));

                    CSO.Tool.UIHelper.ScrollToBottom(GridProduct);
                }
            }

            _orderProduct.SetValue(new OrderProductVO { Quantity = 0 });

            // Hide edit button
            ButtonCancel.Visibility = ButtonAccept.Visibility = Visibility.Hidden;
        }
        private void GridProduct_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Name)
            {
                case "ButtonNew":
                    FillForm(null);
                    break;

                case "ButtonSearch":
                    ButtonSearch.IsEnabled = false;
                    //OnSearchClicked(new EventArgs());
                    break;

                case "ButtonDelete":
                    //if (_trade.ID == 0)
                    //{
                    //    return;
                    //}

                    //if (MessageBox.Show("Hapus Perhiasan " + _trade.No + "?", "Konfirmasi Hapus", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    //{
                    //    try
                    //    {
                    //        await TradeProxy.Delete(_trade.ID);

                    //        // Notify deleted data
                    //        OnDataDeleted(new DataChangedEventArgs(_trade));
                    //        // Display message
                    //        Main.ShowMessage("Data berhasil dihapus");

                    //        FillForm(null);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Main.ShowMessage("Data gagal di hapus", ex.Message);
                    //    }
                    //}
                    break;
                case "ButtonAccept":
                    UpdateRow();
                    break;

                case "ButtonCancel":
                    UpdateRow(true);
                    break;
            }
        }

        private async void TextPcs_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (ComboProduct.SelectedItem == null) { return; }

                if (_orderProduct.Quantity > 0)
                {
                    UpdateRow();

                    await Task.Delay(50);
                }
                ComboProduct.Focus();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == CheckBoxPromotion)
            {

                if (!_order.IsDiscounted) { _order.PromotionID = 0; ComboPromotion.IsEnabled = false; _order.Products = Promotion(false); }
                if (_order.IsDiscounted) { ComboPromotion.IsEnabled = true; }
            }
            if (sender == CheckBoxAddress)
            {
                CustomerVO customer = ComboCustomer.SelectedItem as CustomerVO;
                if (!_order.IsCustomerAddress) { _order.InstallAddress = String.Empty; }
                if (customer != null && _order.IsCustomerAddress)
                {
                    _order.InstallAddress = customer.Address;
                }
            }
        }
    }
}
