﻿using ModelsApi;
using MyCar.Desktop.ViewModels;
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

namespace MyCar.Desktop.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditPasswordWindow.xaml
    /// </summary>
    public partial class EditPasswordWindow : Window
    {
        public EditPasswordWindow(UserApi user)
        {
            InitializeComponent();
            DataContext = new EditPassword(user);
        }
    }
}
