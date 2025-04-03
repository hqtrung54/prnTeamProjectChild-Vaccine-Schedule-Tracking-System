using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.repos;
using DAL.Models;

namespace BLL.Services
{
    public class ServiceManagerServices
    {
        private readonly ServiceManagerRepository _serviceManagerRepository;
        public ServiceManagerServices()
        {
            _serviceManagerRepository = new ServiceManagerRepository();
        }
        // Lấy danh sách tất cả dịch vụ
        public List<Service> GetAllServices()
        {
            return _serviceManagerRepository.GetAllServices();
        }

        // Thêm dịch vụ mới
        public bool AddService(Service service)
        {
            try
            {
                // Kiểm tra tên dịch vụ không được để trống và không chứa số
                if (string.IsNullOrWhiteSpace(service.ServiceName))
                {
                    Console.WriteLine("Tên dịch vụ không được để trống.");
                    return false; // Kiểm tra tên dịch vụ
                }

                if (service.ServiceName.Any(char.IsDigit))
                {
                    Console.WriteLine("Tên dịch vụ không được chứa số.");
                    return false; // Kiểm tra tên dịch vụ chứa số
                }

                // Kiểm tra nhóm đối tượng không được để trống và không chứa số
                if (string.IsNullOrWhiteSpace(service.TargetGroup))
                {
                    Console.WriteLine("Nhóm đối tượng không được để trống.");
                    return false; // Kiểm tra nhóm đối tượng
                }

                if (service.TargetGroup.Any(char.IsDigit))
                {
                    Console.WriteLine("Nhóm đối tượng không được chứa số.");
                    return false; // Kiểm tra nhóm đối tượng chứa số
                }

                // Kiểm tra mô tả dịch vụ không được để trống
                if (string.IsNullOrWhiteSpace(service.Description))
                {
                    Console.WriteLine("Mô tả dịch vụ không thể để trống.");
                    return false; // Kiểm tra mô tả dịch vụ
                }

                // Kiểm tra giá dịch vụ phải là một số dương
                if (service.Price <= 0)
                {
                    Console.WriteLine("Giá dịch vụ phải là một số dương.");
                    return false; // Kiểm tra giá dịch vụ
                }

                // Kiểm tra VaccineId phải là một số dương
                if (service.VaccineId <= 0)
                {
                    Console.WriteLine("VaccineId không hợp lệ.");
                    return false; // Kiểm tra VaccineId
                }

                // Nếu tất cả các kiểm tra đều hợp lệ, thêm dịch vụ vào cơ sở dữ liệu
                _serviceManagerRepository.AddService(service);
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã có lỗi xảy ra: {ex.Message}");
                return false;
            }
        }

        // Cập nhật thông tin dịch vụ
        public bool UpdateService(Service service)
        {
            try
            {
                // Kiểm tra tên dịch vụ không được để trống (không chỉ chứa khoảng trắng)
                if (string.IsNullOrWhiteSpace(service.ServiceName))
                {
                    throw new ArgumentException("Tên dịch vụ không được để trống và không được chỉ chứa khoảng trắng.", nameof(service.ServiceName));
                }

                // Kiểm tra tên dịch vụ không được chứa số
                if (service.ServiceName.Any(Char.IsDigit))
                {
                    throw new ArgumentException("Tên dịch vụ không được chứa số.", nameof(service.ServiceName));
                }

                // Kiểm tra nhóm đối tượng không được để trống (không chỉ chứa khoảng trắng)
                if (string.IsNullOrWhiteSpace(service.TargetGroup))
                {
                    throw new ArgumentException("Nhóm đối tượng không được để trống và không được chỉ chứa khoảng trắng.", nameof(service.TargetGroup));
                }

                // Kiểm tra nhóm đối tượng không được chứa số
                if (service.TargetGroup.Any(Char.IsDigit))
                {
                    throw new ArgumentException("Nhóm đối tượng không được chứa số.", nameof(service.TargetGroup));
                }

                // Kiểm tra mô tả dịch vụ không được để trống
                if (string.IsNullOrWhiteSpace(service.Description))
                {
                    throw new ArgumentException("Mô tả dịch vụ không thể để trống.", nameof(service.Description));
                }

                // Kiểm tra giá dịch vụ phải là một số dương
                if (service.Price <= 0)
                {
                    throw new ArgumentException("Giá dịch vụ phải là một số dương.", nameof(service.Price));
                }

                // Kiểm tra VaccineId phải là một số dương
                if (service.VaccineId <= 0)
                {
                    throw new ArgumentException("VaccineId không hợp lệ.", nameof(service.VaccineId));
                }

                // Nếu tất cả các kiểm tra đều hợp lệ, cập nhật dịch vụ vào cơ sở dữ liệu
                _serviceManagerRepository.UpdateService(service);
                return true;
            }
            catch (ArgumentException ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi cho người dùng
                Console.WriteLine($"Lỗi: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung và trả về thông báo lỗi cho người dùng
                Console.WriteLine($"Đã có lỗi xảy ra: {ex.Message}");
                return false;
            }
        }

        public bool DeleteService(int serviceId)
        {
            try
            {
                if (serviceId <= 0)
                    return false; // Kiểm tra dữ liệu đầu vào

                _serviceManagerRepository.DeleteService(serviceId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã có lỗi xảy ra: {ex.Message}");
                return false;
            }
        }

        public List<Service> SearchByTargetGroup(string targetGroup)
        {
            if (string.IsNullOrWhiteSpace(targetGroup)) // Kiểm tra nếu targetGroup là rỗng
            {
                return _serviceManagerRepository.GetAllServices(); // Trả về tất cả dịch vụ nếu không có từ khóa tìm kiếm
            }

            // Nếu có từ khóa tìm kiếm, gọi phương thức tìm kiếm theo chứa chuỗi (contains) trong repository
            return _serviceManagerRepository.SearchByTargetGroupContain(targetGroup);
        }

    }
}
