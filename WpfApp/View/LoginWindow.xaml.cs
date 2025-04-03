using BLL.Service;
using DAL.Models;
using System;
using System.Windows;

namespace WpfApp.View
{
    public partial class LoginWindow : Window
    {
        private readonly AccountService _accountService;

        public LoginWindow()
        {
            InitializeComponent();
            _accountService = new AccountService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Bước 3: Lấy thông tin email và password từ form
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Password.Trim();

                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ email và mật khẩu!",
                                  "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Bước 4: Gọi AccountService.GetAccount() để xác thực
                var account = _accountService.GetAccount(email, password);

                // Bước 5: Nếu thành công, lưu tài khoản vào biến toàn cục (App.CurrentAccount)
                if (account != null)
                {
                    // Lưu tài khoản vào biến toàn cục
                    App.CurrentAccount = account;

                    // Hiển thị thông báo đăng nhập thành công
                    MessageBox.Show($"Đăng nhập thành công! Vai trò: {account.Role}",
                                  "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở MainWindow và đóng LoginWindow
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    // Hiển thị thông báo đăng nhập thất bại
                    MessageBox.Show("Email hoặc mật khẩu không đúng!",
                                  "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng nhập: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
