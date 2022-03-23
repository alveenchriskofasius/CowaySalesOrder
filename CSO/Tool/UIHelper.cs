using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CSO.Tool
{
    public static class UIHelper
    {
        public static void WalkLogicalTree(List<RadioButton> radioButtons, object parent)
        {
            DependencyObject doParent = parent as DependencyObject;
            if (doParent == null) return;
            foreach (object child in LogicalTreeHelper.GetChildren(doParent))
            {
                if (child is RadioButton)
                {
                    radioButtons.Add(child as RadioButton);
                }
                WalkLogicalTree(radioButtons, child);
            }
        }

        public static Visual GetAscendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;

            Visual foundElement = null;

            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            Visual visual = VisualTreeHelper.GetParent(element) as Visual;

            foundElement = GetAscendantByType(visual, type);
            return foundElement;
        }

        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;

            Visual foundElement = null;

            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                //Console.WriteLine(visual);
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }

            return foundElement;

        }

        public static Visual GetDescendantsByType(List<Visual> elements, Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;

            Visual foundElement = null;

            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantsByType(elements, visual, type);
                if (foundElement != null)
                {
                    elements.Add(foundElement);
                    break;
                    //foundElement = null;
                }
            }

            return null;

        }

        public static Visual GetParent(Visual element, Type type)
        {
            while (element != null)
            {
                element = VisualTreeHelper.GetParent(element) as Visual;
                if (element == null || element.GetType() == type)
                    break;
            }
            return element;
        }

        public static decimal TruncateDecimal(decimal d, int decimals = 3)
        {
            if (decimals < 0)
                throw new ArgumentOutOfRangeException("decimals", "Value must be in range 0-28.");
            else if (decimals > 28)
                throw new ArgumentOutOfRangeException("decimals", "Value must be in range 0-28.");
            else if (decimals == 0)
                return Math.Truncate(d);
            else
            {
                decimal integerPart = Math.Truncate(d);
                decimal scalingFactor = d - integerPart;
                decimal multiplier = (decimal)Math.Pow(10, decimals);

                scalingFactor = Math.Truncate(scalingFactor * multiplier) / multiplier;

                return integerPart + scalingFactor;
            }
        }

        public static decimal ConvertToDecimal(object value)
        {

            decimal decimalValue = 0;
            if (value != null)
            {
                if (value.GetType() == typeof(decimal))
                {
                    decimalValue = (decimal)value;
                }
                else
                {
                    decimal.TryParse(value.ToString(), out decimalValue);
                }
            }
            return decimalValue;

        }

        public async static void SelectFirstRow(DataGrid dataGrid)
        {
            // Select first row
            if (dataGrid.Items.Count > 0)
            {
                int index = 0;

                // Select the row
                dataGrid.SelectedIndex = index;
                dataGrid.UpdateLayout();
                dataGrid.SelectedItem = dataGrid.Items[index];
                dataGrid.ScrollIntoView(dataGrid.Items[index]);

                // Await before selecting cell
                await Task.Delay(100);

                if (dataGrid.Items.Count > 0)
                {
                    DataGridRow dgrow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.Items[index]);

                    // Select the first cell
                    if (dgrow != null && dataGrid.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader)
                    {
                        for (int column = 0; column < dataGrid.Columns.Count(); column++)
                        {
                            if (dataGrid.Columns[column].Visibility == Visibility.Visible && !dataGrid.Columns[column].IsReadOnly)
                            {
                                DataGridCell cell = GetCell(dataGrid, dgrow, column);
                                if (cell != null) cell.Focus();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void ScrollToBottom(DataGrid dataGrid)
        {
            ScrollViewer scrollViewer = (ScrollViewer)CSO.Tool.UIHelper.GetDescendantByType(dataGrid, typeof(ScrollViewer));
            if (scrollViewer != null)
            {

                if (dataGrid.Items.Count > 0)
                {
                    dataGrid.SelectedIndex = dataGrid.Items.Count - 1;
                    dataGrid.ScrollIntoView(dataGrid.Items[dataGrid.SelectedIndex]);
                    scrollViewer.ScrollToEnd();
                }
            }
        }

        private static System.Windows.Controls.DataGridCell GetDataGridCell(System.Windows.Controls.DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cellContent != null)
                return ((System.Windows.Controls.DataGridCell)cellContent.Parent);

            return (null);
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method 
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        public static DateTime TimeNow(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        }

        public static void PlaySound(string name)
        {
            string path = System.IO.Path.GetFullPath("Resource/Sound/" + name + ".wav");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
            player.Load();
            player.Play();
        }
    }

    public static class DataGridBehavior
    {
        #region DisplayRowNumber

        public static DependencyProperty DisplayRowNumberProperty =
            DependencyProperty.RegisterAttached("DisplayRowNumber",
                                                typeof(bool),
                                                typeof(DataGridBehavior),
                                                new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));

        public static bool GetDisplayRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(DisplayRowNumberProperty);
        }
        public static void SetDisplayRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(DisplayRowNumberProperty, value);
        }

        private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = target as DataGrid;
            if ((bool)e.NewValue == true)
            {
                EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
                loadedRowHandler = (object sender, DataGridRowEventArgs ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.LoadingRow -= loadedRowHandler;
                        return;
                    }
                    ea.Row.Header = ea.Row.GetIndex() + 1;
                };
                dataGrid.LoadingRow += loadedRowHandler;

                ItemsChangedEventHandler itemsChangedHandler = null;
                itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
                        return;
                    }
                    GetVisualChildCollection<DataGridRow>(dataGrid).
                        ForEach(d => d.Header = d.GetIndex() + 1);
                };
                dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
            }
        }

        #endregion // DisplayRowNumber

        #region Get Visuals

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        #endregion // Get Visuals

    }
}
