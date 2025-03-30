using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repos
{
    public class AccountRepository
    {
        private readonly VaccineManagementSystem1Context _vaccineManagementSystem1Context;

        public AccountRepository()
        {
            _vaccineManagementSystem1Context = new VaccineManagementSystem1Context();
        }
        
        public Account GetAccount(string email , string password)
        {
            return _vaccineManagementSystem1Context.Accounts.FirstOrDefault(x => x.Email == email && x.PasswordHash == password);
        }

    }
}
