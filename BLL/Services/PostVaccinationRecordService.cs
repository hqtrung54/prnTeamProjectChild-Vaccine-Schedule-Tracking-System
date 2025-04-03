using BLL.Services;
using DAL.Models;
using DAL.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class PostVaccinationRecordService
    {
        private readonly PostVaccinationRecordRepository _recordRepository;
        private readonly ChildService _childService;
        private readonly VaccineService _vaccineService;

        public PostVaccinationRecordService()
        {
            _recordRepository = new PostVaccinationRecordRepository();
            _childService = new ChildService();
            _vaccineService = new VaccineService();
        }

        // Lấy tất cả hồ sơ tiêm chủng
        public List<PostVaccinationRecord> GetAllRecords()
        {
            return _recordRepository.GetAllRecords();
        }

        // Lấy hồ sơ theo ID
        public PostVaccinationRecord GetRecordById(int id)
        {
            return _recordRepository.GetRecordById(id);
        }

        // Lấy hồ sơ theo ChildId
        public List<PostVaccinationRecord> GetRecordsByChildId(int childId)
        {
            return _recordRepository.GetRecordsByChildId(childId);
        }

        // Lấy hồ sơ theo CustomerId (cho khách hàng)
        public List<PostVaccinationRecord> GetRecordsByCustomerId(int customerId)
        {
            return _recordRepository.GetRecordsByCustomerId(customerId);
        }

        // Thêm hồ sơ mới
        public bool AddRecord(PostVaccinationRecord record)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ValidateRecord(record, out string errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return _recordRepository.AddRecord(record);
        }

        // Cập nhật hồ sơ
        public bool UpdateRecord(PostVaccinationRecord record)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ValidateRecord(record, out string errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return _recordRepository.UpdateRecord(record);
        }

        // Xóa hồ sơ
        public bool DeleteRecord(int id)
        {
            return _recordRepository.DeleteRecord(id);
        }

        // Tìm kiếm hồ sơ
        public List<PostVaccinationRecord> SearchRecords(string searchType, string keyword, DateTime? date)
        {
            switch (searchType)
            {
                case "Tìm theo tên trẻ":
                    return _recordRepository.SearchRecordsByChildName(keyword);

                case "Tìm theo vắc-xin":
                    return _recordRepository.SearchRecordsByVaccineName(keyword);

                case "Tìm theo ngày":
                    if (date.HasValue)
                        return _recordRepository.SearchRecordsByDate(date.Value);
                    return new List<PostVaccinationRecord>();

                default:
                    return GetAllRecords();
            }
        }

        // Lấy danh sách trẻ em cho ComboBox
        public List<Child> GetChildrenForComboBox()
        {
            return _childService.GetChildren();
        }

        // Lấy danh sách vắc-xin cho ComboBox
        public List<Vaccine> GetVaccinesForComboBox()
        {
            return _vaccineService.GetAllVaccines();
        }

        // Replace 'RecordDate' with 'ReportDate' in the following method

        // Phương thức kiểm tra dữ liệu hồ sơ
        private bool ValidateRecord(PostVaccinationRecord record, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Kiểm tra ChildId
            if (record.ChildId == null || record.ChildId <= 0)
            {
                errorMessage = "Vui lòng chọn trẻ em.";
                return false;
            }

            // Kiểm tra VaccineId
            if (record.VaccineId == null || record.VaccineId <= 0)
            {
                errorMessage = "Vui lòng chọn vắc-xin.";
                return false;
            }

            // Kiểm tra ReactionDescription (có thể để trống)

            // Kiểm tra ReportDate
            if (record.ReportDate > DateTime.Now)
            {
                errorMessage = "Ngày ghi nhận không thể là ngày trong tương lai.";
                return false;
            }

            // Nếu tất cả các kiểm tra đều thành công
            return true;
        }

        // Tạo đối tượng hiển thị cho DataGrid (có thêm tên trẻ và tên vắc-xin)
        public List<dynamic> GetRecordsForDisplay()
        {
            var records = GetAllRecords();

            return records.Select(r => new
            {
                r.RecordId,
                r.ChildId,
                ChildName = r.Child?.FullName ?? "Không xác định",
                r.VaccineId,
                VaccineName = r.Vaccine?.VaccineName ?? "Không xác định",
                r.ReactionDescription,
                r.ReportDate
            }).ToList<dynamic>();
        }
    }
}
