using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DAL.Models;
using BLL.Services;
using System.Text.RegularExpressions;
using DAL.repos;

namespace WpfApp
{
    public partial class ChildWindow : Window
    {
        private readonly ChildService _childService;

        public ChildWindow()
        {
            InitializeComponent();
            _childService = new ChildService();
            LoadChildren();
            ConfigureUIBasedOnRole();
        }

        private void LoadChildren()
        {
            if (App.CurrentAccount == null) return;

            int userRole = App.CurrentAccount.Role ?? 0;
            int customerId = App.CurrentAccount.CustomerId ?? 0; // ID khách hàng đang đăng nhập

            if (userRole == 3) // Nếu là khách hàng thì chỉ load trẻ thuộc về tài khoản khách hàng hiện tại
            {
                dgChilds.ItemsSource = _childService.GetChildrenByCustomerId(customerId);
            }
            else
            {
                // Nếu là Admin hoặc nhân viên thì load toàn bộ danh sách trẻ
                dgChilds.ItemsSource = _childService.GetChildren();
            }
        }


        private void ConfigureUIBasedOnRole() 
        {
            if (App.CurrentAccount == null || !App.CurrentAccount.Role.HasValue) return;

            // Kiểm tra vai trò
            int userRole = App.CurrentAccount.Role.Value;
            if (userRole == 1 || userRole == 2)
            {
                if (btnAddChild != null)
                    btnAddChild.Visibility = Visibility.Visible;
                if(btnDeleteChild != null)
                    btnDeleteChild.Visibility = Visibility.Visible;
                if(btnSearchChild != null)
                    btnSearchChild.Visibility = Visibility.Visible;
                if(btnUpdateChild != null)
                    btnUpdateChild.Visibility = Visibility.Visible;
            }else if(userRole == 3){
                if(btnAddChild != null)
                    btnAddChild.Visibility = Visibility.Hidden;
                if(btnDeleteChild!= null)   
                    btnDeleteChild.Visibility = Visibility.Hidden;
                if( btnSearchChild != null) 
                    btnSearchChild.Visibility = Visibility.Hidden;
                if(btnUpdateChild != null)
                    btnUpdateChild.Visibility = Visibility.Hidden;

            }
        }

        private void BtnAddChild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNewFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtNewCustomerId.Text) ||
                    dpNewBirthday.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(txtNewGender.Text) ||
                    string.IsNullOrWhiteSpace(txtNewMedicalHistory.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!Regex.IsMatch(txtNewFullName.Text, @"^[\p{L}\s]+$"))
                {
                    MessageBox.Show("Tên trẻ chỉ được chứa chữ cái, không được có số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtNewCustomerId.Text, out int customerId))
                {
                    MessageBox.Show("ID khách hàng phải là số hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!_childService.CustomerExists(customerId))
                {
                    MessageBox.Show("ID khách hàng không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (txtNewGender.Text.ToLower() != "male" && txtNewGender.Text.ToLower() != "female")
                {
                    MessageBox.Show("Giới tính chỉ được nhập là 'Male' hoặc 'Female'.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var child = new Child
                {
                    FullName = txtNewFullName.Text,
                    CustomerId = customerId,
                    DateOfBirth = DateOnly.FromDateTime(dpNewBirthday.SelectedDate ?? DateTime.Now),
                    Gender = txtNewGender.Text,
                    MedicalHistory = txtNewMedicalHistory.Text
                };

                _childService.AddChild(child);
                LoadChildren();
                MessageBox.Show("Thêm trẻ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteChild_Click(object sender, RoutedEventArgs e)
        {
            if (dgChilds.SelectedItem is Child selectedChild)
            {
                try
                {
                    _childService.DeleteChild(selectedChild.ChildId);
                    LoadChildren();
                    MessageBox.Show("Xóa trẻ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn trẻ để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnUpdateChild_Click(object sender, RoutedEventArgs e)
        {
            if (dgChilds.SelectedItem is Child selectedChild)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtEditFullName.Text) ||
                        string.IsNullOrWhiteSpace(txtEditCustomerId.Text) ||
                        dpEditBirthday.SelectedDate == null ||
                        string.IsNullOrWhiteSpace(txtEditGender.Text) ||
                        string.IsNullOrWhiteSpace(txtEditMedicalHistory.Text))
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!Regex.IsMatch(txtEditFullName.Text, @"^[\p{L}\s]+$"))
                    {
                        MessageBox.Show("Tên trẻ chỉ được chứa chữ cái, không được có số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!int.TryParse(txtEditCustomerId.Text, out int customerId))
                    {
                        MessageBox.Show("ID khách hàng phải là số hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!_childService.CustomerExists(customerId))
                    {
                        MessageBox.Show("ID khách hàng không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (txtNewGender.Text.ToLower() != "male" && txtNewGender.Text.ToLower() != "female")
                    {
                        MessageBox.Show("Giới tính chỉ được nhập là 'Male' hoặc 'Female'.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    selectedChild.FullName = txtEditFullName.Text;
                    selectedChild.CustomerId = customerId;
                    selectedChild.DateOfBirth = DateOnly.FromDateTime(dpEditBirthday.SelectedDate ?? DateTime.Now);
                    selectedChild.Gender = txtEditGender.Text;
                    selectedChild.MedicalHistory = txtEditMedicalHistory.Text;

                    _childService.UpdateChild(selectedChild);
                    LoadChildren();
                    MessageBox.Show("Cập nhật thông tin trẻ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn trẻ để cập nhật!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSearchChild_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchChild.Text))
            {
                // Nếu không có ID khách hàng nhập vào, sẽ hiển thị tất cả danh sách trẻ
                dgChilds.ItemsSource = _childService.GetChildren();
            }
            else
            {
                if (!int.TryParse(txtSearchChild.Text, out int customerId) || customerId <= 0)
                {
                    MessageBox.Show("Vui lòng nhập ID khách hàng hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    // Nếu có customerId hợp lệ, tìm trẻ theo ID khách hàng
                    dgChilds.ItemsSource = _childService.GetChildrenByCustomerId(customerId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
