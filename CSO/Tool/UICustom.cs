using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

/* Taken from:
 
    http://dotbay.blogspot.com/2009/04/building-filtered-combobox-for-wpf.html

*/

namespace UICustom
{
    public class ImageButton : Button
    {
        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        #region Dependency Properties

        public double ImageSize
        {
            get { return (double)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        public static readonly DependencyProperty ImageSizeProperty =
            DependencyProperty.Register("ImageSize", typeof(double), typeof(ImageButton),
            new FrameworkPropertyMetadata(30.0, FrameworkPropertyMetadataOptions.AffectsRender, ImageSizeChanged));

        public string NormalImage
        {
            get { return (string)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); SetValue(NormalImageProperty, value); }
        }

        public static readonly DependencyProperty NormalImageProperty =
            DependencyProperty.Register("NormalImage", typeof(string), typeof(ImageButton),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        public string HoverImage
        {
            get { return (string)GetValue(HoverImageProperty); }
            set { SetValue(HoverImageProperty, value); }
        }

        public static readonly DependencyProperty HoverImageProperty =
            DependencyProperty.Register("HoverImage", typeof(string), typeof(ImageButton),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        public string PressedImage
        {
            get { return (string)GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }

        public static readonly DependencyProperty PressedImageProperty =
            DependencyProperty.Register("PressedImage", typeof(string), typeof(ImageButton),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        public string DisabledImage
        {
            get { return (string)GetValue(DisabledImageProperty); }
            set { SetValue(DisabledImageProperty, value); }
        }

        public static readonly DependencyProperty DisabledImageProperty =
            DependencyProperty.Register("DisabledImage", typeof(string), typeof(ImageButton),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, ImageSourceChanged));

        private static void ImageSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                Application.GetResourceStream(new Uri("pack://application:,,," + (string)e.NewValue));

        }

        private static void ImageSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion
    }

    /// <summary>
    /// Editable combo box which uses the text in its editable textbox to perform a lookup
    /// in its data source.
    /// </summary>
    public class FilteredComboBox : ComboBox
    {

        public enum SearchMode
        {
            /// <summary>
            /// StartsWith
            /// </summary>
            StartsWith,

            /// <summary>
            /// Contains 
            /// </summary>
            Contains,
        }

        ////
        // Public Fields
        ////

        /// <summary>
        /// The search string treshold length.
        /// </summary>
        /// <remarks>
        /// It's implemented as a Dependency Property, so you can set it in a XAML template 
        /// </remarks>
        public static readonly DependencyProperty MinimumSearchLengthProperty =
                DependencyProperty.Register(
                    "MinimumSearchLength",
                    typeof(int),
                    typeof(FilteredComboBox),
                    new UIPropertyMetadata(1));

        /// <summary>
        /// Search mode.
        /// </summary>
        /// <remarks>
        /// It's implemented as a Dependency Property, so you can set it in a XAML template 
        /// </remarks>
        public static readonly DependencyProperty TextSearchModeProperty =
            DependencyProperty.Register(
                "TextSearchMode",
                typeof(SearchMode),
                typeof(FilteredComboBox),
                new UIPropertyMetadata(UICustom.FilteredComboBox.SearchMode.StartsWith));

        ////
        // Private Fields
        //// 

        /// <summary>
        /// Caches the previous value of the filter.
        /// </summary>
        private string oldFilter = string.Empty;

        /// <summary>
        /// Holds the current value of the filter.
        /// </summary>
        private string currentFilter = string.Empty;

        ////
        // Constructors
        //// 

        /// <summary>
        /// Initializes a new instance of the FilteredComboBox class.
        /// </summary>
        /// <remarks>
        /// You could set 'IsTextSearchEnabled' to 'false' here,
        /// to avoid non-intuitive behavior of the control
        /// </remarks>
        public FilteredComboBox()
        {
            IsTextSearchEnabled = false;
        }

        ////
        // Properties
        //// 

        /// <summary>
        /// Gets or sets the search string treshold length.
        /// </summary>
        /// <value>The minimum length of the search string that triggers filtering.</value>
        [Description("Length of the search string that triggers filtering.")]
        [Category("Filtered ComboBox")]
        [DefaultValue(1)]
        public int MinimumSearchLength
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (int)this.GetValue(MinimumSearchLengthProperty);
            }

            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                this.SetValue(MinimumSearchLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the search mode (Starts With vs Contains).
        /// </summary>
        /// <value>Search Mode.</value>
        [Description("Search mode. StarsWith or Contains")]
        [Category("Filtered ComboBox")]
        [DefaultValue(SearchMode.StartsWith)]
        public SearchMode TextSearchMode
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return (SearchMode)this.GetValue(TextSearchModeProperty);
            }

            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                this.SetValue(TextSearchModeProperty, value);
            }
        }


        /// <summary>
        /// Gets a reference to the internal editable textbox.
        /// </summary>
        /// <value>A reference to the internal editable textbox.</value>
        /// <remarks>
        /// We need this to get access to the Selection.
        /// </remarks>
        protected TextBox EditableTextBox
        {
            get
            {
                return this.GetTemplateChild("PART_EditableTextBox") as TextBox;
            }
        }

        ////
        // Event Raiser Overrides
        //// 

        /// <summary>
        /// Keep the filter if the ItemsSource is explicitly changed.
        /// </summary>
        /// <param name="oldValue">The previous value of the filter.</param>
        /// <param name="newValue">The current value of the filter.</param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(newValue);
                view.Filter += this.FilterPredicate;
            }

            if (oldValue != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(oldValue);
                view.Filter -= this.FilterPredicate;
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Confirm or cancel the selection when Tab, Enter, or Escape are hit. 
        /// Open the DropDown when the Down Arrow is hit.
        /// </summary>
        /// <param name="e">Key Event Args.</param>
        /// <remarks>
        /// The 'KeyDown' event is not raised for Arrows, Tab and Enter keys.
        /// It is swallowed by the DropDown if it's open.
        /// So use the Preview instead.
        /// </remarks>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                // Explicit Selection -> Close ItemsPanel
                this.IsDropDownOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                if (IsDropDownOpen && SelectedIndex >= 0)
                {
                    IsDropDownOpen = false;
                }
                else if (!IsDropDownOpen && SelectedIndex >= 0)
                {
                    // continue
                }
                else
                {
                    SelectedIndex = 0; // move down
                    e.Handled = true;
                }

            }
            else if (e.Key == Key.Escape)
            {
                // Escape -> Close DropDown and redisplay Filter
                this.IsDropDownOpen = false;
                this.SelectedIndex = -1;
                //this.EditableTextBox.Focus();
                this.Text = this.currentFilter;
            }
            else
            {
                if (e.Key == Key.Down)
                {
                    // Arrow Down -> Open DropDown
                    this.IsDropDownOpen = true;
                }

                base.OnPreviewKeyDown(e);
            }

            // Cache text
            this.oldFilter = this.Text;
        }

        /// <summary>
        /// Modify and apply the filter.
        /// </summary>
        /// <param name="e">Key Event Args.</param>
        /// <remarks>
        /// Alternatively, you could react on 'OnTextChanged', but navigating through 
        /// the DropDown will also change the text.
        /// </remarks>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                // Navigation keys are ignored
                if (this.SelectedIndex == -1 && e.Key == Key.Down)
                {
                    // force selection, because sometimes the key down is hooked with the scrollbar control,
                    // not sure how to release it
                    this.SelectedIndex = 0;
                }
            }
            else if (e.Key == Key.Tab)
            {
                // Explicit Select -> Clear Filter
                this.ClearFilter();
                //Console.WriteLine("clear filter");
            }
            else if (e.Key == Key.Enter)
            {
                if (this.SelectedIndex > 0)
                {
                    this.ClearFilter();
                }
            }
            else
            {
                // The text was changed
                if (this.Text != this.oldFilter)
                {
                    // Clear the filter if the text is empty,
                    // apply the filter if the text is long enough
                    if (this.Text.Length == 0 || this.Text.Length >= this.MinimumSearchLength)
                    {
                        // If an item is selected, and user is typing new string
                        // Must reset the selected index, or otherwise the text will be overwritten with the selected index 
                        // when opening the drop down list
                        //Console.WriteLine("reset filter");
                        string temp = this.Text; // save
                        this.RefreshFilter();
                        if (this.SelectedIndex > 0)
                        {
                            this.SelectedIndex = -1;
                            //EditableTextBox.Focus();
                        }
                        this.Text = temp; // restore
                        this.IsDropDownOpen = true;
                        //this.Focus();

                        // Unselect
                        this.EditableTextBox.SelectionStart = int.MaxValue;
                    }
                }

                base.OnKeyUp(e);

                // Update Filter Value
                this.currentFilter = this.Text;
            }
        }

        /// <summary>
        /// Make sure the text corresponds to the selection when leaving the control.
        /// </summary>
        /// <param name="e">A KeyBoardFocusChangedEventArgs.</param>
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            this.ClearFilter();
            int temp = this.SelectedIndex;
            this.SelectedIndex = -1;
            this.Text = string.Empty;
            this.SelectedIndex = temp;
            base.OnPreviewLostKeyboardFocus(e);
        }

        /// <summary>
        /// On selection, clear selected items from the list.
        /// </summary>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            //SelectedIndex = -1;
        }


        ////
        // Helpers
        ////

        /// <summary>
        /// Re-apply the Filter.
        /// </summary>
        private void RefreshFilter()
        {
            if (this.ItemsSource != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemsSource);
                view.Refresh();
            }
        }

        /// <summary>
        /// Clear the Filter.
        /// </summary>
        private void ClearFilter()
        {
            this.currentFilter = string.Empty;
            this.RefreshFilter();
        }

        /// <summary>
        /// The Filter predicate that will be applied to each row in the ItemsSource.
        /// </summary>
        /// <param name="value">A row in the ItemsSource.</param>
        /// <returns>Whether or not the item will appear in the DropDown.</returns>
        private bool FilterPredicate(object value)
        {
            // No filter, no text
            if (value == null)
            {
                return false;
            }

            // No text, no filter
            if (this.currentFilter.Length == 0)
            {
                return true;
            }

            // Case insensitive search
            string stringValue = "";
            stringValue = (value.GetType().GetProperty(this.DisplayMemberPath).GetValue(value, null)).ToString();

            if (TextSearchMode == SearchMode.StartsWith)
                return stringValue.ToLower().StartsWith(this.Text.ToLower());
            else
                return stringValue.ToLower().Contains(this.Text.ToLower());
        }
    }


    public class DropDownButtonBehavior : Behavior<ImageButton>
    {
        private bool isContextMenuOpen;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button source = sender as Button;
            if (source != null && source.ContextMenu != null)
            {
                if (!isContextMenuOpen)
                {
                    // Add handler to detect when the ContextMenu closes
                    source.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                    // If there is a drop-down assigned to this button, then position and display it 
                    source.ContextMenu.PlacementTarget = source;
                    source.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    source.ContextMenu.IsOpen = true;
                    isContextMenuOpen = true;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            isContextMenuOpen = false;
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
            }
        }
    }

}
