using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public  interface IRoleMenuService
    {
        void Add(List<RoleMenu> roleMenus);
        Task<IEnumerable<RoleMenu>> GetByRoleId(int id);
        void Update(List<RoleMenu> roleMenus);
    }
}
