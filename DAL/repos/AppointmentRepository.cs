using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.repos
{
    public class AppointmentRepository
    {
        private readonly VaccineManagementSystem1Context _vaccineManagementSystem1Context;

        public AppointmentRepository()
        {
            _vaccineManagementSystem1Context = new VaccineManagementSystem1Context();
        }

        // Phương thức lấy tất cả danh sách Appointment
        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _vaccineManagementSystem1Context.Appointments
                .Include(a => a.Customer)   // Nạp thông tin Customer
                .Include(a => a.Child)      // Nạp thông tin Child
                .Include(a => a.Vaccine)    // Nạp thông tin Vaccine
                .ToList();
        }
        // Phương thức thêm Appointment
        public void AddAppointment(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            // Kiểm tra nếu ChildId đã tồn tại trong cơ sở dữ liệu
            bool childExistsInAppointments = _vaccineManagementSystem1Context.Appointments
            .Any(a => a.ChildId == appointment.ChildId);

            if (childExistsInAppointments)
            {
                throw new ArgumentException("ChildId đã tồn tại trong một cuộc hẹn trước đó. Vui lòng nhập ChildId khác.");
            }

            // Kiểm tra nếu CustomerId và ChildId không khớp
            bool isValidCustomerChild = _vaccineManagementSystem1Context.Children
                .Any(c => c.ChildId == appointment.ChildId && c.CustomerId == appointment.CustomerId);

            if (!isValidCustomerChild)
            {
                throw new ArgumentException("CustomerId và ChildId không khớp. Vui lòng nhập CustomerId khác.");
            }

            // Thêm Appointment vào cơ sở dữ liệu
            _vaccineManagementSystem1Context.Appointments.Add(appointment);
            _vaccineManagementSystem1Context.SaveChanges();
        }

        // Phương thức cập nhật Appointment
        public void UpdateAppointment(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            // Kiểm tra nếu ChildId đã tồn tại trong cơ sở dữ liệu
            bool childExistsInAppointments = _vaccineManagementSystem1Context.Appointments
            .Any(a => a.ChildId == appointment.ChildId);

            if (childExistsInAppointments)
            {
                throw new ArgumentException("ChildId đã tồn tại trong một cuộc hẹn trước đó. Vui lòng nhập ChildId khác.");
            }

            // Kiểm tra nếu CustomerId và ChildId không khớp
            bool isValidCustomerChild = _vaccineManagementSystem1Context.Children
                .Any(c => c.ChildId == appointment.ChildId && c.CustomerId == appointment.CustomerId);

            if (!isValidCustomerChild)
            {
                throw new ArgumentException("CustomerId và ChildId không khớp. Vui lòng nhập CustomerId khác.");
            }

            var existingAppointment = _vaccineManagementSystem1Context.Appointments
                .FirstOrDefault(a => a.AppointmentId == appointment.AppointmentId);

            if (existingAppointment != null)
            {
                existingAppointment.CustomerId = appointment.CustomerId;
                existingAppointment.ChildId = appointment.ChildId;
                existingAppointment.VaccineId = appointment.VaccineId;
                existingAppointment.AppointmentDate = appointment.AppointmentDate;
                existingAppointment.Status = appointment.Status;

                _vaccineManagementSystem1Context.SaveChanges();
            }
        }

        // Phương thức xóa Appointment
        public void DeleteAppointment(int appointmentId)
        {
            var appointment = _vaccineManagementSystem1Context.Appointments
                .FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appointment != null)
            {
                _vaccineManagementSystem1Context.Appointments.Remove(appointment);
                _vaccineManagementSystem1Context.SaveChanges();
            }
        }

        // Phương thức tìm kiếm Appointment theo Status
        public IEnumerable<Appointment> SearchAppointmentsByStatus(string status)
        {
            // Nếu không nhập gì thì trả về toàn bộ danh sách Appointment
            if (string.IsNullOrEmpty(status))
            {
                return _vaccineManagementSystem1Context.Appointments.ToList();
            }

            // Tìm kiếm theo phần nhập của Status (sử dụng Contains và ToLower để so sánh không phân biệt chữ hoa chữ thường)
            var statusLower = status.ToLower();

            return _vaccineManagementSystem1Context.Appointments
                .Where(a => a.Status.ToLower().Contains(statusLower))
                .ToList();
        }

        public List<Appointment> GetAppointmentsByCustomerId(int customerId)
        {
            return _vaccineManagementSystem1Context.Appointments
                .Where(a => a.CustomerId == customerId)
                .Include(a => a.Customer)
                .Include(a => a.Child)
                .Include(a => a.Vaccine)
                .ToList();
        }
    }
}
