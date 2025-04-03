using BLL;
using BLL.Services;
using BLL.Services;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    public partial class PaymentWindow : Window
    {
        private readonly PaymentService _paymentService;
        private readonly AppointmentServices _appointmentService;
        private Payment _selectedPayment;

        public PaymentWindow()
        {
            InitializeComponent();

            // Khởi tạo các service
            _paymentService = new PaymentService();
            _appointmentService = new AppointmentServices();

            // Hiển thị thông tin người dùng đăng nhập
            if (App.CurrentAccount != null)
            {
                txtLoginStatus.Text = $"Đăng nhập: {App.CurrentAccount.Email}";
                ConfigureUIBasedOnRole();
            }

            // Cài đặt các sự kiện cho ComboBox
            cboSearchType.SelectionChanged += CboSearchType_SelectionChanged;
            cboAppointment.SelectionChanged += CboAppointment_SelectionChanged;

            // Load dữ liệu ban đầu
            LoadAppointments();
            LoadPayments();

            // Thiết lập trạng thái ban đầu
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;

            // Thiết lập ngày mặc định cho DatePicker
            dpPaymentDate.SelectedDate = DateTime.Now;

            // Thiết lập trạng thái mặc định
            cboStatus.SelectedIndex = 0; // Pending
            cboPaymentMethod.SelectedIndex = 0; // Cash
        }

        #region Phân quyền và load dữ liệu

        /// <summary>
        /// Cấu hình UI dựa trên vai trò người dùng
        /// </summary>
        private void ConfigureUIBasedOnRole()
        {
            if (App.CurrentAccount == null) return;

            int? userRole = App.CurrentAccount.Role;
            int? customerId = App.CurrentAccount.CustomerId;

            // Admin (1) và Staff (2) có tất cả quyền
            if (userRole == 1 || userRole == 2)
            {
                btnAdd.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;

                // Load tất cả lịch hẹn và thanh toán
                LoadAppointments();
                LoadPayments();
            }
            // Customer (3) chỉ xem thanh toán của mình
            else if (userRole == 3 && customerId.HasValue)
            {
                // Ẩn các nút thêm, sửa, xóa
                btnAdd.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;

                // Chỉ load lịch hẹn và thanh toán của khách hàng hiện tại
                LoadAppointmentsByCustomerId(customerId.Value);
                LoadPaymentsByCustomerId(customerId.Value);
            }
        }

        /// <summary>
        /// Load danh sách lịch hẹn
        /// </summary>
        private void LoadAppointments()
        {
            try
            {
                var appointments = _appointmentService.GetAllAppointments();

                // Thêm DisplayInfo cho mỗi appointment
                var appointmentList = appointments.Select(a => new
                {
                    a.AppointmentId,
                    DisplayInfo = $"{a.Child?.FullName ?? "Không xác định"} - " +
                                 $"{a.Vaccine?.VaccineName ?? "Không xác định"} - " +
                                 $"{a.AppointmentDate.ToString("dd/MM/yyyy")}"
                }).ToList();

                cboAppointment.ItemsSource = appointmentList;
                cboAppointmentSearch.ItemsSource = appointmentList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách lịch hẹn: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load danh sách lịch hẹn theo khách hàng
        /// </summary>
        private void LoadAppointmentsByCustomerId(int customerId)
        {
            try
            {
                var appointments = _paymentService.GetAppointmentsForComboBoxByCustomerId(customerId);

                // Thêm DisplayInfo cho mỗi appointment
                var appointmentList = appointments.Select(a => new
                {
                    a.AppointmentId,
                    DisplayInfo = $"{a.Child?.FullName ?? "Không xác định"} - " +
                                 $"{a.Vaccine?.VaccineName ?? "Không xác định"} - " +
                                 $"{a.AppointmentDate.ToString("dd/MM/yyyy")}"
                }).ToList();

                cboAppointment.ItemsSource = appointmentList;
                cboAppointmentSearch.ItemsSource = appointmentList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách lịch hẹn: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load tất cả thanh toán
        /// </summary>
        private void LoadPayments()
        {
            try
            {
                // Sử dụng phương thức tạo dữ liệu hiển thị từ Service
                var displayData = _paymentService.GetPaymentsForDisplay();
                dgPayments.ItemsSource = displayData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách thanh toán: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load thanh toán theo khách hàng
        /// </summary>
        private void LoadPaymentsByCustomerId(int customerId)
        {
            try
            {
                // Lấy thanh toán theo CustomerId
                var payments = _paymentService.GetPaymentsByCustomerId(customerId);

                // Tạo dữ liệu hiển thị
                var displayData = payments.Select(p => new
                {
                    p.PaymentId,
                    p.AppointmentId,
                    AppointmentInfo = GetAppointmentDisplayInfo(p.Appointment),
                    p.Amount,
                    PaymentDateText = p.PaymentDate.HasValue ?
                        p.PaymentDate.Value.ToString("dd/MM/yyyy") : "Chưa ghi nhận",
                    p.PaymentMethod,
                    p.Status
                }).ToList<dynamic>();

                dgPayments.ItemsSource = displayData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách thanh toán: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tạo thông tin hiển thị cho lịch hẹn
        /// </summary>
        private string GetAppointmentDisplayInfo(Appointment appointment)
        {
            if (appointment == null) return "Không xác định";

            string childName = appointment.Child?.FullName ?? "Không xác định";
            string vaccineName = appointment.Vaccine?.VaccineName ?? "Không xác định";

            return $"{childName} - {vaccineName} ({appointment.AppointmentDate.ToString("dd/MM/yyyy")})";
        }

        #endregion

        #region Xử lý sự kiện tìm kiếm

        /// <summary>
        /// Xử lý khi loại tìm kiếm thay đổi
        /// </summary>
        private void CboSearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSearchType.SelectedItem == null) return;

            string searchType = ((ComboBoxItem)cboSearchType.SelectedItem).Content.ToString();

            // Ẩn tất cả control tìm kiếm
            cboAppointmentSearch.Visibility = Visibility.Collapsed;
            cboStatusSearch.Visibility = Visibility.Collapsed;
            cboMethodSearch.Visibility = Visibility.Collapsed;
            dpSearchDate.Visibility = Visibility.Collapsed;

            // Hiển thị control tìm kiếm phù hợp
            switch (searchType)
            {
                case "Tìm theo lịch hẹn":
                    cboAppointmentSearch.Visibility = Visibility.Visible;
                    break;
                case "Tìm theo trạng thái":
                    cboStatusSearch.Visibility = Visibility.Visible;
                    break;
                case "Tìm theo phương thức":
                    cboMethodSearch.Visibility = Visibility.Visible;
                    break;
                case "Tìm theo ngày":
                    dpSearchDate.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// Tìm kiếm thanh toán
        /// </summary>
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cboSearchType.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn loại tìm kiếm!",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string searchType = ((ComboBoxItem)cboSearchType.SelectedItem).Content.ToString();
                object searchValue = null;

                // Lấy giá trị tìm kiếm tương ứng
                switch (searchType)
                {
                    case "Tìm theo lịch hẹn":
                        if (cboAppointmentSearch.SelectedItem == null)
                        {
                            MessageBox.Show("Vui lòng chọn lịch hẹn!",
                                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        searchValue = cboAppointmentSearch.SelectedValue;
                        break;

                    case "Tìm theo trạng thái":
                        if (cboStatusSearch.SelectedItem == null)
                        {
                            MessageBox.Show("Vui lòng chọn trạng thái!",
                                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        searchValue = ((ComboBoxItem)cboStatusSearch.SelectedItem).Content.ToString();
                        break;

                    case "Tìm theo phương thức":
                        if (cboMethodSearch.SelectedItem == null)
                        {
                            MessageBox.Show("Vui lòng chọn phương thức thanh toán!",
                                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        searchValue = ((ComboBoxItem)cboMethodSearch.SelectedItem).Content.ToString();
                        break;

                    case "Tìm theo ngày":
                        if (dpSearchDate.SelectedDate == null)
                        {
                            MessageBox.Show("Vui lòng chọn ngày!",
                                           "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        searchValue = dpSearchDate.SelectedDate.Value;
                        break;
                }

                // Thực hiện tìm kiếm
                var payments = _paymentService.SearchPayments(searchType, searchValue);

                // Tạo dữ liệu hiển thị
                var displayData = payments.Select(p => new
                {
                    p.PaymentId,
                    p.AppointmentId,
                    AppointmentInfo = GetAppointmentDisplayInfo(p.Appointment),
                    p.Amount,
                    PaymentDateText = p.PaymentDate.HasValue ?
                        p.PaymentDate.Value.ToString("dd/MM/yyyy") : "Chưa ghi nhận",
                    p.PaymentMethod,
                    p.Status
                }).ToList<dynamic>();

                dgPayments.ItemsSource = displayData;

                // Thông báo nếu không có kết quả
                if (!payments.Any())
                {
                    MessageBox.Show("Không tìm thấy kết quả phù hợp!",
                                   "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xóa tìm kiếm và hiển thị tất cả thanh toán
        /// </summary>
        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Reset các control tìm kiếm
                cboSearchType.SelectedIndex = 0;
                cboAppointmentSearch.SelectedIndex = -1;
                cboStatusSearch.SelectedIndex = -1;
                cboMethodSearch.SelectedIndex = -1;
                dpSearchDate.SelectedDate = null;

                // Load lại dữ liệu
                if (App.CurrentAccount?.Role == 3 && App.CurrentAccount?.CustomerId != null)
                {
                    LoadPaymentsByCustomerId(App.CurrentAccount.CustomerId.Value);
                }
                else
                {
                    LoadPayments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa tìm kiếm: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Xử lý sự kiện lựa chọn

        /// <summary>
        /// Xử lý khi chọn lịch hẹn
        /// </summary>
        private void CboAppointment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboAppointment.SelectedValue == null) return;

            try
            {
                // Lấy giá trị AppointmentId
                int appointmentId = (int)cboAppointment.SelectedValue;

                // Tìm thông tin lịch hẹn
                var appointment = _appointmentService.GetAppointmentById(appointmentId);

                if (appointment != null && appointment.Vaccine != null)
                {
                    // Tự động điền số tiền từ giá vắc-xin
                    txtAmount.Text = appointment.Vaccine.Price.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn lịch hẹn: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xử lý khi chọn thanh toán trong DataGrid
        /// </summary>
        private void DgPayments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Vì dùng anonymous type cho hiển thị, cần lấy PaymentId để load lại từ service
                if (dgPayments.SelectedItem != null)
                {
                    // Dùng reflection để lấy PaymentId từ anonymous type
                    var paymentId = (int)dgPayments.SelectedItem.GetType().GetProperty("PaymentId").GetValue(dgPayments.SelectedItem, null);

                    // Lấy payment từ service
                    _selectedPayment = _paymentService.GetPaymentById(paymentId);

                    if (_selectedPayment != null)
                    {
                        // Hiển thị thông tin lên form
                        cboAppointment.SelectedValue = _selectedPayment.AppointmentId;
                        txtAmount.Text = _selectedPayment.Amount.ToString();
                        dpPaymentDate.SelectedDate = _selectedPayment.PaymentDate;

                        // Tìm và chọn PaymentMethod trong ComboBox
                        foreach (ComboBoxItem item in cboPaymentMethod.Items)
                        {
                            if (item.Content.ToString() == _selectedPayment.PaymentMethod)
                            {
                                cboPaymentMethod.SelectedItem = item;
                                break;
                            }
                        }

                        // Tìm và chọn Status trong ComboBox
                        foreach (ComboBoxItem item in cboStatus.Items)
                        {
                            if (item.Content.ToString() == _selectedPayment.Status)
                            {
                                cboStatus.SelectedItem = item;
                                break;
                            }
                        }

                        // Kích hoạt các nút chức năng
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
                MessageBox.Show($"Lỗi khi chọn thanh toán: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Xử lý CRUD

        /// <summary>
        /// Thêm thanh toán mới
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

                // Tạo đối tượng Payment mới
                var payment = new Payment
                {
                    AppointmentId = (int)cboAppointment.SelectedValue,
                    Amount = decimal.Parse(txtAmount.Text),
                    PaymentDate = dpPaymentDate.SelectedDate,
                    PaymentMethod = ((ComboBoxItem)cboPaymentMethod.SelectedItem).Content.ToString(),
                    Status = ((ComboBoxItem)cboStatus.SelectedItem).Content.ToString()
                };

                // Thêm thanh toán
                _paymentService.AddPayment(payment);

                MessageBox.Show("Thêm thanh toán thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadPayments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm thanh toán: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cập nhật thanh toán
        /// </summary>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedPayment == null)
                {
                    MessageBox.Show("Vui lòng chọn thanh toán để cập nhật",
                                   "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (!ValidateInput(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật thông tin
                _selectedPayment.AppointmentId = (int)cboAppointment.SelectedValue;
                _selectedPayment.Amount = decimal.Parse(txtAmount.Text);
                _selectedPayment.PaymentDate = dpPaymentDate.SelectedDate;
                _selectedPayment.PaymentMethod = ((ComboBoxItem)cboPaymentMethod.SelectedItem).Content.ToString();
                _selectedPayment.Status = ((ComboBoxItem)cboStatus.SelectedItem).Content.ToString();

                // Cập nhật thanh toán
                _paymentService.UpdatePayment(_selectedPayment);

                MessageBox.Show("Cập nhật thanh toán thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadPayments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thanh toán: {ex.Message}",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xóa thanh toán
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedPayment == null)
                {
                    MessageBox.Show("Vui lòng chọn thanh toán để xóa",
                                   "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa thanh toán này không?",
                                          "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Xóa thanh toán
                    _paymentService.DeletePayment(_selectedPayment.PaymentId);

                    MessageBox.Show("Xóa thanh toán thành công!", "Thông báo",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadPayments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa thanh toán: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Kiểm tra dữ liệu đầu vào
        /// </summary>
        private bool ValidateInput(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (cboAppointment.SelectedItem == null)
            {
                errorMessage = "Vui lòng chọn lịch hẹn!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                errorMessage = "Vui lòng nhập số tiền!";
                return false;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                errorMessage = "Số tiền phải là số dương!";
                return false;
            }

            if (dpPaymentDate.SelectedDate == null)
            {
                errorMessage = "Vui lòng chọn ngày thanh toán!";
                return false;
            }

            if (cboPaymentMethod.SelectedItem == null)
            {
                errorMessage = "Vui lòng chọn phương thức thanh toán!";
                return false;
            }

            if (cboStatus.SelectedItem == null)
            {
                errorMessage = "Vui lòng chọn trạng thái thanh toán!";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Làm mới form
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        /// <summary>
        /// Xóa dữ liệu form
        /// </summary>
        private void ClearForm()
        {
            cboAppointment.SelectedIndex = -1;
            txtAmount.Text = string.Empty;
            dpPaymentDate.SelectedDate = DateTime.Now;
            cboPaymentMethod.SelectedIndex = 0;
            cboStatus.SelectedIndex = 0;

            _selectedPayment = null;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        #endregion

        #region Xử lý điều hướng

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
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                                         "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);

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

        #endregion
    }
}
