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
using System.Windows.Threading;

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for CustomerGridUI.xaml
    /// </summary>
    public partial class CustomerGridUI : BaseUI
    {
        FilterVO _filter;
        FilterVO _oldFilter;
        public CustomerGridUI()
        {
            InitializeComponent();
        }
        #region Event declarations
        public event EventHandler<DataChangedEventArgs> RowDoubleClicked;
        protected virtual void OnRowDoubleClicked(DataChangedEventArgs e)
        {
            EventHandler<DataChangedEventArgs> handler = RowDoubleClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private void GridCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow gridRow = Tool.UIHelper.GetParent(e.OriginalSource as Visual, typeof(DataGridRow)) as DataGridRow;
            if (gridRow != null)
            {
                CustomerVO customer = GridCustomer.SelectedItem as CustomerVO;
                OnRowDoubleClicked(new DataChangedEventArgs(customer));
            }
        }

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Name)
            {
                case "ButtonAdd":
                    OnRowDoubleClicked(new DataChangedEventArgs(null));
                    break;
                case "ButtonRefresh":
                    FillGrid();
                    break;
                case "ButtonReset":
                    FilterVO newFilter = new FilterVO();
                    _filter.SetValue(newFilter);
                    FillGrid(); // apply filters
                    break;
            }
        }
        public async void FillGrid()
        {
            Main.ShowLoading(Loading.Loading, this);

            try
            {
                GridCustomer.ItemsSource = await CustomerProxy.Data(_filter);

                // update old filter
                _oldFilter.SetValue(_filter);
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data", ex.Message);
            }

            Main.HideLoading();

        }
        private void FillComboCity()
        {
            // Fill City Combo
            ObservableCollection<AreaVO> cities;
            try
            {
                int ID = _filter.ProvinceID;
                cities = AreaProxy.Data(-1, Level.City, true);
                ComboCity.ItemsSource = cities;
                ComboCity.DisplayMemberPath = "Name";
                ComboCity.SelectedValuePath = "ID";
                ComboCity.IsTextSearchEnabled = false;

            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data kota", ex.Message);
            }
        }

        private void FillComboArea()
        {
            // Fill Area Combo
            ObservableCollection<AreaVO> areas;
            try
            {
                areas = AreaProxy.Data(_filter.CityID, Level.Area, true);
                ComboArea.ItemsSource = areas;
                ComboArea.DisplayMemberPath = "Name";
                ComboArea.SelectedValuePath = "ID";
                ComboArea.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data area", ex.Message);
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((sender as ComboBox).Name == "ComboCity")
            {
                // Repopulate City combos
                FillComboArea();
            }
            // Perform search when filter is changed 
            if (!_oldFilter.Compare(_filter))
            {
                FillGrid();
            }
        }

        private void CheckFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            // this if clause condition prevent filter executed
            //if (!_oldFilter.Compare(_filter))
            {
                FillGrid();
            }
        }
        DispatcherTimer _typingTimer;
        private void TextName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_typingTimer == null)
            {
                _typingTimer = new DispatcherTimer();
                _typingTimer.Interval = TimeSpan.FromMilliseconds(500);

                _typingTimer.Tick += new EventHandler(this.HandleTypingTimerTimeout);
            }

            _typingTimer.Stop(); // Resets the timer
            _typingTimer.Start();

        }

        private void HandleTypingTimerTimeout(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;

            if (timer == null)
            {
                return;
            }

            // Perform search when filter is changed 
            if (!_oldFilter.Compare(_filter))
            {
                FillGrid();
            }

            // The timer must be stopped! We want to act only once per keystroke.
            timer.Stop();
        }

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            _filter = new FilterVO();
            _oldFilter = new FilterVO();
            DataContext = _filter;
            FillComboLookup(ComboType, "Type", "Semua Tipe");   
            FillComboCity();
            FillGrid();
        }
    }
}
