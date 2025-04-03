using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BLL.Services;
using DAL.Models;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private readonly CustomerServices _customerServices;

        public CustomerWindow()
        {
            InitializeComponent();
            _customerServices = new CustomerServices();
            LoadCustomerData();
            ConfigureUIBasedOnRole();
        }

        private void LoadCustomerData()
        {
            if (App.CurrentAccount == null) return;

            int userRole = App.CurrentAccount.Role ?? 0;
            int customerId = App.CurrentAccount.CustomerId ?? 0; // ID khách hàng đang đăng nhập

            List<Customer> customers = new List<Customer>(); // Khởi tạo danh sách rỗng để chứa khách hàng

            if (userRole == 3) // Nếu là khách hàng thì chỉ load thông tin khách hàng của chính mình
            {
                var customer = _customerServices.GetCustomerById(customerId);
                if (customer != null)
                {
                    customers.Add(customer); // Thêm khách hàng của chính mình vào danh sách
                }
            }
            else // Nếu là Admin hoặc Nhân viên, load toàn bộ danh sách khách hàng
            {
                customers = _customerServices.GetAllCustomers();
            }

            // Gán danh sách khách hàng vào DataGrid
            dgCustomers.ItemsSource = customers;
        }

        private void ConfigureUIBasedOnRole()
        {
            if (App.CurrentAccount == null || !App.CurrentAccount.Role.HasValue) return;

            // Kiểm tra vai trò
            int userRole = App.CurrentAccount.Role.Value;
            if (userRole == 1 || userRole == 2)
            {
                if (btnAddCustomer != null)
                    btnAddCustomer.Visibility = Visibility.Visible;
                if (btnDeleteCustomer != null)
                    btnDeleteCustomer.Visibility = Visibility.Visible;
                if (btnSearchCustomer != null)
                    btnSearchCustomer.Visibility = Visibility.Visible;
                if (btnUpdateCustomer != null)
                    btnUpdateCustomer.Visibility = Visibility.Visible;
            }
            else if (userRole == 3)
            {
                if (btnAddCustomer != null)
                    btnAddCustomer.Visibility = Visibility.Hidden;
                if (btnDeleteCustomer != null)
                    btnDeleteCustomer.Visibility = Visibility.Hidden;
                if (btnSearchCustomer != null)
                    btnSearchCustomer.Visibility = Visibility.Hidden;
                if (btnUpdateCustomer != null)
                    btnUpdateCustomer.Visibility = Visibility.Hidden;

            }
        }

        private bool ValidateCustomerInput(string fullName, string email, string phoneNumber, string address)
        {
            if (string.IsNullOrWhiteSpace(fullName) || !Regex.IsMatch(fullName, @"^[\p{L} ]+$"))
            {
                MessageBox.Show("Họ tên không hợp lệ. Vui lòng nhập lại (không chứa số).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ. Vui lòng nhập lại (Phải có @).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(phoneNumber) || !Regex.IsMatch(phoneNumber, @"^\d+$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập lại (chỉ chứa số).", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Địa chỉ không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fullName = txtNewCustomerFullName.Text.Trim();
                var email = txtNewEmail.Text.Trim();
                var phoneNumber = txtNewPhoneNumber.Text.Trim();
                var address = txtNewAddress.Text.Trim();
                var dateOfBirth = dpNewBirthday.SelectedDate;
                var password = txtNewPassword.Password.Trim(); // Lấy mật khẩu từ ô nhập

                if (!ValidateCustomerInput(fullName, email, phoneNumber, address))
                {
                    return;
                }

                var customer = new Customer
                {
                    CustomerFullName = fullName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    DateOfBirth = DateOnly.FromDateTime(dateOfBirth ?? DateTime.Now)
                };

                _customerServices.AddCustomer(customer ,password);
                MessageBox.Show("Thêm khách hàng thành công.");
                LoadCustomerData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgCustomers.SelectedItem is Customer selectedCustomer)
                {
                    // Lấy thông tin từ các trường nhập liệu
                    var fullName = txtEditCustomerFullName.Text.Trim();
                    var email = txtEditEmail.Text.Trim();
                    var phoneNumber = txtEditPhoneNumber.Text.Trim();
                    var address = txtEditAddress.Text.Trim();
                    var dateOfBirth = dpEditBirthday.SelectedDate;

                    // Kiểm tra tính hợp lệ của dữ liệu đầu vào
                    if (!ValidateCustomerInput(fullName, email, phoneNumber, address))
                    {
                        return;
                    }

                    // Xác định mật khẩu (ví dụ như để trống hoặc yêu cầu người dùng nhập mật khẩu mới)
                    string password = txtEditPassword.Password.Trim(); // Giả sử bạn có một ô nhập mật khẩu cho người dùng

                    if (string.IsNullOrEmpty(password))
                    {
                        // Nếu không có mật khẩu, bạn có thể tạo mật khẩu tự động hoặc báo lỗi
                        throw new Exception("Mật khẩu không được để trống.");
                    }

                    // Tạo đối tượng Customer mới với dữ liệu cập nhật
                    var updatedCustomer = new Customer
                    {
                        CustomerId = selectedCustomer.CustomerId,
                        CustomerFullName = fullName,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        Address = address,
                        DateOfBirth = DateOnly.FromDateTime(dateOfBirth ?? DateTime.Now)
                    };

                    // Gọi phương thức UpdateCustomer để cập nhật thông tin và tạo Account
                    _customerServices.UpdateCustomer(updatedCustomer, password);

                    // Hiển thị thông báo thành công và tải lại dữ liệu khách hàng
                    MessageBox.Show("Cập nhật khách hàng thành công.");
                    LoadCustomerData();
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để cập nhật.");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi cập nhật
                MessageBox.Show($"Lỗi khi cập nhật khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgCustomers.SelectedItem is Customer selectedCustomer)
                {
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xoá khách hàng này?", "Xoá khách hàng", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        _customerServices.DeleteCustomer(selectedCustomer.CustomerId);
                        MessageBox.Show("Xoá khách hàng thành công.");
                        LoadCustomerData();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để xoá.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xoá khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchKeyword = txtSearchCustomer.Text.Trim();
                var customers = _customerServices.GetCustomersByEmailContains(searchKeyword);
                dgCustomers.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại trang chủ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
