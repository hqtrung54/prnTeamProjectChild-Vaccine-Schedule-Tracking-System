using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL.Models;
using DAL.repos;

namespace BLL.Service
{
    public class CustomerServices
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerServices()
        {
            _customerRepository = new CustomerRepository();
        }
        public List<Customer> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }

        public void AddCustomer(Customer customer, string password)
        {
            _customerRepository.AddCustomer(customer,password);
        }

        public void DeleteCustomer(int customerId)
        {
            _customerRepository.DeleteCustomer(customerId);
        }

        public void UpdateCustomer(Customer customer, string password)
        {
            _customerRepository.UpdateCustomer(customer, password);
        }

        public List<Customer> GetCustomersByEmailContains(string emailKeyword)
        {
            return _customerRepository.GetCustomersByEmailContains(emailKeyword);
        }

        private void ValidateCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerFullName) || !Regex.IsMatch(customer.CustomerFullName, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                throw new ArgumentException("Họ tên không được để trống và chỉ chứa chữ cái.");
            }

            if (string.IsNullOrWhiteSpace(customer.Email) || !customer.Email.Contains("@"))
            {
                throw new ArgumentException("Email không hợp lệ. Phải chứa ký tự '@'.");
            }

            if (string.IsNullOrWhiteSpace(customer.PhoneNumber) || !Regex.IsMatch(customer.PhoneNumber, @"^\d+$"))
            {
                throw new ArgumentException("Số điện thoại chỉ được chứa số.");
            }

            if (string.IsNullOrWhiteSpace(customer.Address))
            {
                throw new ArgumentException("Địa chỉ không được để trống.");
            }
        }


    }
}
