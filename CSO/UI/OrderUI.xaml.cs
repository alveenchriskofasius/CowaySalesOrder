﻿using System;
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

namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for OrderUI.xaml
    /// </summary>
    public partial class OrderUI : BaseUI
    {
        public OrderUI()
        {
            InitializeComponent();
        }

        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            OrderForm.Main = Main;
        }
    }
}
