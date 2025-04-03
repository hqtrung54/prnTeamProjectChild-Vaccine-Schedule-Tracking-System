using BLL.Service;
using DAL.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    public partial class VaccineWindow : Window
    {
        private readonly VaccineService _vaccineService;
        private Vaccine _selectedVaccine;

        public VaccineWindow()
        {
            InitializeComponent();
            _vaccineService = new VaccineService();

            // Hiển thị thông tin người dùng đăng nhập
            if (App.CurrentAccount != null)
            {
                txtLoginStatus.Text = $"Đăng nhập: {App.CurrentAccount.Email}";
                ConfigureUIBasedOnRole();
            }

            // Load danh sách vắc-xin khi khởi tạo
            LoadVaccines();

            // Thiết lập trạng thái ban đầu của các nút
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        /// <summary>
        /// Cài đặt phân quyền theo vai trò người dùng
        /// </summary>
        private void ConfigureUIBasedOnRole()
        {
            // Role = 1 là Admin, có tất cả quyền
            // Role = 2 là Nhân viên, chỉ được xem, tìm kiếm
            if (App.CurrentAccount?.Role != 1)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Bước 5: Cài đặt chức năng hiển thị danh sách vắc-xin
        /// </summary>
        private void LoadVaccines()
        {
            try
            {
                var vaccines = _vaccineService.GetAllVaccines();
                dgVaccines.ItemsSource = vaccines;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vắc-xin: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi chọn vắc-xin trong DataGrid
        /// </summary>
        private void dgVaccines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVaccines.SelectedItem is Vaccine selectedVaccine)
            {
                _selectedVaccine = selectedVaccine;

                // Hiển thị thông tin lên form
                txtVaccineName.Text = selectedVaccine.VaccineName;
                txtManufacturer.Text = selectedVaccine.Manufacturer;
                txtDescription.Text = selectedVaccine.Description;
                txtAgeGroup.Text = selectedVaccine.AgeGroup;
                txtPrice.Text = selectedVaccine.Price.ToString();

                // Kích hoạt các nút chức năng
                if (App.CurrentAccount?.Role == 1)
                {
                    btnUpdate.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Bước 6: Cài đặt chức năng thêm vắc-xin mới
        /// </summary>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (!ValidateInput(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tạo đối tượng vắc-xin mới
                var vaccine = new Vaccine
                {
                    VaccineName = txtVaccineName.Text.Trim(),
                    Manufacturer = txtManufacturer.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    AgeGroup = txtAgeGroup.Text.Trim(),
                    Price = decimal.Parse(txtPrice.Text.Trim())
                };

                // Thêm vào database
                if (_vaccineService.AddVaccine(vaccine))
                {
                    MessageBox.Show("Thêm vắc-xin mới thành công!", "Thông báo",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadVaccines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm vắc-xin: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Bước 7: Cài đặt chức năng cập nhật thông tin vắc-xin
        /// </summary>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedVaccine == null)
                {
                    MessageBox.Show("Vui lòng chọn vắc-xin để cập nhật",
                                   "Chưa chọn vắc-xin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (!ValidateInput(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật thông tin vắc-xin
                _selectedVaccine.VaccineName = txtVaccineName.Text.Trim();
                _selectedVaccine.Manufacturer = txtManufacturer.Text.Trim();
                _selectedVaccine.Description = txtDescription.Text.Trim();
                _selectedVaccine.AgeGroup = txtAgeGroup.Text.Trim();
                _selectedVaccine.Price = decimal.Parse(txtPrice.Text.Trim());

                // Cập nhật vào database
                if (_vaccineService.UpdateVaccine(_selectedVaccine))
                {
                    MessageBox.Show("Cập nhật vắc-xin thành công!", "Thông báo",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadVaccines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật vắc-xin: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Bước 8: Cài đặt chức năng xóa vắc-xin
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedVaccine == null)
                {
                    MessageBox.Show("Vui lòng chọn vắc-xin để xóa",
                                   "Chưa chọn vắc-xin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa vắc-xin '{_selectedVaccine.VaccineName}'?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Xóa khỏi database
                    if (_vaccineService.DeleteVaccine(_selectedVaccine.VaccineId))
                    {
                        MessageBox.Show("Xóa vắc-xin thành công!", "Thông báo",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        LoadVaccines();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa vắc-xin: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Bước 9: Cài đặt chức năng tìm kiếm và lọc
        /// </summary>
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                string filterType = cboFilterType.SelectedItem != null
                    ? ((ComboBoxItem)cboFilterType.SelectedItem).Content.ToString()
                    : "Tìm theo tên";

                var results = _vaccineService.SearchVaccines(keyword, filterType);
                dgVaccines.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm vắc-xin: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút quay lại
        /// </summary>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại trang chính: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút đăng xuất
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn đăng xuất không?",
                    "Xác nhận đăng xuất",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    App.CurrentAccount = null;
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

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút làm mới form
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        /// <summary>
        /// Xóa dữ liệu trên form nhập liệu
        /// </summary>
        private void ClearForm()
        {
            txtVaccineName.Text = string.Empty;
            txtManufacturer.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtAgeGroup.Text = string.Empty;
            txtPrice.Text = string.Empty;

            _selectedVaccine = null;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        /// <summary>
        /// Kiểm tra dữ liệu nhập liệu
        /// </summary>
        private bool ValidateInput(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(txtVaccineName.Text))
            {
                errorMessage = "Tên vắc-xin không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtManufacturer.Text))
            {
                errorMessage = "Nhà sản xuất không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAgeGroup.Text))
            {
                errorMessage = "Nhóm tuổi không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                errorMessage = "Giá tiền không được để trống!";
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                errorMessage = "Giá tiền phải là số!";
                return false;
            }

            if (price <= 0)
            {
                errorMessage = "Giá tiền phải lớn hơn 0!";
                return false;
            }

            return true;
        }
    }
}
