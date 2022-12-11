using LegoasApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Interfaces
{
    public interface IUserBranchService
    {
        void Add(List<UserBranch> userBranches);
        Task<IEnumerable<UserBranch>> GetByUserId(int id);
        void Update(List<UserBranch> userBranches);
    }
}
