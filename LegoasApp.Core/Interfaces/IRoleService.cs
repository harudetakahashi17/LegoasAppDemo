using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAll();
        Task<Role> GetById(int id);
        void Add(Role role);
        void Delete(int id, string userLogin);
        Role Update(int id, Role role, string userLogin);
    }
}
