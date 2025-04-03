using BLL.Service;
using BLL.Services;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    public partial class PostVaccinationRecordWindow : Window
    {
        private readonly PostVaccinationRecordService _recordService;
        private readonly ChildService _childService;
        private readonly VaccineService _vaccineService;
        private PostVaccinationRecord _selectedRecord;

        public PostVaccinationRecordWindow()
        {
            InitializeComponent();

            _recordService = new PostVaccinationRecordService();
            _childService = new ChildService();
            _vaccineService = new VaccineService();

            // Hiển thị thông tin người dùng đăng nhập
            if (App.CurrentAccount != null)
            {
                txtLoginStatus.Text = $"Đăng nhập: {App.CurrentAccount.Email}";
                ConfigureUIBasedOnRole();
            }

            // Thiết lập các ComboBox tìm kiếm
            cboSearchType.SelectionChanged += CboSearchType_SelectionChanged;

            // Load dữ liệu ban đầu
            //LoadChildren();
            //LoadVaccines();
            //LoadRecords();

            // Thiết lập trạng thái ban đầu của các nút
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        /// <summary>
        /// Thiết lập giao diện dựa trên vai trò người dùng
        /// </summary>
        private void ConfigureUIBasedOnRole()
        {
            // Lấy vai trò người dùng
            int? userRole = App.CurrentAccount?.Role;
            int? customerId = App.CurrentAccount?.CustomerId;

            // Admin (1) và Nhân viên (2) có tất cả quyền
            if (userRole == 1 || userRole == 2)
            {
                btnAdd.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;

                // Load tất cả hồ sơ
                LoadRecords();
            }
            // Khách hàng (3) chỉ xem được hồ sơ của con mình
            else if (userRole == 3 && customerId.HasValue)
            {
                // Ẩn các nút thêm, sửa, xóa
                btnAdd.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;

                // Chỉ load hồ sơ của con mình
                LoadCustomerRecords(customerId.Value);
            }
        }

        /// <summary>
        /// Load danh sách trẻ em vào ComboBox
        /// </summary>
        private void LoadChildren()
        {
            try
            {
                var children = _childService.GetChildren();
                cboChild.ItemsSource = children;

                // Nếu là khách hàng, chỉ hiển thị con của họ
                if (App.CurrentAccount?.Role == 3 && App.CurrentAccount?.CustomerId != null)
                {
                    int customerId = App.CurrentAccount.CustomerId.Value;
                    children = children.Where(c => c.CustomerId == customerId).ToList();
                }

                cboChild.ItemsSource = children;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách trẻ em: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load danh sách vắc-xin vào ComboBox
        /// </summary>
        private void LoadVaccines()
        {
            try
            {
                var vaccines = _vaccineService.GetAllVaccines();
                cboVaccine.ItemsSource = vaccines;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách vắc-xin: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load tất cả hồ sơ tiêm chủng
        /// </summary>
        private void LoadRecords()
        {
            try
            {
                var records = _recordService.GetAllRecords();
                // Tạo dữ liệu hiển thị với tên trẻ và tên vắc-xin
                var displayRecords = records.Select(r => new
                {
                    r.RecordId,
                    r.ChildId,
                    ChildName = r.Child?.FullName ?? "Không xác định",
                    r.VaccineId,
                    VaccineName = r.Vaccine?.VaccineName ?? "Không xác định",
                    r.ReactionDescription,
                    ReportDateText = r.ReportDate.HasValue
         ? r.ReportDate.Value.ToString("dd/MM/yyyy")
         : "Chưa ghi nhận"
                }).ToList();

                dgRecords.ItemsSource = displayRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách hồ sơ tiêm chủng: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load hồ sơ tiêm chủng của con khách hàng
        /// </summary>
        private void LoadCustomerRecords(int customerId)
        {
            try
            {
                var records = _recordService.GetRecordsByCustomerId(customerId);
                // Tạo dữ liệu hiển thị với tên trẻ và tên vắc-xin
                var displayRecords = records.Select(r => new
                {
                    r.RecordId,
                    r.ChildId,
                    ChildName = r.Child?.FullName ?? "Không xác định",
                    r.VaccineId,
                    VaccineName = r.Vaccine?.VaccineName ?? "Không xác định",
                    r.ReactionDescription,
                    r.ReportDate
                }).ToList();

                dgRecords.ItemsSource = displayRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách hồ sơ tiêm chủng: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Sự kiện khi chọn loại tìm kiếm
        /// </summary>
        private void CboSearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hiển thị hoặc ẩn DatePicker dựa trên loại tìm kiếm
            if (cboSearchType.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content.ToString() == "Tìm theo ngày")
                {
                    dpSearchDate.Visibility = Visibility.Visible;
                    txtSearch.Visibility = Visibility.Collapsed;
                }
                else
                {
                    dpSearchDate.Visibility = Visibility.Collapsed;
                    txtSearch.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Sự kiện khi chọn hồ sơ trong DataGrid
        /// </summary>
        private void DgRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Lấy ID của hồ sơ được chọn (do sử dụng anonymous type cho hiển thị)
                if (dgRecords.SelectedItem != null)
                {
                    // Dùng reflection để lấy RecordId từ anonymous type
                    var recordId = (int)dgRecords.SelectedItem.GetType().GetProperty("RecordId").GetValue(dgRecords.SelectedItem, null);

                    // Lấy hồ sơ từ service
                    _selectedRecord = _recordService.GetRecordById(recordId);

                    if (_selectedRecord != null)
                    {
                        // Hiển thị thông tin lên form
                        cboChild.SelectedValue = _selectedRecord.ChildId;
                        cboVaccine.SelectedValue = _selectedRecord.VaccineId;
                        txtReactionDescription.Text = _selectedRecord.ReactionDescription;
                        dpRecordDate.SelectedDate = _selectedRecord.ReportDate;

                        // Kích hoạt các nút chức năng cho admin và staff
                        if (App.CurrentAccount?.Role == 1 || App.CurrentAccount?.Role == 2)
                        {
                            btnUpdate.IsEnabled = true;
                            btnDelete.IsEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn hồ sơ: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Thêm hồ sơ tiêm chủng mới
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

                // Tạo đối tượng hồ sơ mới
                var record = new PostVaccinationRecord
                {
                    ChildId = (int)cboChild.SelectedValue,
                    VaccineId = (int)cboVaccine.SelectedValue,
                    ReactionDescription = txtReactionDescription.Text.Trim(),
                    ReportDate = dpRecordDate.SelectedDate
                };

                // Thêm vào database
                if (_recordService.AddRecord(record))
                {
                    MessageBox.Show("Thêm hồ sơ tiêm chủng thành công!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadRecords();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm hồ sơ tiêm chủng: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cập nhật hồ sơ tiêm chủng
        /// </summary>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedRecord == null)
                {
                    MessageBox.Show("Vui lòng chọn hồ sơ để cập nhật",
                                    "Chưa chọn hồ sơ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (!ValidateInput(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật thông tin hồ sơ
                _selectedRecord.ChildId = (int)cboChild.SelectedValue;
                _selectedRecord.VaccineId = (int)cboVaccine.SelectedValue;
                _selectedRecord.ReactionDescription = txtReactionDescription.Text.Trim();
                _selectedRecord.ReportDate = dpRecordDate.SelectedDate;

                // Cập nhật vào database
                if (_recordService.UpdateRecord(_selectedRecord))
                {
                    MessageBox.Show("Cập nhật hồ sơ tiêm chủng thành công!", "Thông báo",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadRecords();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật hồ sơ tiêm chủng: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xóa hồ sơ tiêm chủng
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedRecord == null)
                {
                    MessageBox.Show("Vui lòng chọn hồ sơ để xóa",
                                    "Chưa chọn hồ sơ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                MessageBoxResult result = MessageBox.Show(
                    "Bạn có chắc muốn xóa hồ sơ tiêm chủng này không?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Xóa khỏi database
                    if (_recordService.DeleteRecord(_selectedRecord.RecordId))
                    {
                        MessageBox.Show("Xóa hồ sơ tiêm chủng thành công!", "Thông báo",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        LoadRecords();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa hồ sơ tiêm chủng: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tìm kiếm hồ sơ tiêm chủng
        /// </summary>
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchType = cboSearchType.SelectedItem != null
                    ? ((ComboBoxItem)cboSearchType.SelectedItem).Content.ToString()
                    : "Tìm theo tên trẻ";

                string keyword = txtSearch.Text.Trim();
                DateTime? searchDate = dpSearchDate.SelectedDate;

                // Gọi service để tìm kiếm
                var records = _recordService.SearchRecords(searchType, keyword, searchDate);

                // Tạo dữ liệu hiển thị
                var displayRecords = records.Select(r => new
                {
                    r.RecordId,
                    r.ChildId,
                    ChildName = r.Child?.FullName ?? "Không xác định",
                    r.VaccineId,
                    VaccineName = r.Vaccine?.VaccineName ?? "Không xác định",
                    r.ReactionDescription,
                    ReportDateText = r.ReportDate.HasValue
        ? r.ReportDate.Value.ToString("dd/MM/yyyy")
        : "Chưa ghi nhận"
                }).ToList();

                dgRecords.ItemsSource = displayRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm hồ sơ tiêm chủng: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Làm mới form
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        /// <summary>
        /// Quay lại trang chính
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
        /// Đăng xuất
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
        /// Xóa dữ liệu trên form nhập liệu
        /// </summary>
        private void ClearForm()
        {
            cboChild.SelectedIndex = -1;
            cboVaccine.SelectedIndex = -1;
            txtReactionDescription.Text = string.Empty;
            dpRecordDate.SelectedDate = null;

            _selectedRecord = null;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        /// <summary>
        /// Kiểm tra dữ liệu nhập liệu
        /// </summary>
        private bool ValidateInput(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (cboChild.SelectedItem == null)
            {
                errorMessage = "Vui lòng chọn trẻ!";
                return false;
            }

            if (cboVaccine.SelectedItem == null)
            {
                errorMessage = "Vui lòng chọn vắc-xin!";
                return false;
            }

            // Ngày báo cáo có thể null trong model nhưng chúng ta có thể bắt buộc nhập
            if (dpRecordDate.SelectedDate == null)
            {
                errorMessage = "Vui lòng chọn ngày ghi nhận!";
                return false;
            }

            // Kiểm tra ngày không được trong tương lai
            if (dpRecordDate.SelectedDate > DateTime.Now)
            {
                errorMessage = "Ngày ghi nhận không thể là ngày trong tương lai!";
                return false;
            }

            return true;
        }
    }
}
