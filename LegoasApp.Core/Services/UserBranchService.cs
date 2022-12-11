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
    public class UserBranchService : IUserBranchService
    {
        private LegoasAppContext _context;
        private ILogger<UserBranchService> _logger;

        public UserBranchService(LegoasAppContext context, ILogger<UserBranchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(List<UserBranch> userBranches)
        {
            try
            {
                foreach(var userBranch in userBranches)
                {
                    var usbr = _context.UserBranches.FirstOrDefault(x => x.UserId == userBranch.UserId && x.BranchId == userBranch.BranchId && x.RowStatus);
                    if(usbr != null)
                    {
                        throw new Exception("User has already in the branch");
                    }
                }

                _context.UserBranches.AddRange(userBranches);
                _context.SaveChanges();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Failed to save");
            }
        }

        public async Task<IEnumerable<UserBranch>> GetByUserId(int id)
        {
            return await _context.UserBranches.Where(x => x.UserId == id && x.RowStatus).ToListAsync();
        }

        public void Update(List<UserBranch> userBranches)
        {
            try
            {
                int userId = userBranches.First().UserId;
                var tobeDeleted = _context.UserBranches.Where(x => x.UserId == userId && x.RowStatus);
                _context.UserBranches.RemoveRange(tobeDeleted);

                _context.UserBranches.AddRange(userBranches);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save");
            }
        }
    }
}
