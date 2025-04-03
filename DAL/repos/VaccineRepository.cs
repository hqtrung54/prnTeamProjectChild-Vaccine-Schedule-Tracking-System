using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repos
{
    public class VaccineRepository
    {
        private readonly VaccineManagementSystem1Context _context;

        public VaccineRepository()
        {
            _context = new VaccineManagementSystem1Context();
        }

        // Lấy tất cả vắc-xin
        public List<Vaccine> GetAllVaccines()
        {
            try
            {
                return _context.Vaccines.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách vắc-xin: {ex.Message}");
                return new List<Vaccine>();
            }
        }

        // Tìm vắc-xin theo ID
        public Vaccine GetVaccineById(int id)
        {
            try
            {
                return _context.Vaccines.FirstOrDefault(v => v.VaccineId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm vắc-xin theo ID: {ex.Message}");
                return null;
            }
        }

        // Thêm vắc-xin mới
        public bool AddVaccine(Vaccine vaccine)
        {
            try
            {
                _context.Vaccines.Add(vaccine);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm vắc-xin: {ex.Message}");
                return false;
            }
        }

        // Cập nhật vắc-xin
        public bool UpdateVaccine(Vaccine vaccine)
        {
            try
            {
                var existingVaccine = _context.Vaccines.FirstOrDefault(v => v.VaccineId == vaccine.VaccineId);
                if (existingVaccine == null)
                    return false;

                _context.Entry(existingVaccine).CurrentValues.SetValues(vaccine);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật vắc-xin: {ex.Message}");
                return false;
            }
        }

        // Xóa vắc-xin
        public bool DeleteVaccine(int id)
        {
            try
            {
                var vaccine = _context.Vaccines.FirstOrDefault(v => v.VaccineId == id);
                if (vaccine == null)
                    return false;

                _context.Vaccines.Remove(vaccine);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa vắc-xin: {ex.Message}");
                return false;
            }
        }

        // Tìm kiếm vắc-xin theo tên
        public List<Vaccine> SearchVaccinesByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return GetAllVaccines();

                return _context.Vaccines
                    .Where(v => v.VaccineName.Contains(name))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm vắc-xin theo tên: {ex.Message}");
                return new List<Vaccine>();
            }
        }

        // Tìm kiếm vắc-xin theo nhà sản xuất
        public List<Vaccine> SearchVaccinesByManufacturer(string manufacturer)
        {
            try
            {
                if (string.IsNullOrEmpty(manufacturer))
                    return GetAllVaccines();

                return _context.Vaccines
                    .Where(v => v.Manufacturer.Contains(manufacturer))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm vắc-xin theo nhà sản xuất: {ex.Message}");
                return new List<Vaccine>();
            }
        }

        // Tìm kiếm vắc-xin theo nhóm tuổi
        public List<Vaccine> SearchVaccinesByAgeGroup(string ageGroup)
        {
            try
            {
                if (string.IsNullOrEmpty(ageGroup))
                    return GetAllVaccines();

                return _context.Vaccines
                    .Where(v => v.AgeGroup.Contains(ageGroup))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm vắc-xin theo nhóm tuổi: {ex.Message}");
                return new List<Vaccine>();
            }
        }

        // Kiểm tra vắc-xin đã được sử dụng chưa
        public bool IsVaccineInUse(int vaccineId)
        {
            try
            {
                bool usedInAppointments = _context.Appointments
                    .Any(a => a.VaccineId == vaccineId);

                bool usedInRecords = _context.PostVaccinationRecords
                    .Any(p => p.VaccineId == vaccineId);

                return usedInAppointments || usedInRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra vắc-xin đã được sử dụng: {ex.Message}");
                return false;
            }
        }
    }
}
