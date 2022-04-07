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
using System.Transactions;
using Microsoft.Win32;
using System.IO;

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
        public event EventHandler SearchClicked;
        protected virtual void OnSearchClicked(EventArgs e)
        {
            EventHandler handler = SearchClicked;
            if (handler != null)
            {
                handler(this, e);
            }
            _order.Validate();
        }
        public void Restore()
        {
            _order.Restore();
        }
        #region Event declarations
        #endregion
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
            FillComboCustomer(ComboCustomer);
            FillComboPromotion(ComboPromotion);
            FillComboPIC();
            ComboProduct.IsEnabled = TextPcs.IsEnabled = _order.CustomerID > 0 && _order.ServicePackageID > 0;
            FillForm(null);
        }
        private void FillComboPIC()
        {
            try
            {
                ComboPIC.ItemsSource = UserProxy.Data().Where(x => x.Role("Person In Charge")).ToList();
                ComboPIC.SelectedValuePath = "ID";
                ComboPIC.DisplayMemberPath = "FullName";
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void FillForm(OrderVO order)
        {
            if (order == null)
            {
                order = new OrderVO();
                _order.SetValue(order);
                ComboServicePackage.IsEnabled = false;
            }
            else
            {

                _order.SetValue(OrderProxy.Data(order.ID));
                ComboServicePackage.ItemsSource = FillComboService(_order.TypeID);
                _order.IsUploaded = _order.URLImage.Length > 0;
                ComboServicePackage.IsEnabled = _order.ServicePackageID != 1 && _order.StatusID == 1;
            }

            GridProduct.ItemsSource = _order.Products;
            ImageUploaded.Visibility = _order.IsUploaded ? Visibility.Visible : Visibility.Collapsed;
            Calculate();

            ButtonDelete.IsEnabled = _order.ID != 0 && _order.StatusID == 1; // only status Open that can be deleted

        }
        private void Calculate()
        {
            TextDiscount.Text = _order.Discount.ToString("N0");
            TextGrandTotal.Text = _order.Products.Sum(x => x.TotalAmount).ToString("N0");
            _order.GrandTotal = _order.Products.Sum(x => x.TotalAmount);
        }
        private void ComboChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            CustomerVO customer = ComboCustomer.SelectedItem as CustomerVO;
            PromotionVO promotion = ComboPromotion.SelectedItem as PromotionVO;
            ProductVO product = ComboProduct.SelectedItem as ProductVO;
            LookupVO service = ComboServicePackage.SelectedItem as LookupVO;
            switch (combo.Name)
            {

                case "ComboProduct":
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
                    if (customer == null) return;
                    LookupVO type = types.FirstOrDefault(x => x.ID == customer.TypeID);
                    ComboPaymentType.SelectedItem = type;
                    List<LookupVO> services = FillComboService(type.ID);
                    if (type.ID == 1)
                    {
                        LookupVO selectPackage = services.Where(x => x.ID == 1).FirstOrDefault();
                        ComboServicePackage.SelectedItem = selectPackage;
                    }

                    ComboServicePackage.IsEnabled = type.ID != 1;
                    Promotion();
                    break;
                case "ComboPromotion":

                    if (promotion != null && customer != null)
                    {
                        Promotion();
                    }
                    break;
                case "ComboServicePackage":

                    //clear products in the grid
                    break;

            }
            if (ComboProduct.SelectedItem != null)
            {
                List<ProductVO> products = ComboProduct.ItemsSource as List<ProductVO>;
                product.Discount = _orderProduct.DiscountAmount = _order.PromotionID != 0 && _order.IsDiscounted ? products.Where(x => x.ID == product.ID).FirstOrDefault().Discount : 0;
            }
            ComboProduct.IsEnabled = TextPcs.IsEnabled = CheckBoxPromotion.IsEnabled = _order.CustomerID != 0 && _order.ServicePackageID != 0;
        }

        private List<LookupVO> FillComboService(int typeID)
        {
            List<LookupVO> lookups =
            typeID == 1 ? LookupProxy.Data("ServicePackage", null).Where(x => x.ID == 1).ToList() : LookupProxy.Data("ServicePackage", null).Where(x => x.ID != 1).ToList();
            ComboServicePackage.ItemsSource = lookups;
            ComboServicePackage.SelectedValuePath = "ID";
            ComboServicePackage.DisplayMemberPath = "Name";
            if (typeID == 2 && _order.ID > 0)
            {
                LookupVO lookup = lookups.FirstOrDefault(x => x.ID == _order.ServicePackageID);
                if (lookup != null)
                {
                    ComboServicePackage.SelectedValue = lookup.ID;
                }
            }
            else
            {
                ComboServicePackage.SelectedIndex = 0;
            }
            return lookups;
        }

        private void Promotion()
        {
            List<ProductVO> products = ComboProduct.ItemsSource as List<ProductVO>;
            decimal discounts = _order.PaymentTypeID == 1 && _order.PromotionID != 0 ? 2000000 : 1000000;

            if (_order.IsDiscounted && _order.PromotionID != 0)
            {
                foreach (OrderProductVO orderProduct in _order.Products)
                {
                    if (_order.PaymentTypeID == 1)
                    {
                        orderProduct.DiscountAmount = discounts;
                    }
                    else if (_order.PaymentTypeID == 2 && orderProduct.ProductTypeID == 1)
                    {
                        orderProduct.DiscountAmount = discounts;
                    }
                    else if (_order.PaymentTypeID == 2 && orderProduct.ProductTypeID == 2)
                    {
                        orderProduct.DiscountAmount = 0;
                    }
                    orderProduct.TotalAmount = _order.IsDiscounted ? (orderProduct.Amount * orderProduct.Quantity) - discounts : orderProduct.Amount * orderProduct.Quantity;
                    orderProduct.IsEdited = true;
                }
                foreach (ProductVO product in products.ToList())
                {
                    if (_order.PaymentTypeID == 1)
                    {
                        product.Discount = discounts;
                    }
                    else if (_order.PaymentTypeID == 2 && product.ProductTypeID == 1)
                    {
                        product.Discount = discounts;
                    }
                    else if (_order.PaymentTypeID == 2 && product.ProductTypeID == 2)
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
                    orderProduct.IsEdited = true;
                }
                foreach (ProductVO product in products)
                {
                    product.Discount = 0;
                }
            }
            if (_order.PromotionID != 0)
            {
                _order.Discount = _order.PaymentTypeID == 1 ? 2000000 : 1000000;
            }
            else
            {
                _order.Discount = 0;
            }
            Calculate();
        }
        #region Command Definitions
        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0 && GridProduct.Items.Count > 0 && _order.IsUploaded && _order.StatusID == 1;
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
                ButtonDelete.IsEnabled = _order.ID != 0;
                // reset edited
                foreach (OrderProductVO product in _order.Products)
                {
                    product.IsEdited = false;
                }
            }
            catch (Exception e)
            {
                Main.ShowMessage("Data gagal disimpan", e.Message, Message.Error);
            }
            Main.HideLoading();
        }
        private async void DeleteProduct()
        {
            bool delete = true;

            OrderProductVO orderProduct = GridProduct.SelectedItem as OrderProductVO;

            if (orderProduct == null)
            {
                return;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (orderProduct.ID != 0)
                    {
                        delete = MessageBox.Show("Hapus data?", "Konfirmasi Hapus", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                        if (delete)
                        {
                            OrderProxy.Delete(orderProduct);
                        }
                    }

                    if (delete)
                    {
                        ObservableCollection<OrderProductVO> orderProducts = GridProduct.ItemsSource as ObservableCollection<OrderProductVO>;
                        if (orderProducts.Count > 0)
                        {
                            orderProducts.Remove(orderProduct);
                            Calculate();
                        }
                    }

                    if (orderProduct.ID != 0 && delete)
                    {
                        await OrderProxy.Save(_order);
                        Main.ShowMessage("Data berhasil dihapus");
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Data gagal dihapus", ex.Message);
            }
        }

        override protected void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dataGrid = e.Source as DataGrid;

            if (dataGrid == null)
            {
                return;
            }

            int selectedIndex = dataGrid.SelectedIndex;

            // check current item if selected item is null
            if (selectedIndex < 0 && dataGrid.CurrentItem != null)
            {
                selectedIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                dataGrid.SelectedIndex = selectedIndex;
            }

            if (selectedIndex >= 0)
            {
                if (e.Key == Key.Delete && (e.OriginalSource.GetType() == typeof(DataGridCell)))
                {
                    e.Handled = true;
                    DeleteProduct();
                }
            }

            base.Grid_PreviewKeyDown(sender, e);
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

                    Tool.UIHelper.ScrollToBottom(GridProduct);
                }
            }

            _orderProduct.SetValue(new OrderProductVO { Quantity = 0 });
            Calculate();
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
                    OnSearchClicked(new EventArgs());
                    break;

                case "ButtonDelete":
                    if (_order.ID == 0)
                    {
                        return;
                    }

                    if (MessageBox.Show("Hapus Order " + _order.No + "?", "Konfirmasi Hapus", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            OrderProxy.Delete(_order);

                            // Display message
                            Main.ShowMessage("Data berhasil dihapus");

                            FillForm(null);
                        }
                        catch (Exception ex)
                        {
                            Main.ShowMessage("Data gagal di hapus", ex.Message);
                        }
                    }
                    break;
                case "ButtonAccept":
                    UpdateRow();
                    break;

                case "ButtonCancel":
                    UpdateRow(true);
                    break;
                case "ButtonUpload":
                    OpenFileDialog op = new OpenFileDialog();
                    op.Title = "Pilih KTP";
                    op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                      "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                      "Portable Network Graphic (*.png)|*.png";
                    if (op.ShowDialog() == true)
                    {
                        _order.URLImage = ReadFile(op.FileName);
                        _order.IsUploaded = true;
                        ImageUploaded.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }
        private byte[] ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes to read from file.
            //In this case we want to read entire file. So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);
            return data;
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

                if (!_order.IsDiscounted) { _order.PromotionID = 0; ComboPromotion.IsEnabled = false; Promotion(); }
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
