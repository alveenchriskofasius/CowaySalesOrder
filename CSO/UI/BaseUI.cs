using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CSO.Proxy;
using CSO.VO;
using UICustom;
using WindowsInput;
using WindowsInput.Native;

namespace CSO.UI
{

    public enum Message { Normal = 0, Error, Info };
    public enum Loading { Loading = 0, Saving, Deleting };

    public interface IBaseUI
    {
        void ShowMessage(string message, Message type = Message.Normal);
        void ShowMessage(string message, string error);
        void ShowMessage(string message, string error, Message type);
        void ShowLoading(Loading loading, BaseUI panel);
        void HideLoading();
        void ShowOverlay();
        void HideOverlay();

        string LoadingText { set; }
        bool IsLoading { get; set; }
    }

    public class BaseUI : UserControl
    {
        private DataGridSelectionMode _gridSelectionMode = DataGridSelectionMode.Single;

        public IBaseUI Main { get; set; }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BaseUI),
            new PropertyMetadata(""));

        protected virtual void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // bypass keypress handler: keypressed is handled in filtered combo box
                FilteredComboBox comboBox = Tool.UIHelper.GetParent(e.OriginalSource as Visual, typeof(FilteredComboBox)) as FilteredComboBox;
                if (comboBox != null)
                {
                    return;
                }

                DataGrid dataGrid = e.Source as DataGrid;

                if ((e.Key == Key.Enter || e.Key == Key.Tab) && e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    // Count visible columns & index
                    int visibleColumn = 0;
                    int visibleIndex = 0;

                    for (int column = 0; column < dataGrid.Columns.Count(); column++)
                    {
                        if (dataGrid.Columns[column].Visibility == Visibility.Visible && !dataGrid.Columns[column].IsReadOnly)
                        {
                            visibleColumn++;
                            if (column <= dataGrid.CurrentColumn.DisplayIndex)
                            {
                                visibleIndex++;
                            }
                        }
                    }

                    if (visibleIndex == visibleColumn)
                    {
                        ItemContainerGenerator icg = dataGrid.ItemContainerGenerator;
                        if (dataGrid.Items.IndexOf(dataGrid.CurrentItem) == icg.Items.Count - 2)
                        {
                            //Console.WriteLine("Commit Editing");
                            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                        }
                    }

                    if (e.Key == Key.Enter)
                    {
                        e.Handled = true;
                        InputSimulator sim = new InputSimulator();
                        //Console.WriteLine("Enter Pressed");
                        sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    }

                    if (e.Key == Key.Tab)
                    {
                        //Console.WriteLine("Begin Edit");
                        //dataGrid.BeginEdit();
                    }

                }

                // selecting grid data
                else if (e.Key == Key.A && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    _gridSelectionMode = dataGrid.SelectionMode;
                    dataGrid.SelectionMode = DataGridSelectionMode.Extended;
                    dataGrid.SelectAll();
                }

                //// copying grid data
                else if (e.Key == Key.C && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    if (dataGrid.SelectionMode == DataGridSelectionMode.Extended || dataGrid.SelectedItems.Count > 1)
                    {
                        dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                    }
                    else
                    {
                        dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                    }
                }

                // do navigation as usual
                else if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
                {
                }

                // keys to ignore
                else if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.F11 || e.Key == Key.Delete)
                {
                }
                else if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    dataGrid.BeginEdit();
                }
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Error", ex.Message);
            }

        }

        protected virtual void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            DataGrid dataGrid = e.Source as DataGrid;

            if (e.Key == Key.C && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                // restore selection mode
                dataGrid.SelectionMode = _gridSelectionMode;
                Main.ShowMessage("Data sudah dicopy ke clipboard", Message.Info);
            }
        }

        protected virtual void Grid_LostFocus(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader)
            {
                dataGrid.UnselectAllCells();
            }
        }

        public bool IsDataGridCellEditing { get; set; }

        protected virtual void Grid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsDataGridCellEditing = true;
        }

        protected virtual void Grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsDataGridCellEditing = false;
        }

        public void Global_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Ignore certain keys (F11, modifiers: CTRL, SHIFT, ALT)
            if (e.Key == Key.F11 || e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                return;
            }

            // Special key handler for combobox
            if (sender.GetType() == typeof(ComboBox))
            {
                ComboBox comboBox = sender as ComboBox;
                if (e.Key == Key.Enter)
                {
                    // close drop down as user press Enter
                    comboBox.IsDropDownOpen = false;
                    e.Handled = true;
                }
                else
                {
                    // drop down combo as user starts typing
                    comboBox.IsDropDownOpen = true;
                }
            }

            UIElement uiElement = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uiElement.MoveFocus(
                new TraversalRequest(
                FocusNavigationDirection.Next));
            }

        }
        public class DataChangedEventArgs : EventArgs
        {
            public object Entity { get; set; }
            public object Sender { get; set; }

            public DataChangedEventArgs() { }

            public DataChangedEventArgs(object entity)
            {
                Entity = entity;
            }
            public DataChangedEventArgs(object entity, object sender)
            {
                Entity = entity;
                Sender = sender;
            }

        }
        #region FillCombo
        protected void FillComboLookup(ComboBox combo, string entity, string withAll = null)
        {
            List<LookupVO> lookups;
            try
            {
                lookups = LookupProxy.Data(entity, withAll);
                combo.ItemsSource = lookups;
                combo.SelectedValuePath = "ID";
                combo.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data", ex.Message);
            }
        }
        protected void FillComboProduct(ComboBox combo, bool isDiscount = false, int typeID = 0, decimal discount = 0)
        {
            List<ProductVO> products;
            try
            {
                products = ProductProxy.Data(isDiscount, typeID, discount);
                combo.ItemsSource = products;
                combo.SelectedValuePath = "ID";
                combo.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data Produk", ex.Message);
            }
        }
        protected async void FillComboCustomer(ComboBox combo)
        {
            List<CustomerVO> customers;
            try
            {
                customers = await CustomerProxy.Data(false);
                combo.ItemsSource = customers;
                combo.SelectedValuePath = "ID";
                combo.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data Produk", ex.Message);
            }
        }
        protected void FillComboPromotion(ComboBox combo)
        {
            List<PromotionVO> promotions;
            try
            {
                promotions = PromotionProxy.Data();
                combo.ItemsSource = promotions;
                combo.SelectedValuePath = "ID";
                combo.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                Main.ShowMessage("Gagal tarik data Produk", ex.Message);
            }
        }
        #endregion
    }

}
