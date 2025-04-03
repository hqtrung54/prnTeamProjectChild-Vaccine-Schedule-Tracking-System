using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using DAL.repos;

namespace BLL
{
    public class AppointmentServices
    {
        private readonly AppointmentRepository _appointmentRepository;

        public AppointmentServices()
        {
            _appointmentRepository = new AppointmentRepository();
        }

        // Phương thức lấy tất cả danh sách Appointment
        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _appointmentRepository.GetAllAppointments();
        }

        // Phương thức thêm Appointment
        public void AddAppointment(Appointment appointment)
        {
            // Kiểm tra CustomerId, ChildId, VaccineId là số và không trống
            if (string.IsNullOrEmpty(appointment.CustomerId.ToString()) || !int.TryParse(appointment.CustomerId.ToString(), out _))
            {
                throw new ArgumentException("CustomerId phải là số và không được để trống.");
            }

            if (string.IsNullOrEmpty(appointment.ChildId.ToString()) || !int.TryParse(appointment.ChildId.ToString(), out _))
            {
                throw new ArgumentException("ChildId phải là số và không được để trống.");
            }

            if (string.IsNullOrEmpty(appointment.VaccineId.ToString()) || !int.TryParse(appointment.VaccineId.ToString(), out _))
            {
                throw new ArgumentException("VaccineId phải là số và không được để trống.");
            }

            // Kiểm tra Status không để trống và không chứa số
            if (string.IsNullOrEmpty(appointment.Status))
            {
                throw new ArgumentException("Trạng thái không được để trống.");
            }

            var validStatuses = new List<string> { "completed", "scheduled", "cancelled" };
            if (!validStatuses.Contains(appointment.Status.ToLower()))
            {
                throw new ArgumentException("Trạng thái phải là 'Completed', 'Scheduled' hoặc 'Cancelled'.");
            }

            if (appointment.Status.Any(char.IsDigit)) // Kiểm tra xem trạng thái có chứa số không
            {
                throw new ArgumentException("Trạng thái không được chứa số.");
            }

            // Nếu không có lỗi, thêm Appointment vào cơ sở dữ liệu
            _appointmentRepository.AddAppointment(appointment);
        }

        // Phương thức cập nhật Appointment
        public void UpdateAppointment(Appointment appointment)
        {
            // Kiểm tra CustomerId, ChildId, VaccineId là số và không trống
            if (string.IsNullOrEmpty(appointment.CustomerId.ToString()) || !int.TryParse(appointment.CustomerId.ToString(), out _))
            {
                throw new ArgumentException("CustomerId phải là số và không được để trống.");
            }

            if (string.IsNullOrEmpty(appointment.ChildId.ToString()) || !int.TryParse(appointment.ChildId.ToString(), out _))
            {
                throw new ArgumentException("ChildId phải là số và không được để trống.");
            }

            if (string.IsNullOrEmpty(appointment.VaccineId.ToString()) || !int.TryParse(appointment.VaccineId.ToString(), out _))
            {
                throw new ArgumentException("VaccineId phải là số và không được để trống.");
            }

            // Kiểm tra Status không để trống và không chứa số
            if (string.IsNullOrEmpty(appointment.Status))
            {
                throw new ArgumentException("Trạng thái không được để trống.");
            }

            var validStatuses = new List<string> { "completed", "scheduled", "cancelled" };
            if (!validStatuses.Contains(appointment.Status.ToLower()))
            {
                throw new ArgumentException("Trạng thái phải là 'Completed', 'Scheduled' hoặc 'Cancelled'.");
            }

            if (appointment.Status.Any(char.IsDigit)) // Kiểm tra xem trạng thái có chứa số không
            {
                throw new ArgumentException("Trạng thái không được chứa số.");
            }

            // Nếu không có lỗi, cập nhật Appointment vào cơ sở dữ liệu
            _appointmentRepository.UpdateAppointment(appointment);
        }

        // Phương thức xóa Appointment
        public void DeleteAppointment(int appointmentId)
        {
            _appointmentRepository.DeleteAppointment(appointmentId);
        }

        // Phương thức tìm kiếm Appointment theo Status
        public IEnumerable<Appointment> SearchAppointmentsByStatus(string status)
        {
            return _appointmentRepository.SearchAppointmentsByStatus(status);
        }

        public List<Appointment> GetAppointmentsByCustomerId(int customerId)
        {
            return _appointmentRepository.GetAppointmentsByCustomerId(customerId);
        }
    }
}
