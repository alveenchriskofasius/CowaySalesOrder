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
using System.Text.RegularExpressions;
using CSO.Tool;
using System.Collections.ObjectModel;

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for CustomerFormUI.xaml
    /// </summary>
    public partial class CustomerFormUI : BaseUI
    {
        private int _noOfErrorsOnScreen;
        private ObservableCollection<ZIPCodeVO> _postalCodes;
        private ObservableCollection<AreaVO> _cities;

        CustomerVO _customer;
        public CustomerFormUI()
        {
            InitializeComponent();
            _customer = new CustomerVO();
            DataContext = _customer;
            FillComboLookup(ComboType, "CustomerType");
            FillPostalCode();
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
        #region Command Definitions
        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
            e.Handled = true;
        }
        #endregion
        #region Event declarations
        public event EventHandler DataChanged;

        protected virtual void OnDataChanged(EventArgs e)
        {
            EventHandler handler = DataChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboCity();

            if (_customer.Postal != "")
            {
                SetPostCodeInfo(_customer.Postal);
            }
        }

        private void Save()
        {
            if (Main.IsLoading)
            {
                return;
            }

            Main.ShowLoading(Loading.Saving, this);
            try
            {
                CustomerProxy.Save(ref _customer);
                Main.ShowMessage("Data berhasil disimpan ");
                OnDataChanged(new EventArgs());
            }
            catch (Exception e)
            {

                Main.ShowMessage("Data gagal disimpan ", e.Message, Message.Error);

            }
            Main.HideLoading();
        }
        public void FillForm(CustomerVO customer)
        {
            if (customer == null)
            {
                customer = new CustomerVO();
            }
            else
            {
                customer = CustomerProxy.Data(customer);
            }
            // Set radio button
            List<RadioButton> radioButtons = new List<RadioButton>();
            UIHelper.WalkLogicalTree(radioButtons, PanelGender);
            foreach (RadioButton rb in radioButtons)
            {
                if (rb.GroupName == "Gender")
                {
                    rb.IsChecked = rb.Content.ToString() == customer.Gender;
                }
            }
            _customer.SetValue(customer);
            ButtonDelete.IsEnabled = customer.ID != 0;
            TextName.Focus();
        }
        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            switch (button.Name)
            {

                case "ButtonAdd":
                    FillForm(null);
                    break;

                case "ButtonDelete":
                    Delete();
                    break;


            }
        }
        public void Delete()
        {
            if (_customer.ID == 0)
            {
                return;
            }

            if (MessageBox.Show("Hapus pelanggan " + _customer.Name + "?", "Konfirmasi hapus ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    CustomerProxy.Delete(_customer);
                    // Notify deleted data
                    OnDataChanged(new EventArgs());

                    // Display message
                    Main.ShowMessage("Data berhasil dihapus ");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Data gagal dihapus  ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenderButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton gender = sender as RadioButton;

            _customer.Gender = gender.Content.ToString();

        }
        private void TextName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Force validation (why, because the built in validation only works if user has typed into the textbox)
            TextBox textBox = sender as TextBox;

            BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpression.UpdateSource();
            if (textBox.Name == "TextBoxZIP")
            {
                SetPostCodeInfo(textBox.Text, true);
            }
        }
        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //number only
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DateDOB_LostFocus(object sender, RoutedEventArgs e)
        {
            //compare date between what user select and current date
            DateTime date = DateTime.Now;
            DateTime age = _customer.Date.AddYears(65);
            int id = age.Date >= date ? 2 : 1;
            //fetch data from combotype and automatic select typeID to select customer type based customer age
            List<LookupVO> types = ComboType.ItemsSource as List<LookupVO>;
            LookupVO typeID = types.First(t => t.ID == id);
            ComboType.SelectedItem = typeID;
        }
        private void FillPostalCode()
        {
            _postalCodes = AreaProxy.FetchPostalCodes();
        }
        #region Fill Combo & Grid

        private void FillComboCity()
        {

            try
            {
                _cities = AreaProxy.Data(-1, Level.City);
                ComboCity.ItemsSource = _cities;
                ComboCity.DisplayMemberPath = "Name";
                ComboCity.SelectedValuePath = "ID";
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data kota", ex.Message);
            }
        }

        private void FillComboArea()
        {
            ObservableCollection<AreaVO> areas;
            try
            {
                areas = AreaProxy.Data(_customer.CityID, Level.Area);
                ComboArea.ItemsSource = areas;
                ComboArea.DisplayMemberPath = "Name";
                ComboArea.SelectedValuePath = "ID";
                ComboArea.SelectedValue = _customer.AreaID;
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data area", ex.Message);
            }
        }


        private void ComboCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillComboArea();
            ComboArea.IsEnabled = ComboCity.SelectedIndex >= 0;
        }
        private void ComboDistrict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            List<ZIPCodeVO> zipList = _postalCodes.Where(z => z.ZIP == _customer.Postal && z.District == _customer.DistrictName).ToList<ZIPCodeVO>();

            ComboUrban.Visibility = Visibility.Visible;
            ComboUrban.ItemsSource = zipList;
            ComboUrban.DisplayMemberPath = "Urban";
            ComboUrban.SelectedValuePath = "Urban";

            ZIPCodeVO traderUrban = zipList.Find(d => d.Urban == _customer.UrbanName);
            if (traderUrban != null)
            {
                ComboUrban.SelectedItem = traderUrban;
            }
            else
            {
                ComboUrban.SelectedIndex = 0; // select first item
            }

        }
        #endregion
        private void SetPostCodeInfo(string zipcode, bool force = false)
        {
            try
            {
                List<ZIPCodeVO> zipList = _postalCodes.Where(z => z.ZIP == zipcode).ToList<ZIPCodeVO>();
                if (zipList.Count > 0)
                {
                    AreaVO city = new AreaVO();
                    city = _cities.FirstOrDefault(c => c.Name == _customer.CityName);
                    if (city == null || force) // not found, get from the list
                    {
                        city = _cities.FirstOrDefault(c => c.Name == zipList[0].City);
                    }
                    ComboCity.SelectedItem = city;

                    List<ZIPCodeVO> lstDistrict = new List<ZIPCodeVO>();
                    string tempDistrict = string.Empty;
                    foreach (ZIPCodeVO item in zipList)
                    {
                        if (!tempDistrict.Equals(item.District))
                        {
                            lstDistrict.Add(item);
                        }
                        tempDistrict = item.District;
                    }


                    ComboDistrict.ItemsSource = lstDistrict;
                    ComboDistrict.DisplayMemberPath = "District";
                    ComboDistrict.SelectedValuePath = "District";
                    ZIPCodeVO traderDistrict = lstDistrict.Find(d => d.District == _customer.DistrictName);
                    if (traderDistrict != null)
                    {
                        ComboDistrict.SelectedItem = traderDistrict;
                    }
                    else
                    {
                        ComboDistrict.SelectedIndex = 0; // select first item
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Gagal memuat data kode pos",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                _customer.DistrictName = "";
                _customer.UrbanName = "";
                ComboCity.SelectedItem = null;
            }
        }

    }
}
