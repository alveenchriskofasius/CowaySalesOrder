using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using CSO.Proxy;
using CSO.VO;
namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for DashboardSelectorUI.xaml
    /// </summary>
    public partial class DashboardSelectorUI : BaseUI
    {
        FilterVO _filter;
        FilterVO _oldFilter;
        OrderVO _order;
        int _primaryStatusID;
        int _secondaryStatusID;

        private enum Status { Open = 1, Verified, Approved, Completed, Canceled, Rejected, Refund, OnCallProcess, CTAssign, Process, CTAssigned };
        private enum DeliveryStatus { Completed, Canceled, Process }
        public DashboardSelectorUI()
        {
            InitializeComponent();
        }
        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            _filter = new FilterVO();
            _oldFilter = new FilterVO();
            _order = new OrderVO();
            DataContext = _filter;
            ComboAssign.DataContext = _order;
            RadioOpen.IsChecked = true;
            FillGrid();
            FillComboAssign();
            FillComboCustomer(ComboCustomer, true);
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
        private void Update(int statusID)
        {
            List<OrderVO> orders = GridCustomer.ItemsSource as List<OrderVO>;
            bool canUpdate = false;

            if (orders.Count == 0) { return; }
            foreach (OrderVO order in orders)
            {
                if (order.Selected)
                {
                    canUpdate = true;
                }
            }
            if (canUpdate)
            {
                try
                {
                    OrderProxy.UpdateStatus(orders, statusID, _order.AssignID);
                    Main.ShowMessage("Status berhasil diupdate");
                    FillGrid();
                }
                catch (Exception e)
                {
                    Main.ShowMessage("Status gagal diupdate", e.Message, Message.Error);

                }
            }
            UnCheck();
        }
        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Name)
            {

                case "ButtonPrimaryStatus":
                    Update(_primaryStatusID);
                    break;
                case "ButtonSecondaryStatus":
                    Update(_secondaryStatusID);
                    break;
                case "ButtonPhone":
                    Update(_primaryStatusID);
                    break;
                case "ButtonCancel":
                    Update(_secondaryStatusID);
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
        private void FillComboAssign()
        {
            try
            {
                ComboAssign.ItemsSource = UserProxy.Data().Where(x => x.Role("CT")).ToList();
                ComboAssign.SelectedValuePath = "ID";
                ComboAssign.DisplayMemberPath = "FullName";
            }
            catch (Exception)
            {

                throw;
            }
        }
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
        private void Text_TextChanged(object sender, TextChangedEventArgs e)
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Perform search when filter is changed 
            if (!_oldFilter.Compare(_filter))
            {
                FillGrid();
            }
        }

        private void Filter_LostFocus(object sender, RoutedEventArgs e)
        {
            // Perform search when filter is changed 
            if (!_oldFilter.Compare(_filter))
            {
                FillGrid();
            }
        }
        private void SetFilters()
        {
            _filter.StatusList = String.Empty;
            if (RadioOpen.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Open;
            }
            else if (RadioVerified.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Verified;

            }
            else if (RadioApproved.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Approved;

            }
            else if (RadioProcess.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Process;

            }
            else if (RadioPhoneProcess.IsChecked == true)
            {
                _filter.StatusList += (int)Status.OnCallProcess;
            }
            else if (RadioCTAssign.IsChecked == true)
            {
                _filter.StatusList += (int)Status.CTAssign + "," + (int)Status.CTAssigned;

            }
            else if (RadioCompleted.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Completed;

            }
            else if (RadioRejected.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Rejected;
            }
            else if (RadioRefund.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Refund;
            }
            else if (RadioCanceled.IsChecked == true)
            {
                _filter.StatusList += (int)Status.Canceled;
            }
        }
        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            ColumnVerificationOn.Width = ColumnVerificationBy.Width = ColumnApprovedOn.Width = ColumnApprovedBy.Width =
            ColumnCompletedOn.Width = ColumnCompletedBy.Width = ColumnRejectedOn.Width = ColumnRejectedBy.Width =
            ColumnRefundOn.Width = ColumnRefundBy.Width = ColumnCancelOn.Width = ColumnCancelBy.Width = ColumnAssign.Width =
            ColumnAssignOn.Width = ColumnAssignBy.Width = 0;
            ButtonSecondaryStatus.Visibility = ButtonSecondaryStatus.Visibility =
            ButtonPrimaryStatus.Visibility = BorderPrimaryStatus.Visibility = BorderSecondaryStatus.Visibility = PanelAssign.Visibility =
            ButtonCancel.Visibility = BorderCancel.Visibility = ButtonPhone.Visibility = BorderPhone.Visibility = Visibility.Collapsed;

            ButtonPrimaryStatus.IsEnabled = ButtonSecondaryStatus.IsEnabled = ButtonPhone.IsEnabled = ButtonCancel.IsEnabled = false;
            _filter.StatusList = String.Empty;
            switch (radio.Name)
            {
                case "RadioOpen":
                    _filter.StatusList += (int)Status.Open;
                    _primaryStatusID = (int)Status.Verified;
                    ButtonPrimaryStatus.Content = "Verifikasi";
                    ButtonPrimaryStatus.Visibility = BorderPrimaryStatus.Visibility = UserProxy.CurrentUser.Role("Admin Level 1") ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "RadioVerified":
                    _filter.StatusList += (int)Status.Verified;
                    _primaryStatusID = (int)Status.Approved; //Approved
                    _secondaryStatusID = (int)Status.Rejected; //Rejected
                    ColumnVerificationOn.Width = ColumnVerificationBy.Width = UserProxy.CurrentUser.Role("Admin Level 2") ? new DataGridLength(1, DataGridLengthUnitType.Auto) : 0;
                    ButtonPrimaryStatus.Content = "Setujui";
                    ButtonSecondaryStatus.Content = "Tolak";
                    ButtonPrimaryStatus.Visibility = BorderPrimaryStatus.Visibility = ButtonSecondaryStatus.Visibility = BorderSecondaryStatus.Visibility
                        = UserProxy.CurrentUser.Role("Admin Level 2") ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "RadioApproved":
                    _filter.StatusList += (int)Status.Approved;
                    _primaryStatusID = (int)Status.Process;
                    ColumnVerificationOn.Width = ColumnVerificationBy.Width = ColumnApprovedOn.Width = ColumnApprovedBy.Width =
                         UserProxy.CurrentUser.Role("Admin Level 1") && GridCustomer.Items.Count > 0 ? new DataGridLength(1, DataGridLengthUnitType.Auto) : 0;
                    ButtonPrimaryStatus.Content = "Proses";
                    ButtonPrimaryStatus.Visibility = BorderPrimaryStatus.Visibility = UserProxy.CurrentUser.Role("Admin Level 1") ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "RadioProcess":
                    _filter.StatusList += (int)Status.Process;
                    _primaryStatusID = (int)Status.OnCallProcess;
                    ButtonPhone.Visibility = BorderPhone.Visibility = UserProxy.CurrentUser.Role("Admin Level 1") ? Visibility.Visible: Visibility.Collapsed;
                    break;
                case "RadioPhoneProcess":
                    _filter.StatusList += (int)Status.OnCallProcess;
                    _primaryStatusID = (int)Status.CTAssign;
                    _secondaryStatusID = (int)Status.Refund;
                    ButtonPrimaryStatus.Content = "Verifikasi berhasil";
                    ButtonCancel.Content = "Verifikasi gagal";
                    ButtonPrimaryStatus.Visibility = BorderPrimaryStatus.Visibility = ButtonCancel.Visibility = BorderCancel.Visibility =
                    UserProxy.CurrentUser.Role("Admin Level 1") ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "RadioCTAssign":
                    _filter.StatusList += (int)Status.CTAssign + "," + (int)Status.CTAssigned;
                    _primaryStatusID = (int)Status.Completed;
                    _secondaryStatusID = (int)Status.Refund;
                    ButtonPrimaryStatus.Content = "Install berhasil";
                    ButtonCancel.Content = "Install gagal";
                    ButtonPrimaryStatus.Visibility = BorderPrimaryStatus.Visibility = ButtonCancel.Visibility = BorderCancel.Visibility =
                    PanelAssign.Visibility = UserProxy.CurrentUser.Role("Technician") ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "RadioCompleted":
                    _filter.StatusList += (int)Status.Completed;
                    ColumnVerificationOn.Width = ColumnVerificationBy.Width = ColumnApprovedOn.Width = ColumnApprovedBy.Width =
                    ColumnCompletedOn.Width = ColumnCompletedBy.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);

                    break;
                case "RadioRejected":
                    _filter.StatusList += (int)Status.Rejected;
                    _secondaryStatusID = (int)Status.Refund;
                    ButtonCancel.Content = "Proses Refund";
                    ButtonCancel.Visibility = UserProxy.CurrentUser.Role("Admin Level 1") ? Visibility.Visible : Visibility.Collapsed;
                    ColumnRejectedOn.Width = ColumnRejectedBy.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
                    break;
                case "RadioRefund":
                    _filter.StatusList += (int)Status.Refund;
                    _secondaryStatusID = (int)Status.Canceled;
                    ButtonCancel.Content = "Proses Cancel";
                    ButtonCancel.Visibility = UserProxy.CurrentUser.Role("Accounts Payable") ? Visibility.Visible : Visibility.Collapsed;
                    ColumnRejectedOn.Width = ColumnRejectedBy.Width = ColumnRefundOn.Width = ColumnRefundBy.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
                    break;
                case "RadioCanceled":
                    _filter.StatusList += (int)Status.Canceled;
                    ColumnVerificationOn.Width = ColumnVerificationBy.Width = ColumnApprovedOn.Width = ColumnApprovedBy.Width =
                    ColumnCancelOn.Width = ColumnCancelBy.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);

                    break;

            }
            //Perform search when filter is changed
            if (!_oldFilter.Compare(_filter))
            {
                FillGrid();
            }


        }
        private bool IsStatusUniform(string status, string role)
        {
            try
            {
                string Status = status;
                bool isValid = false;
                _filter.StatusList = String.Empty;
                foreach (OrderVO order in GridCustomer.ItemsSource)
                {
                    if (order.Selected == true)
                    {

                        if (order.Status == Status && UserProxy.CurrentUser.Role(role))
                        {
                            _filter.StatusList += order.Status + ", ";
                            isValid = true;
                        }
                        else
                        {
                            if (Status != order.Status && !UserProxy.CurrentUser.Role(role)) { isValid = false; return isValid; }
                        }

                    }
                }
                return isValid;

            }
            catch (Exception ex)
            {

                Main.ShowMessage("Checkmark gagal", ex.Message);
                return false;
            }
        }
        #region Checkbox
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (RadioOpen.IsChecked == true)
            {
                ButtonPrimaryStatus.IsEnabled = IsStatusUniform("Open", "Admin Level 1");
            }
            else if (RadioVerified.IsChecked == true)
            {
                ButtonPrimaryStatus.IsEnabled = ButtonSecondaryStatus.IsEnabled = IsStatusUniform("Verified", "Admin Level 2");
            }
            else if (RadioApproved.IsChecked == true)
            {
                ButtonPrimaryStatus.IsEnabled = IsStatusUniform("Approved", "Admin Level 1");
            }
            else if (RadioProcess.IsChecked == true)
            {
                ButtonPhone.IsEnabled = IsStatusUniform("Process", "Admin Level 1");

            }
            else if (RadioPhoneProcess.IsChecked == true)
            {
                ButtonPrimaryStatus.IsEnabled = ButtonCancel.IsEnabled = IsStatusUniform("On Call Process", "Admin Level 1");
            }
            else if (RadioCTAssign.IsChecked == true)
            {
                ButtonPrimaryStatus.IsEnabled = ButtonCancel.IsEnabled = IsStatusUniform("CT Assigned", "Technician");
            }
            else if (RadioRejected.IsChecked == true)
            {
                ButtonCancel.IsEnabled = IsStatusUniform("Rejected", "Admin Level 1");
            }
            else if (RadioRefund.IsChecked == true)
            {
                ButtonCancel.IsEnabled = IsStatusUniform("Refund", "Accounts Payable");
            }
        }

        #endregion

        private void UnCheck()
        {

            // Deselect all rows
            CheckBox checkRowSelect = (CheckBox)Tool.UIHelper.GetDescendantByType(GridCustomer, typeof(CheckBox));
            // if rowheader select is checked (all rows selected), deselect all rows
            if (checkRowSelect.IsChecked == true)
            {
                checkRowSelect.IsChecked = false;
            }
            // else deselect one row
            else
            {
                foreach (OrderVO order in GridCustomer.ItemsSource)
                {
                    order.Selected = false;
                }
            }
        }
        private void CheckBoxAll_Checked(object sender, RoutedEventArgs e)
        {
            List<OrderVO> orders = GridCustomer.ItemsSource as List<OrderVO>;

            if (orders == null) return;

            foreach (OrderVO order in orders)
            {
                order.Selected = (sender as CheckBox).IsChecked == true;
            }
        }

        public async void FillGrid()
        {
            Main.ShowLoading(Loading.Loading, this);
            SetFilters();
            try
            {
                GridCustomer.ItemsSource = await OrderProxy.DataDashboard(_filter);

                //update old filter
                _oldFilter.SetValue(_filter);
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data ", ex.Message);
            }

            Main.HideLoading();

        }

        private void Assign_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool canAssign = false;
            if (GridCustomer.Items.Count > 0)
            {
                List<OrderVO> orders = GridCustomer.ItemsSource as List<OrderVO>;
                foreach (OrderVO order in orders)
                {
                    if (order.Selected) { canAssign = true; break; }

                }
                e.CanExecute = canAssign && _filter.StatusList != "11" && UserProxy.CurrentUser.Role("Technician") && _order.AssignID != 0;
                e.Handled = true;
            }
        }

        private void Assign_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            Update((int)Status.CTAssigned);
        }

        private void GridCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrderVO order = GridCustomer.SelectedItem as OrderVO;
            if (order != null)
            {
                _order.SetValue(order);
            }
        }

        private void GridCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OrderVO order = GridCustomer.SelectedItem as OrderVO;
            if (order != null)
            {
                OnRowDoubleClicked(new DataChangedEventArgs(order));
            }
        }
    }
}
