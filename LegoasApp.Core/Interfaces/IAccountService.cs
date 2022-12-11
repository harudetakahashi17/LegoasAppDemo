using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAll();
        Account GetById(int id);
        Account Add(Account account);
        Account Update(int id, Account account);
        void Delete(int id, string userLogin);
        Task<Account> Login(string accountName, string password);
    }
}
