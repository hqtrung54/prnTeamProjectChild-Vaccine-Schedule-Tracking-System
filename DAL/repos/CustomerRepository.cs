using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.repos
{
    public class CustomerRepository
    {
        private readonly VaccineManagementSystem1Context _vaccineManagementSystem1Context;
        public CustomerRepository()
        {
            _vaccineManagementSystem1Context = new VaccineManagementSystem1Context();
        }

        public List<Customer> GetAllCustomers()
        {
            return _vaccineManagementSystem1Context.Customers.ToList();
        }

        public void AddCustomer(Customer customer,string password )
        {
            if (_vaccineManagementSystem1Context.Accounts.Any(a => a.Email == customer.Email))
            {
                throw new Exception("Email đã tồn tại, vui lòng sử dụng email khác.");
            }
            _vaccineManagementSystem1Context.Customers.Add(customer);
            _vaccineManagementSystem1Context.SaveChanges();

            var account = new Account
            {
                Email = customer.Email,
                PasswordHash = password, // Cần hash password trong thực tế
                Role = 3, 
                CustomerId = customer.CustomerId
            };

            _vaccineManagementSystem1Context.Accounts.Add(account);
            _vaccineManagementSystem1Context.SaveChanges();
        }

        public void DeleteCustomer(int customerId)
        {
            var customer = _vaccineManagementSystem1Context.Customers
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (customer != null)
            {
                _vaccineManagementSystem1Context.Customers.Remove(customer);
                _vaccineManagementSystem1Context.SaveChanges();
            }
        }

        public void UpdateCustomer(Customer customer, string password)
        {
            var existingCustomer = _vaccineManagementSystem1Context.Customers
                .FirstOrDefault(c => c.CustomerId == customer.CustomerId);

            if (existingCustomer != null)
            {
                // Kiểm tra xem Email mới có trùng với email của các Customer khác không
                if (_vaccineManagementSystem1Context.Customers
                        .Any(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId))
                {
                    throw new Exception("Email đã tồn tại, vui lòng sử dụng email khác.");
                }

                // Cập nhật thông tin Customer
                existingCustomer.CustomerFullName = customer.CustomerFullName;
                existingCustomer.Email = customer.Email;
                existingCustomer.PhoneNumber = customer.PhoneNumber;
                existingCustomer.Address = customer.Address;
                existingCustomer.DateOfBirth = customer.DateOfBirth;

                _vaccineManagementSystem1Context.SaveChanges();

                // Kiểm tra xem Customer đã có Account chưa
                var existingAccount = _vaccineManagementSystem1Context.Accounts
                    .FirstOrDefault(a => a.CustomerId == customer.CustomerId);

                // Nếu chưa có Account, tạo mới Account cho Customer
                if (existingAccount == null)
                {
                    var account = new Account
                    {
                        Email = customer.Email,
                        PasswordHash = password, // Cần phải mã hóa mật khẩu trong thực tế
                        Role = 3, // Giả sử 3 là Customer
                        CustomerId = customer.CustomerId
                    };

                    _vaccineManagementSystem1Context.Accounts.Add(account);
                    _vaccineManagementSystem1Context.SaveChanges();
                }
            }
            else
            {
                throw new Exception("Không tìm thấy khách hàng để cập nhật.");
            }
        }
        public List<Customer> GetCustomersByEmailContains(string emailKeyword)
        {
            // Nếu không nhập gì thì trả về toàn bộ danh sách khách hàng
            if (string.IsNullOrWhiteSpace(emailKeyword))
            {
                return _vaccineManagementSystem1Context.Customers.ToList();
            }

            return _vaccineManagementSystem1Context.Customers
                .Where(c => c.Email.ToLower().Contains(emailKeyword.ToLower()))
                .ToList();
        }





    }
}
