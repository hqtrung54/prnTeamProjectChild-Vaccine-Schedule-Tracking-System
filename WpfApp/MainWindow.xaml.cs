using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DAL.Models;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Account Account { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Wire up the button click events
            if (btnChild != null)
                btnChild.Click += BtnChild_Click;

            if (btnCustomer != null)
                btnCustomer.Click += BtnCustomer_Click;
        }

        private void BtnChild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChildWindow childWindow = new ChildWindow();
                childWindow.Show();
                this.Hide(); // Hide instead of close to maintain app state
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ Hồ Sơ Trẻ Em: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerWindow customerWindow = new CustomerWindow();
                customerWindow.Show();
                this.Hide(); // Hide instead of close to maintain app state
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ Hồ Sơ Khách Hàng: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Your existing login logic here
        }
    }
}
