using DAL.Models;
using DAL.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BLL.Service
{
    public class VaccineService
    {
        private readonly VaccineRepository _vaccineRepository;

        public VaccineService()
        {
            _vaccineRepository = new VaccineRepository();
        }

        // Lấy tất cả vắc-xin
        public List<Vaccine> GetAllVaccines()
        {
            return _vaccineRepository.GetAllVaccines();
        }

        // Lấy vắc-xin theo ID
        public Vaccine GetVaccineById(int id)
        {
            return _vaccineRepository.GetVaccineById(id);
        }

        // Thêm vắc-xin mới
        public bool AddVaccine(Vaccine vaccine)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ValidateVaccine(vaccine, out string errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return _vaccineRepository.AddVaccine(vaccine);
        }

        // Cập nhật vắc-xin
        public bool UpdateVaccine(Vaccine vaccine)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ValidateVaccine(vaccine, out string errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return _vaccineRepository.UpdateVaccine(vaccine);
        }

        // Xóa vắc-xin
        public bool DeleteVaccine(int id)
        {
            // Kiểm tra vắc-xin đã được sử dụng chưa
            if (_vaccineRepository.IsVaccineInUse(id))
            {
                throw new Exception("Không thể xóa vắc-xin này vì đã được sử dụng trong hệ thống.");
            }

            return _vaccineRepository.DeleteVaccine(id);
        }

        // Tìm kiếm vắc-xin theo loại
        public List<Vaccine> SearchVaccines(string keyword, string filterType)
        {
            if (string.IsNullOrEmpty(keyword))
                return GetAllVaccines();

            switch (filterType)
            {
                case "Tìm theo tên":
                    return _vaccineRepository.SearchVaccinesByName(keyword);
                case "Tìm theo nhà sản xuất":
                    return _vaccineRepository.SearchVaccinesByManufacturer(keyword);
                case "Tìm theo nhóm tuổi":
                    return _vaccineRepository.SearchVaccinesByAgeGroup(keyword);
                default:
                    // Tìm trên tất cả các trường
                    var nameResults = _vaccineRepository.SearchVaccinesByName(keyword);
                    var manufacturerResults = _vaccineRepository.SearchVaccinesByManufacturer(keyword);
                    var ageGroupResults = _vaccineRepository.SearchVaccinesByAgeGroup(keyword);

                    // Kết hợp và loại bỏ trùng lặp
                    return nameResults
                        .Union(manufacturerResults)
                        .Union(ageGroupResults)
                        .ToList();
            }
        }

        // Phương thức kiểm tra dữ liệu vắc-xin
        private bool ValidateVaccine(Vaccine vaccine, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Kiểm tra tên vắc-xin
            if (string.IsNullOrWhiteSpace(vaccine.VaccineName))
            {
                errorMessage = "Tên vắc-xin không được để trống.";
                return false;
            }

            // Kiểm tra nhà sản xuất
            if (string.IsNullOrWhiteSpace(vaccine.Manufacturer))
            {
                errorMessage = "Nhà sản xuất không được để trống.";
                return false;
            }

            // Kiểm tra mô tả (có thể để trống)

            // Kiểm tra nhóm tuổi
            if (string.IsNullOrWhiteSpace(vaccine.AgeGroup))
            {
                errorMessage = "Nhóm tuổi không được để trống.";
                return false;
            }

            // Kiểm tra giá tiền
            if (vaccine.Price <= 0)
            {
                errorMessage = "Giá tiền phải lớn hơn 0.";
                return false;
            }

            return true;
        }
    }
}