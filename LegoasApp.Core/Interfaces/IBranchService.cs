using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<Branch>> GetAll();
        Task<Branch> GetbyId(int id);
        void Add(Branch branch);
        void Delete(int id, string userLogin);
        Branch Update(int id, Branch branch, string userLogin);
    }
}
