using LegoasApp.Core.Common;
using LegoasApp.Core.Interfaces;
using LegoasApp.Infrastructure.Data;
using LegoasApp.Infrastructure.DTO;
using LegoasApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public AccountService(LegoasAppContext context, ILogger<AccountService> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        public Account Add(Account account)
        {
            try
            {
                var acc = _context.Accounts.Where(x => x.AccountName.ToUpper() == account.AccountName.ToUpper()).FirstOrDefault();
                if(acc != null)
                {
                    throw new Exception("Account name already exist");
                }

                // Encrypt Password
                PasswordEncryptor passKey = new PasswordEncryptor(_config);
                account.Password = passKey.ConvertToEncrypt(account.Password);

                _context.Accounts.Add(account);
                _context.SaveChanges();

                return account;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to save data");
                return null;
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

        public void Update(int id, Account account)
        {
            Account acc = _context.Accounts.Where(x => x.Id == id && x.RowStatus).FirstOrDefault();
            
            if(acc == null)
            {
                throw new Exception("Invalid user");
            }

            // Encrypt Password
            PasswordEncryptor passKey = new PasswordEncryptor(_config);

            if (!string.IsNullOrEmpty(account.AccountName))
                acc.AccountName = account.AccountName;
            if(!string.IsNullOrEmpty(account.Password))
                acc.Password = passKey.ConvertToEncrypt(account.Password);

            acc.ModifiedBy = account.AccountName;
            acc.ModifiedDate = account.ModifiedDate;

            try
            {
                _context.Accounts.Update(acc);
                _context.SaveChanges();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to update data");
            }
        }

        public async Task<Account> Login(string accountName, string password)
        {
            // Encrypt Password
            PasswordEncryptor passKey = new PasswordEncryptor(_config);
            password = passKey.ConvertToEncrypt(password);

            return await _context.Accounts.FirstOrDefaultAsync(x => x.AccountName.ToUpper() == accountName.ToUpper() 
            && x.Password == password && x.RowStatus);
        }

        public async Task<AccountSettingDTO> GetAccountUser(int accountId)
        {
            // Encrypt Password
            PasswordEncryptor passKey = new PasswordEncryptor(_config);

            return await (from account in _context.Accounts
                          join user in _context.Users on account.Id equals user.AccountId
                          where account.Id == accountId && account.RowStatus && user.RowStatus
                          select new AccountSettingDTO
                          {
                              AccountId = accountId,
                              AccountName = account.AccountName,
                              Address = user.Address,
                              Password = passKey.ConvertToDecrypt(account.Password),
                              PostalCode = user.PostalCode,
                              Province = user.Province,
                              UserId = user.Id,
                              UserName = user.UserName,
                              Branches = (from ub in _context.UserBranches
                                          where ub.UserId == user.Id && ub.RowStatus
                                          select ub.BranchId).ToList(),
                              MenuAccess = (from ma in _context.RoleMenus
                                            join ro in _context.Roles on ma.RoleId equals ro.Id
                                            join ar in _context.AccountRoles on ro.Id equals ar.RoleId
                                            join ac in _context.Accounts on ar.AccountId equals ac.Id
                                            where ac.Id == accountId && ma.RowStatus && ro.RowStatus && ar.RowStatus && ac.RowStatus
                                            select ma.MenuScreenId).Distinct().ToList(),
                              Roles = (from ar in _context.AccountRoles
                                       join ac in _context.Accounts on ar.AccountId equals ac.Id
                                       where ac.Id == accountId
                                       select ar.RoleId).ToList()
                          }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AccountListDTO>> GetAllList()
        {
            var res = await (from acc in _context.Accounts
                             join usr in _context.Users on acc.Id equals usr.AccountId
                             where acc.RowStatus && usr.RowStatus
                             select new AccountListDTO
                             {
                                 Id = acc.Id,
                                 AccountName = acc.AccountName,
                                 BranchNames = usr.BranchName,
                                 CreatedDate = acc.CreatedDate.ToString(),
                                 RoleName = string.Join(", ", (from acr in _context.AccountRoles
                                             join rol in _context.Roles on acr.RoleId equals rol.Id
                                             where acr.RowStatus && rol.RowStatus && acr.AccountId == acc.Id
                                             select rol.RoleName).ToList()),
                                 UserName = usr.UserName
                             }).ToListAsync();

            foreach(var item in res)
            {
                DateTime dt = DateTime.Parse(item.CreatedDate);
                item.CreatedDate = dt.ToString("dd-MM-yyyy");
            }

            return res;
        }
    }
}
