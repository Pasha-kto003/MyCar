﻿using ModelsApi;
using MyCar.Desktop.ViewModels.AddViewModels;
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
using System.Windows.Shapes;

namespace MyCar.Desktop.Windows.AddWindows
{
    /// <summary>
    /// Логика взаимодействия для AddCarSaleWindow.xaml
    /// </summary>
    public partial class AddCarSaleWindow : Window
    {
        public AddCarSaleWindow()
        {
            InitializeComponent();
            DataContext = new AddCarSaleViewModel(null);
        }

        public AddCarSaleWindow(SaleCarApi saleCar)
        {
            InitializeComponent();
            DataContext = new AddCarSaleViewModel(saleCar);
        }
    }
}
