using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repos
{
    public class PostVaccinationRecordRepository
    {
        private readonly VaccineManagementSystem1Context _context;

        public PostVaccinationRecordRepository()
        {
            _context = new VaccineManagementSystem1Context();
        }

        // Lấy tất cả hồ sơ tiêm chủng với thông tin chi tiết
        public List<PostVaccinationRecord> GetAllRecords()
        {
            try
            {
                return _context.PostVaccinationRecords
                    .Include(r => r.Child)
                    .Include(r => r.Vaccine)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách hồ sơ tiêm chủng: {ex.Message}");
                return new List<PostVaccinationRecord>();
            }
        }

        // Lấy hồ sơ theo ID
        public PostVaccinationRecord GetRecordById(int id)
        {
            try
            {
                return _context.PostVaccinationRecords
                    .Include(r => r.Child)
                    .Include(r => r.Vaccine)
                    .FirstOrDefault(r => r.RecordId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy hồ sơ tiêm chủng theo ID: {ex.Message}");
                return null;
            }
        }

        // Lấy hồ sơ theo ChildId
        public List<PostVaccinationRecord> GetRecordsByChildId(int childId)
        {
            try
            {
                return _context.PostVaccinationRecords
                    .Include(r => r.Child)
                    .Include(r => r.Vaccine)
                    .Where(r => r.ChildId == childId)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy hồ sơ tiêm chủng theo trẻ em: {ex.Message}");
                return new List<PostVaccinationRecord>();
            }
        }

        // Thêm hồ sơ mới
        public bool AddRecord(PostVaccinationRecord record)
        {
            try
            {
                _context.PostVaccinationRecords.Add(record);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm hồ sơ tiêm chủng: {ex.Message}");
                return false;
            }
        }

        // Cập nhật hồ sơ
        public bool UpdateRecord(PostVaccinationRecord record)
        {
            try
            {
                var existingRecord = _context.PostVaccinationRecords.FirstOrDefault(r => r.RecordId == record.RecordId);
                if (existingRecord == null)
                    return false;

                _context.Entry(existingRecord).CurrentValues.SetValues(record);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật hồ sơ tiêm chủng: {ex.Message}");
                return false;
            }
        }

        // Xóa hồ sơ
        public bool DeleteRecord(int id)
        {
            try
            {
                var record = _context.PostVaccinationRecords.FirstOrDefault(r => r.RecordId == id);
                if (record == null)
                    return false;

                _context.PostVaccinationRecords.Remove(record);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa hồ sơ tiêm chủng: {ex.Message}");
                return false;
            }
        }

        // Tìm kiếm theo tên trẻ
        public List<PostVaccinationRecord> SearchRecordsByChildName(string childName)
        {
            try
            {
                return _context.PostVaccinationRecords
                    .Include(r => r.Child)
                    .Include(r => r.Vaccine)
                    .Where(r => r.Child.FullName.Contains(childName))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm hồ sơ theo tên trẻ: {ex.Message}");
                return new List<PostVaccinationRecord>();
            }
        }

        // Tìm kiếm theo tên vắc-xin
        public List<PostVaccinationRecord> SearchRecordsByVaccineName(string vaccineName)
        {
            try
            {
                return _context.PostVaccinationRecords
                    .Include(r => r.Child)
                    .Include(r => r.Vaccine)
                    .Where(r => r.Vaccine.VaccineName.Contains(vaccineName))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm hồ sơ theo tên vắc-xin: {ex.Message}");
                return new List<PostVaccinationRecord>();
            }
        }

        // Tìm kiếm theo ngày
        public List<PostVaccinationRecord> SearchRecordsByDate(DateTime date)
        {
            try
            {
                return _context.PostVaccinationRecords
                    .Include(r => r.Child)
                    .Include(r => r.Vaccine)
                    .Where(r => r.ReportDate.HasValue && r.ReportDate.Value.Date == date.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm kiếm hồ sơ theo ngày: {ex.Message}");
                return new List<PostVaccinationRecord>();
            }
        }

        // Lấy hồ sơ theo CustomerId (cho khách hàng chỉ xem hồ sơ của con mình)
        public List<PostVaccinationRecord> GetRecordsByCustomerId(int customerId)
        {
            try
            {
                return _context.PostVaccinationRecords
                    .Include(r => r.Child)
                    .Include(r => r.Vaccine)
                    .Where(r => r.Child.CustomerId == customerId)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy hồ sơ theo khách hàng: {ex.Message}");
                return new List<PostVaccinationRecord>();
            }
        }
    }
}
