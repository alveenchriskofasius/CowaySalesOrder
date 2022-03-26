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
using CSO.Tool;
using System.Reflection;

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for UserFormUI.xaml
    /// </summary>
    public partial class UserFormUI : BaseUI
    {
        private int _noOfErrorsOnScreen;
        UserVO _user;
        public UserFormUI()
        {
            InitializeComponent();
        }
        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            _user = new UserVO();
            DataContext = _user;
            FillForm(null);
        }
        #region Event declarations
        public event EventHandler<EventArgs> DataChange;
        protected virtual void OnDataChange(EventArgs e)
        {
            EventHandler<EventArgs> handler = DataChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }
        public void FillForm(UserVO user)
        {

            // new record
            if (user == null || user.ID == 0)
            {
                //int UserID = 0;
                user = new UserVO();
            }

            List<RoleVO> roles = ListboxRole.ItemsSource as List<RoleVO>;
            FillRole(roles, user.ID);

            _user.SetValue(user);

            // Set form (password cannot be bound)
            TextPassword.Password = user.Password;

            // Set radio button
            List<RadioButton> radioButtons = new List<RadioButton>();
            UIHelper.WalkLogicalTree(radioButtons, PanelGender);
            foreach (RadioButton rb in radioButtons)
            {
                if (rb.GroupName == "Gender")
                {
                    rb.IsChecked = rb.Content.ToString() == user.Gender;
                }
            }

            if (user.ID == 0)
            {
                TextFullName.Focus();
            }

            ButtonDelete.IsEnabled = user.ID != 0;
        }
        public void ListboxRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserGridUI userGrid = new UserGridUI();
            UserVO user = userGrid.GridUser.SelectedItem as UserVO;
            int UserID = user.ID;
            UserVO users = new UserVO();
            if (users == null || users != null)
            {
                List<RoleVO> roles = ListboxRole.ItemsSource as List<RoleVO>;
                FillRole(roles, UserID);
            }
        }
        private void GenderButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton gender = sender as RadioButton;
            _user.Gender = gender.Content.ToString();

        }
        public void FillRole(List<RoleVO> roles, int UserID)
        {
            try
            {
                roles = RoleProxy.Data(UserID);
                ListboxRole.ItemsSource = roles;
                ListboxRole.DisplayMemberPath = "Name";
                ListboxRole.SelectedValuePath = "ID";
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data Role", ex.Message);
            }
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
                    if (_user.ID == 0)
                    {
                        return;
                    }

                    if (MessageBox.Show("Hapus user " + _user.FullName + "?", "Konfirmasi hapus ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            UserProxy.Delete(_user);
                            // Notify deleted data
                            OnDataChange(new EventArgs());

                            // Display message
                            Main.ShowMessage("Data berhasil dihapus ");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Data gagal dihapus  ", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    break;

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
        private async void Save()
        {
            // Animate button pressed
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ButtonSave, new object[] { true });
            await Task.Delay(75);
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ButtonSave, new object[] { false });
            if (Main.IsLoading) return;

            Main.ShowLoading(Loading.Saving, this);

            try
            {
                List<RoleVO> roles = ListboxRole.ItemsSource as List<RoleVO>;
                UserProxy.Save(ref _user, roles);
                // Reset form
                FillForm(_user);
                OnDataChange(new EventArgs());
                // Show message
                Main.ShowMessage("Data berhasil disimpan");

            }
            catch (Exception ex)
            {
                Main.ShowMessage("Data gagal disimpan", ex.Message);
            }
            Main.HideLoading();
        }
        #endregion
        private void TextPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox.Password != _user.Password)
            {
                _user.Password = passwordBox.Password;
            }
        }
    }
}
