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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(passwordtxt.PasswordChar == '*')
            {
                textpass.Text = passwordtxt.Password;
            }
            if(textpass.Text != "")
            {
                //Image img = new Image();
                //img.Source = new BitmapImage(new Uri(@"C:\Users\User\source\repos\MyCar\MyCar.Desktop\Images\icons8-close-30.png"));
                //Button3.Content = img;
                Button3.Visibility = Visibility.Hidden;
                Button4.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            textpass.Text = "";
            Button3.Visibility = Visibility.Visible;
            Button4.Visibility = Visibility.Hidden;
        }
    }
}
