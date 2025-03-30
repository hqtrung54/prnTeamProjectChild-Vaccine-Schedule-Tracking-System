using DAL.Models;
using DAL.Repos;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;

        public AccountService()
        {
            _accountRepository = new AccountRepository();
        }

        public Account? GetAccount(string email,string password)
        {
            return _accountRepository.GetAccount(email, password);
        }

    }
}
