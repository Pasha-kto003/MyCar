using MyCar.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            DataContext = new AuthViewModel(this, passwordtxt);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void ShowPasswordCharsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            passwordtxt.Visibility = Visibility.Collapsed;
            textboxpass.Visibility = Visibility.Visible;

            textboxpass.Text = new NetworkCredential(string.Empty, passwordtxt.SecurePassword).Password;
            textboxpass.Focus();
        }

        private void ShowPasswordCharsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordtxt.Visibility = Visibility.Visible;
            textboxpass.Visibility = Visibility.Collapsed;

            passwordtxt.Focus();
        }

        private void textboxpass_TextChanged(object sender, TextChangedEventArgs e)
        {
            passwordtxt.Password = textboxpass.Text;
        }
    }
}
