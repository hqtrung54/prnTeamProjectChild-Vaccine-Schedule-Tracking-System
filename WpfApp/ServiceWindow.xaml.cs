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
using BLL.Services;
using DAL.Models;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : Window
    {
        private readonly ServiceManagerServices _serviceManagerService;
        public ServiceWindow()
        {
            InitializeComponent();
            _serviceManagerService = new ServiceManagerServices();
            LoadServices();
            ConfigureUIBasedOnRole();
        }


        // Hàm để tải danh sách dịch vụ vào DataGrid
        private void LoadServices()
        {
            var services = _serviceManagerService.GetAllServices();
            dgServices.ItemsSource = services;
        }


        private void ConfigureUIBasedOnRole()
        {
            if (App.CurrentAccount == null || !App.CurrentAccount.Role.HasValue) return;

            // Kiểm tra vai trò
            int userRole = App.CurrentAccount.Role.Value;
            if (userRole == 1 || userRole == 2)
            {
                if (btnAddService != null)
                    btnAddService.Visibility = Visibility.Visible;
                if (btnDeleteService != null)
                    btnDeleteService.Visibility = Visibility.Visible;
                if (btnSearchService != null)
                    btnSearchService.Visibility = Visibility.Visible;
                if (btnUpdateService != null)
                    btnUpdateService.Visibility = Visibility.Visible;
            }
            else if (userRole == 3)
            {
                if (btnAddService != null)
                    btnAddService.Visibility = Visibility.Hidden;
                if (btnDeleteService != null)
                    btnDeleteService.Visibility = Visibility.Hidden;
                if (btnSearchService != null)
                    btnSearchService.Visibility = Visibility.Hidden;
                if (btnUpdateService != null)
                    btnUpdateService.Visibility = Visibility.Hidden;

            }
        }
        // Sự kiện khi nhấn nút Thêm dịch vụ
        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy thông tin từ các TextBox
                string serviceName = txtNewServiceName.Text;
                string targetGroup = txtNewTargetGroup.Text;
                string description = txtNewDescription.Text;
                decimal price = decimal.TryParse(txtNewPrice.Text, out decimal result) ? result : 0;
                int vaccineId = int.TryParse(txtNewVaccineId.Text, out int vaccineIdResult) ? vaccineIdResult : 0;

                // Kiểm tra tên dịch vụ không được để trống và không chứa số
                if (string.IsNullOrWhiteSpace(serviceName))
                {
                    MessageBox.Show("Tên dịch vụ không được để trống.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (serviceName.Any(char.IsDigit))
                {
                    MessageBox.Show("Tên dịch vụ không được chứa số.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra nhóm đối tượng không được để trống và không chứa số
                if (string.IsNullOrWhiteSpace(targetGroup))
                {
                    MessageBox.Show("Nhóm đối tượng không được để trống.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (targetGroup.Any(char.IsDigit))
                {
                    MessageBox.Show("Nhóm đối tượng không được chứa số.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra mô tả dịch vụ không được để trống
                if (string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Mô tả dịch vụ không thể để trống.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra giá dịch vụ phải là một số dương
                if (price <= 0)
                {
                    MessageBox.Show("Giá dịch vụ phải là một số dương.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra VaccineId phải là một số dương
                if (vaccineId <= 0)
                {
                    MessageBox.Show("VaccineId không hợp lệ.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newService = new Service
                {
                    ServiceName = serviceName,
                    TargetGroup = targetGroup,
                    Description = description,
                    Price = price,
                    VaccineId = vaccineId
                };

                // Thêm dịch vụ mới
                if (_serviceManagerService.AddService(newService))
                {
                    MessageBox.Show("Dịch vụ đã được thêm thành công!");
                    LoadServices(); // Cập nhật lại DataGrid
                }
                else
                {
                    MessageBox.Show("Vui lòng kiểm tra lại thông tin dịch vụ.");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra: {ex.Message}", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Sự kiện khi nhấn nút Cập nhật dịch vụ
        // Sự kiện khi nhấn nút Cập nhật dịch vụ
        private void BtnUpdateService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem có dịch vụ nào được chọn không
                if (dgServices.SelectedItem is Service selectedService)
                {
                    // Kiểm tra dữ liệu đầu vào
                    string serviceName = txtEditServiceName.Text;
                    string targetGroup = txtEditTargetGroup.Text;
                    string description = txtEditDescription.Text;
                    decimal price = decimal.TryParse(txtEditPrice.Text, out decimal result) ? result : 0;
                    int vaccineId = int.TryParse(txtEditVaccineId.Text, out int vaccineIdResult) ? vaccineIdResult : 0;

                    // Kiểm tra tên dịch vụ không được để trống và không chứa số
                    if (string.IsNullOrWhiteSpace(serviceName))
                    {
                        MessageBox.Show("Tên dịch vụ không được để trống.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (serviceName.Any(char.IsDigit))
                    {
                        MessageBox.Show("Tên dịch vụ không được chứa số.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra nhóm đối tượng không được để trống và không chứa số
                    if (string.IsNullOrWhiteSpace(targetGroup))
                    {
                        MessageBox.Show("Nhóm đối tượng không được để trống.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (targetGroup.Any(char.IsDigit))
                    {
                        MessageBox.Show("Nhóm đối tượng không được chứa số.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra mô tả dịch vụ không được để trống
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        MessageBox.Show("Mô tả dịch vụ không thể để trống.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra giá dịch vụ phải là một số dương
                    if (price <= 0)
                    {
                        MessageBox.Show("Giá dịch vụ phải là một số dương.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra VaccineId phải là một số dương
                    if (vaccineId <= 0)
                    {
                        MessageBox.Show("VaccineId không hợp lệ.", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật thông tin của dịch vụ đã chọn
                    selectedService.ServiceName = serviceName;
                    selectedService.TargetGroup = targetGroup;
                    selectedService.Description = description;
                    selectedService.Price = price;
                    selectedService.VaccineId = vaccineId;

                    // Cập nhật dịch vụ
                    if (_serviceManagerService.UpdateService(selectedService))
                    {
                        MessageBox.Show("Dịch vụ đã được cập nhật thành công!");
                        LoadServices(); // Cập nhật lại DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng kiểm tra lại thông tin dịch vụ.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn dịch vụ cần cập nhật.");
                }
            }
            catch (ArgumentException ex)
            {
                // Xử lý ngoại lệ và thông báo lỗi
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung và thông báo lỗi
                MessageBox.Show($"Đã có lỗi xảy ra: {ex.Message}", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Sự kiện khi nhấn nút Xóa dịch vụ
        private void BtnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (dgServices.SelectedItem is Service selectedService)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (_serviceManagerService.DeleteService(selectedService.ServiceId))
                    {
                        MessageBox.Show("Dịch vụ đã được xóa thành công!");
                        LoadServices(); // Cập nhật lại DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Xóa dịch vụ không thành công.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần xóa.");
            }
        }

        // Sự kiện khi nhấn nút Tìm kiếm dịch vụ theo nhóm đối tượng
        // Sự kiện khi nhấn nút Tìm kiếm dịch vụ theo nhóm đối tượng
        private void BtnSearchService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetGroup = txtSearchTargetGroup.Text;

                // Gọi phương thức SearchByTargetGroupContain từ ServiceManagerServices để tìm kiếm dịch vụ
                var searchResult = _serviceManagerService.SearchByTargetGroup(targetGroup);

                // Hiển thị kết quả tìm kiếm trong DataGrid
                dgServices.ItemsSource = searchResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã có lỗi xảy ra: {ex.Message}", "Thông báo lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Sự kiện quay lại trang chủ
        private void BtnBackHome_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang chủ (có thể chuyển đến một cửa sổ khác nếu cần)
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
