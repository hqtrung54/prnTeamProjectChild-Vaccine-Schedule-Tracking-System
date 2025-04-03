using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.repos
{
    public class ServiceManagerRepository
    {
        private readonly VaccineManagementSystem1Context _vaccineManagementSystem1Context;

        public ServiceManagerRepository()
        {
            _vaccineManagementSystem1Context = new VaccineManagementSystem1Context();
        }

        public List<Service> GetAllServices()
        {
            var services = _vaccineManagementSystem1Context.Services
                .Include(s => s.Vaccine)  // Bao gồm thông tin Vaccine khi truy vấn Service
                .ToList();

            return services;
        }

        public void AddService(Service service)
        {
            // Kiểm tra tên dịch vụ không được để trống và không chứa số
            if (string.IsNullOrWhiteSpace(service.ServiceName))
            {
                throw new ArgumentException("Tên dịch vụ không được để trống.", nameof(service.ServiceName));
            }

            if (service.ServiceName.Any(Char.IsDigit))
            {
                throw new ArgumentException("Tên dịch vụ không được chứa số.", nameof(service.ServiceName));
            }

            // Kiểm tra nhóm đối tượng không được để trống và không chứa số
            if (string.IsNullOrWhiteSpace(service.TargetGroup))
            {
                throw new ArgumentException("Nhóm đối tượng không được để trống.", nameof(service.TargetGroup));
            }

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

            _vaccineManagementSystem1Context.Services.Add(service);
            _vaccineManagementSystem1Context.SaveChanges();
        }


        public void UpdateService(Service service)
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

            var existingService = _vaccineManagementSystem1Context.Services.Find(service.ServiceId);
            if (existingService != null)
            {
                existingService.ServiceName = service.ServiceName;
                existingService.TargetGroup = service.TargetGroup;
                existingService.Description = service.Description;
                existingService.Price = service.Price;
                existingService.VaccineId = service.VaccineId;

                _vaccineManagementSystem1Context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Dịch vụ không tồn tại.", nameof(service.ServiceId));
            }
        }

        public void DeleteService(int serviceId)
        {
            var service = _vaccineManagementSystem1Context.Services.Find(serviceId);
            if (service != null)
            {
                _vaccineManagementSystem1Context.Services.Remove(service);
                _vaccineManagementSystem1Context.SaveChanges();
            }
        }

        public List<Service> SearchByTargetGroupContain(string targetGroup)
        {

            if (string.IsNullOrWhiteSpace(targetGroup))
            {
                return _vaccineManagementSystem1Context.Services.ToList(); // Trả về tất cả dịch vụ nếu không có từ khóa tìm kiếm
            }
            // Chuyển đổi cả targetGroup và TargetGroup trong dịch vụ thành chữ thường để tìm kiếm không phân biệt chữ hoa chữ thường
            targetGroup = targetGroup.ToLower(); // Chuyển chuỗi tìm kiếm thành chữ thường

            return _vaccineManagementSystem1Context.Services
                .Where(service => service.TargetGroup.ToLower().Contains(targetGroup)) // Chuyển TargetGroup của dịch vụ thành chữ thường và kiểm tra chứa chuỗi tìm kiếm
                .ToList();
        }


    }
}
