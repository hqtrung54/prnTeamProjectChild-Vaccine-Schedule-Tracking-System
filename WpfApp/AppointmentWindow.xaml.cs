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
using BLL;
using DAL.Models;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for AppointmentWindow.xaml
    /// </summary>
    public partial class AppointmentWindow : Window
    {
        private readonly AppointmentServices _appointmentServices;
        public AppointmentWindow()
        {
            InitializeComponent();
            _appointmentServices = new AppointmentServices();
            LoadAppointments();
        }


        // Phương thức để tải danh sách Appointment vào DataGrid
        private void LoadAppointments()
        {
            var appointments = _appointmentServices.GetAllAppointments();
            dgAppointments.ItemsSource = appointments;
        }

        // Phương thức thêm cuộc hẹn mới
        private void BtnAddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu nhập vào có hợp lệ không
                if (string.IsNullOrEmpty(txtCustomerId.Text) || !int.TryParse(txtCustomerId.Text, out int customerId))
                {
                    MessageBox.Show("CustomerId phải là số và không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtChildId.Text) || !int.TryParse(txtChildId.Text, out int childId))
                {
                    MessageBox.Show("ChildId phải là số và không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtVaccineId.Text) || !int.TryParse(txtVaccineId.Text, out int vaccineId))
                {
                    MessageBox.Show("VaccineId phải là số và không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtStatus.Text))
                {
                    MessageBox.Show("Trạng thái không được để trống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (txtStatus.Text.Any(char.IsDigit)) // Kiểm tra trạng thái có chứa số không
                {
                    MessageBox.Show("Trạng thái không được chứa số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tạo đối tượng Appointment từ dữ liệu nhập vào
                var appointment = new Appointment
                {
                    CustomerId = customerId,
                    ChildId = childId,
                    VaccineId = vaccineId,
                    AppointmentDate = dpAppointmentDate.SelectedDate ?? DateTime.Now,
                    Status = txtStatus.Text
                };

                // Thêm cuộc hẹn
                _appointmentServices.AddAppointment(appointment);

                // Thông báo thành công và tải lại danh sách
                MessageBox.Show("Thêm cuộc hẹn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAppointments();
            }
            catch (ArgumentException ex)
            {
                // Xử lý ngoại lệ do lỗi từ AppointmentServices (như ChildId đã tồn tại hoặc CustomerId không khớp)
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ chung
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức xóa cuộc hẹn
        private void BtnDeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgAppointments.SelectedItem != null)
                {
                    var selectedAppointment = dgAppointments.SelectedItem as Appointment;
                    if (selectedAppointment != null)
                    {
                        _appointmentServices.DeleteAppointment(selectedAppointment.AppointmentId);
                        MessageBox.Show("Xóa cuộc hẹn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAppointments();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức cập nhật cuộc hẹn
        // Phương thức cập nhật cuộc hẹn
        private void BtnUpdateAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgAppointments.SelectedItem != null)
                {
                    var selectedAppointment = dgAppointments.SelectedItem as Appointment;
                    if (selectedAppointment != null)
                    {
                        // Cập nhật thông tin cuộc hẹn
                        selectedAppointment.CustomerId = int.Parse(txtEditCustomerId.Text);
                        selectedAppointment.ChildId = int.Parse(txtEditChildId.Text);
                        selectedAppointment.VaccineId = int.Parse(txtEditVaccineId.Text);
                        selectedAppointment.AppointmentDate = dpEditAppointmentDate.SelectedDate ?? DateTime.Now;
                        selectedAppointment.Status = txtEditStatus.Text;

                        // Cập nhật cuộc hẹn
                        _appointmentServices.UpdateAppointment(selectedAppointment);

                        // Thông báo thành công và tải lại danh sách
                        MessageBox.Show("Cập nhật cuộc hẹn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAppointments();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                // Xử lý ngoại lệ từ AppointmentServices (như ChildId đã tồn tại hoặc CustomerId không khớp)
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException ex)
            {
                // Xử lý lỗi khi nhập không đúng định dạng (ví dụ như CustomerId, ChildId không phải là số)
                MessageBox.Show($"Lỗi định dạng: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ chung khác
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức tìm kiếm cuộc hẹn theo trạng thái
        private void BtnSearchAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var status = txtSearchAppointment.Text;
                var appointments = _appointmentServices.SearchAppointmentsByStatus(status);
                dgAppointments.ItemsSource = appointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức khi người dùng chọn một cuộc hẹn trong DataGrid để cập nhật
        private void dgAppointments_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgAppointments.SelectedItem != null)
            {
                var selectedAppointment = dgAppointments.SelectedItem as Appointment;
                if (selectedAppointment != null)
                {
                    txtEditCustomerId.Text = selectedAppointment.CustomerId.ToString();
                    txtEditChildId.Text = selectedAppointment.ChildId.ToString();
                    txtEditVaccineId.Text = selectedAppointment.VaccineId.ToString();
                    dpEditAppointmentDate.SelectedDate = selectedAppointment.AppointmentDate;
                    txtEditStatus.Text = selectedAppointment.Status;
                }
            }
        }

        // Phương thức quay về trang chủ
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();  // Đóng cửa sổ AppointmentWindow và quay lại trang chủ
        }
    }
}
