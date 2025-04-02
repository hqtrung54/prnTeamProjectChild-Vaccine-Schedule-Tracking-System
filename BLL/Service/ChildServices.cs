using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DAL.Models;
using DAL.repos;

namespace BLL.Services
{
    public class ChildService
    {
        private readonly ChildRepository _childRepository;

        public ChildService()
        {
            _childRepository = new ChildRepository();
        }

        public List<Child> GetChildren()
        {
            return _childRepository.GetChildren();
        }

        public void AddChild(Child child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child), "Dữ liệu trẻ không được để trống.");

            if (string.IsNullOrWhiteSpace(child.FullName))
                throw new ArgumentException("Tên trẻ không được để trống.");

            if (!Regex.IsMatch(child.FullName, @"^[\p{L}\s]+$"))
                throw new ArgumentException("Tên trẻ chỉ được chứa chữ cái, không được có số.");

            if (child.CustomerId <= 0)
                throw new ArgumentException("ID khách hàng không hợp lệ.");

            if (!_childRepository.CustomerExists(child.CustomerId))
                throw new ArgumentException("Khách hàng không tồn tại trong hệ thống.");

            if (child.DateOfBirth == default)
                throw new ArgumentException("Ngày sinh không được để trống.");

            if (string.IsNullOrWhiteSpace(child.Gender))
                throw new ArgumentException("Giới tính không được để trống.");

            child.Gender = child.Gender.ToLower() == "male" ? "Male" : (child.Gender.ToLower() == "female" ? "Female" : child.Gender);

            if (child.Gender != "Male" && child.Gender != "Female")
                throw new ArgumentException("Giới tính chỉ được nhập là 'Male' hoặc 'Female'.");

            if (string.IsNullOrWhiteSpace(child.MedicalHistory))
                throw new ArgumentException("Tiền sử bệnh không được để trống.");

            _childRepository.AddChildren(child);
        }

        public void DeleteChild(int childId)
        {
            if (childId <= 0)
                throw new ArgumentException("ID trẻ không hợp lệ! Hãy nhập một số dương.");

            _childRepository.DeleteChildren(childId);
        }

        public void UpdateChild(Child child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child), "Dữ liệu trẻ không được để trống.");

            if (child.ChildId <= 0)
                throw new ArgumentException("ID trẻ không hợp lệ! Hãy nhập một số dương.");

            if (string.IsNullOrWhiteSpace(child.FullName))
                throw new ArgumentException("Tên trẻ không được để trống.");

            if (!Regex.IsMatch(child.FullName, @"^[\p{L}\s]+$"))
                throw new ArgumentException("Tên trẻ chỉ được chứa chữ cái, không được có số.");

            if (child.CustomerId <= 0)
                throw new ArgumentException("ID khách hàng không hợp lệ.");

            if (!_childRepository.CustomerExists(child.CustomerId))
                throw new ArgumentException("Khách hàng không tồn tại trong hệ thống.");

            if (child.DateOfBirth == default)
                throw new ArgumentException("Ngày sinh không được để trống.");

            if (string.IsNullOrWhiteSpace(child.Gender))
                throw new ArgumentException("Giới tính không được để trống.");

            child.Gender = child.Gender.ToLower() == "male" ? "Male" : (child.Gender.ToLower() == "female" ? "Female" : child.Gender);

            if (child.Gender != "Male" && child.Gender != "Female")
                throw new ArgumentException("Giới tính chỉ được nhập là 'Male' hoặc 'Female'.");

            if (string.IsNullOrWhiteSpace(child.MedicalHistory))
                throw new ArgumentException("Tiền sử bệnh không được để trống.");

            _childRepository.UpdateChild(child);
        }

        public List<Child> GetChildrenByCustomerId(int customerId)
        {
            // Nếu customerId <= 0 hoặc không nhập gì, trả về toàn bộ danh sách
            if (customerId <= 0)
            {
                return _childRepository.GetChildren(); // Trả về toàn bộ danh sách nếu không có customerId hợp lệ
            }

            // Kiểm tra xem customerId có tồn tại trong hệ thống không
            if (!_childRepository.CustomerExists(customerId))
            {
                throw new ArgumentException("ID khách hàng không tồn tại trong hệ thống!");
            }

            // Trả về danh sách trẻ của khách hàng nếu customerId hợp lệ và tồn tại
            return _childRepository.GetChildrenByCustomerId(customerId);
        }

        public bool CustomerExists(int customerId)
        {
            return _childRepository.CustomerExists(customerId);
        }
    }
}