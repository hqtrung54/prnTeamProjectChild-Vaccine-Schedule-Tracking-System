using System;
using System.Windows;
using DAL.Models;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Kiểm tra người dùng đã đăng nhập chưa
            if (App.CurrentAccount == null)
            {
                MessageBox.Show("Bạn cần đăng nhập để sử dụng hệ thống!",
                                "Yêu cầu đăng nhập", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Quay về màn hình đăng nhập
                View.LoginWindow loginWindow = new View.LoginWindow();
                loginWindow.Show();
                this.Close();
                return;
            }

            // Bước 7: Hiển thị thông tin người dùng đã đăng nhập
            DisplayUserInfo();

            // Bước 8: Thiết lập UI dựa trên vai trò
            ConfigureUIBasedOnRole();

            // Kết nối sự kiện cho các nút
            if (btnChild != null)
                btnChild.Click += BtnChild_Click;

            if (btnCustomer != null)
                btnCustomer.Click += BtnCustomer_Click;

            if (btnLogout != null)
                btnLogout.Click += BtnLogout_Click;
        }

        // Hiển thị thông tin người dùng
        private void DisplayUserInfo()
        {
            if (txtLoginStatus != null && App.CurrentAccount != null)
            {
                // Hiển thị email và vai trò
                // With this line
                string roleName = App.CurrentAccount.Role == 1 ? "Quản trị viên" : "Nhân viên";
                txtLoginStatus.Text = $"Xin chào: {App.CurrentAccount.Email} - {roleName}";

                // Hiển thị nút đăng xuất
                if (btnLogout != null)
                    btnLogout.Visibility = Visibility.Visible;
            }
        }

        // Thiết lập UI dựa trên vai trò
        private void ConfigureUIBasedOnRole()
        {
            if (App.CurrentAccount == null || !App.CurrentAccount.Role.HasValue) return;

            // Kiểm tra vai trò
            int userRole = App.CurrentAccount.Role.Value;

            // Role = 1: Admin (doctor) - Có tất cả quyền
            if (userRole == 1)
            {
                // Hiển thị tất cả các nút chức năng
                if (btnChild != null)
                    btnChild.Visibility = Visibility.Visible;

                if (btnCustomer != null)
                    btnCustomer.Visibility = Visibility.Visible;

                // Nếu có các nút chức năng khác dành cho admin, hiển thị ở đây
            }
            // Role = 2: Nurse/Staff - Chỉ có một số quyền giới hạn
            else if (userRole == 2)
            {
                // Nurse chỉ có thể xem thông tin trẻ em
                if (btnChild != null)
                    btnChild.Visibility = Visibility.Visible;

                // Nurse không có quyền quản lý khách hàng
                if (btnCustomer != null)
                    btnCustomer.Visibility = Visibility.Collapsed;

                // Ẩn các chức năng khác nếu cần
            }
            // Các vai trò khác nếu có
            else
            {
                // Xử lý các vai trò khác nếu cần
            }
        }

        // Xử lý sự kiện đăng xuất
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Xác nhận đăng xuất
                MessageBoxResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn đăng xuất không?",
                    "Xác nhận đăng xuất",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Xóa thông tin đăng nhập
                    App.CurrentAccount = null;

                    // Quay về màn hình đăng nhập
                    View.LoginWindow loginWindow = new View.LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Các phương thức xử lý sự kiện cho các nút chức năng
        private void BtnChild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChildWindow childWindow = new ChildWindow();
                childWindow.Show();
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
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ Hồ Sơ Khách Hàng: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnVaccine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VaccineWindow w = new VaccineWindow();
                w.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ Hồ Sơ Khách Hàng: {ex.Message}",
                              "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
