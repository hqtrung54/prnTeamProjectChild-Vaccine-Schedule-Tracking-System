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

        public AccountService(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Lấy tất cả tài khoản
        public async Task<List<Account>> GetAllAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        // Lấy tài khoản theo ID
        public async Task<Account> GetByIdAsync(int accountId)
        {
            return await _accountRepository.GetByIdAsync(accountId);
        }

        // Lấy tài khoản theo Email
        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _accountRepository.GetByEmailAsync(email);
        }

        // Thêm tài khoản
        public async Task AddAsync(Account account)
        {
            await _accountRepository.AddAsync(account);
        }

        // Cập nhật tài khoản
        public async Task UpdateAsync(Account account)
        {
            await _accountRepository.UpdateAsync(account);
        }

        // Xóa tài khoản
        public async Task DeleteAsync(int accountId)
        {
            await _accountRepository.DeleteAsync(accountId);
        }
    }
}
