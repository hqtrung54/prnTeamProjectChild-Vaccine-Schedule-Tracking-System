using BLL.Service;  // Import AccountService
using DAL.Models;
using DAL.Repos;
using System;
using System.Windows;

namespace WpfApp.View
{
    public partial class LoginWindow : Window
    {
        private readonly AccountService _accountService;

        // Constructor
        public LoginWindow()
        {
            InitializeComponent();
            _accountService = new AccountService(new AccountRepository(new VaccineManagementSystem1Context()));
        }

        // Login button click event
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;  // Lấy tên người dùng từ TextBox
            string password = Password.Password;  // Lấy mật khẩu từ PasswordBox

            // Kiểm tra tài khoản với dịch vụ
            var account = await _accountService.GetByEmailAsync(username);  // Lấy tài khoản qua email

            if (account != null && account.PasswordHash == password)  // Kiểm tra mật khẩu
            {
                MessageBox.Show("Login successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Tiến hành xử lý sau khi đăng nhập thành công (mở cửa sổ khác, v.v...)
                // Ví dụ, mở cửa sổ dashboard
                var dashboard = new MainWindow();
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
