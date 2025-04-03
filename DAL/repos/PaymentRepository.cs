using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.repos
{
    public class PaymentRepository
    {
        private readonly VaccineManagementSystem1Context _context;

        public PaymentRepository()
        {
            _context = new VaccineManagementSystem1Context();
        }

        // Lấy tất cả thanh toán kèm thông tin lịch hẹn
        public IEnumerable<Payment> GetAllPayments()
        {
            return _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Child)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Vaccine)
                .ToList();
        }

        // Lấy thanh toán theo ID
        public Payment GetPaymentById(int id)
        {
            return _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Child)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Vaccine)
                .FirstOrDefault(p => p.PaymentId == id);
        }

        // Lấy thanh toán theo AppointmentId
        public IEnumerable<Payment> GetPaymentsByAppointmentId(int appointmentId)
        {
            return _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Child)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Vaccine)
                .Where(p => p.AppointmentId == appointmentId)
                .ToList();
        }

        // Thêm thanh toán
        public void AddPayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            _context.Payments.Add(payment);
            _context.SaveChanges();
        }

        // Cập nhật thanh toán
        public void UpdatePayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            var existingPayment = _context.Payments
                .FirstOrDefault(p => p.PaymentId == payment.PaymentId);

            if (existingPayment != null)
            {
                existingPayment.AppointmentId = payment.AppointmentId;
                existingPayment.Amount = payment.Amount;
                existingPayment.PaymentDate = payment.PaymentDate;
                existingPayment.PaymentMethod = payment.PaymentMethod;
                existingPayment.Status = payment.Status;

                _context.SaveChanges();
            }
        }

        // Xóa thanh toán
        public void DeletePayment(int paymentId)
        {
            var payment = _context.Payments
                .FirstOrDefault(p => p.PaymentId == paymentId);

            if (payment != null)
            {
                _context.Payments.Remove(payment);
                _context.SaveChanges();
            }
        }

        // Tìm kiếm thanh toán theo trạng thái
        public IEnumerable<Payment> SearchPaymentsByStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return GetAllPayments();

            return _context.Payments
                .Include(p => p.Appointment)
                .Where(p => p.Status.ToLower() == status.ToLower())
                .ToList();
        }

        // Tìm kiếm thanh toán theo phương thức
        public IEnumerable<Payment> SearchPaymentsByMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                return GetAllPayments();

            return _context.Payments
                .Include(p => p.Appointment)
                .Where(p => p.PaymentMethod.ToLower() == method.ToLower())
                .ToList();
        }

        // Tìm kiếm thanh toán theo ngày
        public IEnumerable<Payment> SearchPaymentsByDate(DateTime date)
        {
            return _context.Payments
                .Include(p => p.Appointment)
                .Where(p => p.PaymentDate.HasValue &&
                           p.PaymentDate.Value.Date == date.Date)
                .ToList();
        }

        // Lấy thanh toán theo customerId (cho khách hàng xem thanh toán của mình)
        public IEnumerable<Payment> GetPaymentsByCustomerId(int customerId)
        {
            return _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Child)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Vaccine)
                .Where(p => p.Appointment.CustomerId == customerId)
                .ToList();
        }
    }
}
