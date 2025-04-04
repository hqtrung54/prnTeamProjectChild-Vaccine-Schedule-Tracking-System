using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace DAL.repos
{
    public class ChildRepository
    {
        private readonly VaccineManagementSystem1Context _vaccineManagementSystem1Context;

        public ChildRepository()
        {
            _vaccineManagementSystem1Context = new VaccineManagementSystem1Context();
        }

        public List<Child> GetChildren()
        {
            return _vaccineManagementSystem1Context.Children.Include(c => c.Customer).ToList();
        }

        //Add
        public void AddChildren(Child child)
        {
             _vaccineManagementSystem1Context.Add(child);
            _vaccineManagementSystem1Context.SaveChanges();
        }

        //Delete
        public void DeleteChildren(int childId)
        {
            var child = _vaccineManagementSystem1Context.Children.FirstOrDefault(x => x.ChildId == childId);
            if(child != null)
            {
                _vaccineManagementSystem1Context.Remove(child);
                _vaccineManagementSystem1Context.SaveChanges();
            }

        }
        //Update
        public void UpdateChild(Child updateChild)
        {
            var existingChild = _vaccineManagementSystem1Context.Children.Find(updateChild.ChildId);
            {
                existingChild.FullName = updateChild.FullName;
                existingChild.DateOfBirth = updateChild.DateOfBirth;
                existingChild.Gender = updateChild.Gender;
                existingChild.MedicalHistory = updateChild.MedicalHistory;
                _vaccineManagementSystem1Context.SaveChanges();
                
            }
        }
        //Search
        public List<Child> GetChildrenByCustomerId(int customerId)
        {
            return _vaccineManagementSystem1Context.Children
                    .Where(c => c.CustomerId == customerId)
                    .Include(c => c.Customer) // Đảm bảo rằng thông tin khách hàng được tải cùng với trẻ
                    .ToList();
        }

        public bool CustomerExists(int customerId)
        {
            return _vaccineManagementSystem1Context.Customers.Any(c => c.CustomerId == customerId);
        }

    }
}
