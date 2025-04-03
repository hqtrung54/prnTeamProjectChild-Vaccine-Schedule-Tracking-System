using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Models;
using DAL.repos;

namespace BLL.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly AppointmentRepository _appointmentRepository;

        public PaymentService()
        {
            _paymentRepository = new PaymentRepository();
            _appointmentRepository = new AppointmentRepository();
        }

        // Lấy tất cả thanh toán
        public IEnumerable<Payment> GetAllPayments()
        {
            return _paymentRepository.GetAllPayments();
        }

        // Lấy thanh toán theo ID
        public Payment GetPaymentById(int id)
        {
            return _paymentRepository.GetPaymentById(id);
        }

        // Lấy thanh toán theo AppointmentId
        public IEnumerable<Payment> GetPaymentsByAppointmentId(int appointmentId)
        {
            return _paymentRepository.GetPaymentsByAppointmentId(appointmentId);
        }

        // Thêm thanh toán với xác thực
        public void AddPayment(Payment payment)
        {
            ValidatePayment(payment);

            // Kiểm tra trùng lặp thanh toán
            var existingPayments = _paymentRepository.GetPaymentsByAppointmentId(payment.AppointmentId);
            if (existingPayments.Any(p => p.Status == "Completed"))
            {
                throw new ArgumentException("Lịch hẹn này đã có thanh toán hoàn tất.");
            }

            // Nếu không có lỗi, thêm thanh toán
            _paymentRepository.AddPayment(payment);

            // Nếu thanh toán thành công (Completed), cập nhật trạng thái lịch hẹn
            if (payment.Status == "Completed")
            {
                UpdateAppointmentStatusAfterPayment(payment.AppointmentId);
            }
        }

        // Cập nhật thanh toán với xác thực
        public void UpdatePayment(Payment payment)
        {
            ValidatePayment(payment);

            // Lấy thanh toán hiện tại để kiểm tra thay đổi trạng thái
            var currentPayment = _paymentRepository.GetPaymentById(payment.PaymentId);

            // Cập nhật thanh toán
            _paymentRepository.UpdatePayment(payment);

            // Nếu trạng thái thay đổi thành Completed, cập nhật trạng thái lịch hẹn
            if (currentPayment.Status != "Completed" && payment.Status == "Completed")
            {
                UpdateAppointmentStatusAfterPayment(payment.AppointmentId);
            }
        }

        // Xóa thanh toán
        public void DeletePayment(int paymentId)
        {
            _paymentRepository.DeletePayment(paymentId);
        }

        // Tìm kiếm thanh toán
        public IEnumerable<Payment> SearchPayments(string searchType, object searchValue)
        {
            if (searchValue == null)
                return GetAllPayments();

            switch (searchType)
            {
                case "Tìm theo lịch hẹn":
                    int appointmentId = Convert.ToInt32(searchValue);
                    return _paymentRepository.GetPaymentsByAppointmentId(appointmentId);

                case "Tìm theo trạng thái":
                    return _paymentRepository.SearchPaymentsByStatus(searchValue.ToString());

                case "Tìm theo phương thức":
                    return _paymentRepository.SearchPaymentsByMethod(searchValue.ToString());

                case "Tìm theo ngày":
                    DateTime searchDate = (DateTime)searchValue;
                    return _paymentRepository.SearchPaymentsByDate(searchDate);

                default:
                    return GetAllPayments();
            }
        }

        // Lấy thanh toán theo CustomerId (phân quyền)
        public IEnumerable<Payment> GetPaymentsByCustomerId(int customerId)
        {
            return _paymentRepository.GetPaymentsByCustomerId(customerId);
        }

        // Lấy danh sách lịch hẹn cho ComboBox
        public IEnumerable<Appointment> GetAppointmentsForComboBox()
        {
            return _appointmentRepository.GetAllAppointments();
        }

        // Lấy danh sách lịch hẹn theo CustomerId (cho khách hàng)
        public IEnumerable<Appointment> GetAppointmentsForComboBoxByCustomerId(int customerId)
        {
            return _appointmentRepository.GetAllAppointments()
                .Where(a => a.CustomerId == customerId);
        }

        // Xác thực dữ liệu thanh toán
        private void ValidatePayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            // Kiểm tra AppointmentId
            if (payment.AppointmentId <= 0)
            {
                throw new ArgumentException("Vui lòng chọn lịch hẹn.");
            }

            // Kiểm tra Amount
            if (payment.Amount <= 0)
            {
                throw new ArgumentException("Số tiền thanh toán phải lớn hơn 0.");
            }

            // Kiểm tra PaymentMethod
            if (string.IsNullOrEmpty(payment.PaymentMethod))
            {
                throw new ArgumentException("Vui lòng chọn phương thức thanh toán.");
            }

            // Kiểm tra Status
            if (string.IsNullOrEmpty(payment.Status))
            {
                throw new ArgumentException("Vui lòng chọn trạng thái thanh toán.");
            }

            var validStatuses = new List<string> { "Pending", "Completed", "Failed", "Refunded" };
            if (!validStatuses.Contains(payment.Status))
            {
                throw new ArgumentException("Trạng thái không hợp lệ. Trạng thái phải là: Pending, Completed, Failed hoặc Refunded.");
            }
        }

        // Cập nhật trạng thái lịch hẹn sau khi thanh toán thành công
        private void UpdateAppointmentStatusAfterPayment(int appointmentId)
        {
            var appointment = _appointmentRepository.GetAllAppointments()
                .FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appointment != null && appointment.Status.ToLower() != "completed")
            {
                appointment.Status = "Completed";
                _appointmentRepository.UpdateAppointment(appointment);
            }
        }

        // Tạo đối tượng hiển thị cho DataGrid với thông tin chi tiết
        public List<dynamic> GetPaymentsForDisplay()
        {
            var payments = GetAllPayments();

            return payments.Select(p => new
            {
                p.PaymentId,
                p.AppointmentId,
                AppointmentInfo = GetAppointmentDisplayInfo(p.Appointment),
                p.Amount,
                PaymentDateText = p.PaymentDate.HasValue ?
                    p.PaymentDate.Value.ToString("dd/MM/yyyy") : "Chưa ghi nhận",
                p.PaymentMethod,
                p.Status
            }).ToList<dynamic>();
        }

        // Tạo thông tin hiển thị cho Appointment
        private string GetAppointmentDisplayInfo(Appointment appointment)
        {
            if (appointment == null) return "Không xác định";

            string customerName = appointment.Customer?.CustomerFullName ?? "Không xác định";
            string childName = appointment.Child?.FullName ?? "Không xác định";
            string vaccineName = appointment.Vaccine?.VaccineName ?? "Không xác định";

            return $"{childName} - {vaccineName} ({appointment.AppointmentDate.ToString("dd/MM/yyyy")})";
        }
    }
}
