using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public interface IUserService
    {
        User Add(User user);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        User Update(int id, User user, string userLogin);
        void Delete(int id, string userLogin);
    }
}
