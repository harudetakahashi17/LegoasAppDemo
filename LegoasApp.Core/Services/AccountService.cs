using LegoasApp.Core.Interfaces;
using LegoasApp.Infrastructure.Data;
using LegoasApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Services
{
    public class AccountService : IAccountService
    {
        private LegoasAppContext _context;
        private ILogger<AccountService> _logger;

        public AccountService(LegoasAppContext context, ILogger<AccountService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Account account)
        {
            try
            {
                var acc = _context.Accounts.Where(x => x.AccountName.ToUpper() == account.AccountName.ToUpper()).FirstOrDefault();
                if(acc != null)
                {
                    throw new Exception("Account name already exist");
                }
                _context.Add(account);
                _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to save data");
            }
        }

        public void Delete(int id, string userLogin)
        {
            try
            {
                var acc = _context.Accounts.Where(x => x.Id == id && x.RowStatus).FirstOrDefault();
                if(acc == null)
                {
                    throw new Exception("Account not found");
                }

                acc.RowStatus = false;
                acc.ModifiedDate = DateTime.Now;
                acc.ModifiedBy = userLogin;

                _context.Update(acc);
                _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to save data");
            }
        }

        public async Task<IEnumerable<Account>> GetAll()
        {
            return await _context.Accounts.Where(x => x.RowStatus).ToListAsync();
        }

        public Account GetById(int id)
        {
            return _context.Accounts.Where(x => x.Id == id && x.RowStatus).FirstOrDefault();
        }

        public Account Update(int id, Account account)
        {
            Account acc = _context.Accounts.Where(x => x.Id == id && x.RowStatus).FirstOrDefault();
            
            if(acc == null)
            {
                throw new Exception("Invalid user");
            }

            acc.AccountName = account.AccountName;
            acc.Password = account.Password;
            acc.ModifiedBy = account.AccountName;
            acc.ModifiedDate = DateTime.Now;
            try
            {
                _context.Update(acc);
                _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to update data");
                return account;
            }

            return acc;
        }

        public async Task<Account> Login(string accountName, string password)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.AccountName.ToUpper() == accountName.ToUpper() 
            && x.Password == password && x.RowStatus);
        }
    }
}
