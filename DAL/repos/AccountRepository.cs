using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repos
{
    public class AccountRepository
    {
        private readonly VaccineManagementSystem1Context _context;

        public AccountRepository(VaccineManagementSystem1Context context)
        {
            _context = context;
        }

        // Lấy tất cả tài khoản
        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        // Lấy tài khoản theo ID
        public async Task<Account> GetByIdAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.Customer)  // Lấy thông tin khách hàng liên quan
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        // Lấy tài khoản theo Email
        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        // Thêm tài khoản
        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        // Cập nhật tài khoản
        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        // Xóa tài khoản
        public async Task DeleteAsync(int accountId)
        {
            var account = await GetByIdAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}
