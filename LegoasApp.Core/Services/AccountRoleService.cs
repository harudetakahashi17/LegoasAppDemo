using LegoasApp.Core.Interfaces;
using LegoasApp.Infrastructure.Data;
using LegoasApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Services
{
    public class AccountRoleService : IAccountRoleService
    {
        private LegoasAppContext _context;
        private ILogger<AccountRoleService> _logger;

        public AccountRoleService(LegoasAppContext context, ILogger<AccountRoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(AccountRole accountRole)
        {
            try
            {
                var accRole = _context.AccountRoles.FirstOrDefault(x => x.AccountId == accountRole.AccountId && x.RoleId == accountRole.RoleId && x.RowStatus);
                if (accRole != null)
                {
                    throw new Exception("This account already has assigned role");
                }

                _context.AccountRoles.Add(accountRole);
                _context.SaveChanges();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to save");
            }
        }

        public void Add(List<AccountRole> accountRoles)
        {
            try
            {
                foreach (var accountRole in accountRoles)
                {
                    var accRole = _context.AccountRoles.FirstOrDefault(x => x.AccountId == accountRole.AccountId && x.RoleId == accountRole.RoleId && x.RowStatus);
                    if (accRole != null)
                    {
                        throw new Exception("This account already has assigned role");
                    }
                }

                _context.AccountRoles.AddRange(accountRoles);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save");
            }
        }

        public void Delete(int accountId, int roleId)
        {
            try
            {
                var accRole = _context.AccountRoles.FirstOrDefault(x => x.AccountId == accountId && x.RoleId == roleId && x.RowStatus);
                if(accRole == null)
                {
                    throw new Exception("Invalid account role");
                }

                _context.AccountRoles.Remove(accRole);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete");
            }
        }

        public async Task<IEnumerable<AccountRole>> GetByAccountId(int id)
        {
            return await _context.AccountRoles.Where(x => x.AccountId == id && x.RowStatus).ToListAsync();
        }

        public void Update(List<AccountRole> accountRoles)
        {
            try
            {
                int accountId = accountRoles.First().AccountId;
                var tobeDeleted = _context.AccountRoles.Where(x => x.AccountId == accountId).ToList();
                _context.AccountRoles.RemoveRange(tobeDeleted);

                _context.AccountRoles.AddRange(accountRoles);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update");
            }
        }
    }
}
