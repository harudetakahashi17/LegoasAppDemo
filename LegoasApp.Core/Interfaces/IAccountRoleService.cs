using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public interface IAccountRoleService
    {
        void Add(AccountRole accountRole);
        void Add(List<AccountRole> accountRoles);
        Task<IEnumerable<AccountRole>> GetByAccountId(int id);
        void Delete(int accountId, int roleId);
        void Update(List<AccountRole> accountRoles);
    }
}
