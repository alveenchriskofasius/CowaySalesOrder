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
using System.Windows.Threading;

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for OrderGridUI.xaml
    /// </summary>
    public partial class OrderGridUI : BaseUI
    {
        FilterVO _filter;
        FilterVO _oldFilter;
        public OrderGridUI()
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
        DispatcherTimer _typingTimer;

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
        private void TextNo_TextChanged(object sender, TextChangedEventArgs e)
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
        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Name)
            {
                case "ButtonReset":
                    FilterVO newFilter = new FilterVO();
                    _filter.SetValue(newFilter);
                    FillGrid(); // apply filters
                    break;

                case "ButtonRefresh":
                    FillGrid();
                    break;

            }
        }
        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Perform search when filter is changed 
            if (!_oldFilter.Compare(_filter))
            {
                FillGrid();
            }
        }
        public async void FillGrid()
        {
            Main.ShowLoading(Loading.Loading, this);
            try
            {
                GridOrder.ItemsSource = await OrderProxy.Data(_filter);
                _oldFilter.SetValue(_filter);
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal Tarik data", ex.Message);
            }
            Main.HideLoading();
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FillGrid();
        }
        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            _filter = new FilterVO();
            _oldFilter = new FilterVO();
            DataContext = _filter;
            FillComboLookup(ComboStatus, "Status", "Semua Status");
            FillComboCustomer(ComboCustomer,true);
        }

        private void GridOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OrderVO order = GridOrder.SelectedItem as OrderVO;
            if (order != null)
            {
                OnRowDoubleClicked(new DataChangedEventArgs(order));
            }
        }
    }
}
