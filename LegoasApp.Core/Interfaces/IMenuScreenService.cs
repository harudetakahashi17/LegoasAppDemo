using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public interface IMenuScreenService
    {
        void Add(MenuScreen menu);

        Task<IEnumerable<MenuScreen>> GetAll();
        Task<MenuScreen> GetById(int id);
        MenuScreen Update(int id, MenuScreen menu, string userLogin);
        void Delete(int id, string userLogin);
    }
}
