using LegoasApp.Infrastructure.DTO;
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
        Task<IEnumerable<AccountListDTO>> GetAllList();
        Account GetById(int id);
        Account Add(Account account);
        void Update(int id, Account account);
        void Delete(int id, string userLogin);
        Task<Account> Login(string accountName, string password);
        Task<AccountSettingDTO> GetAccountUser(int accountId);
    }
}
