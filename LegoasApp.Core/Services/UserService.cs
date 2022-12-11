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
    public class UserService : IUserService
    {
        private LegoasAppContext _context;
        private ILogger<UserService> _logger;

        public UserService(LegoasAppContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public User Add(User user)
        {
            try
            {
                var usac = _context.Users.Where(x => x.AccountId == user.AccountId && x.RowStatus).FirstOrDefault();
                if (usac != null)
                {
                    throw new Exception("This user already has an account");
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                return user;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to save");
                return null;
            }
        }

        public void Delete(int id, string userLogin)
        {
            try
            {
                var usac = _context.Users.FirstOrDefault(x => x.Id == id && x.RowStatus);
                if(usac == null)
                {
                    throw new Exception("Invalid user");
                }

                usac.ModifiedBy = userLogin;
                usac.ModifiedDate = DateTime.Now;
                usac.RowStatus = false;

                _context.Users.Update(usac);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete");
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.Where(x => x.RowStatus).ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus);
        }

        public User Update(int id, User user, string userLogin)
        {
            try
            {
                var usac = _context.Users.Where(x => x.Id == id && x.RowStatus).FirstOrDefault();
                if (usac == null)
                {
                    throw new Exception("Invalid user");
                }

                usac.ModifiedBy = userLogin;
                usac.ModifiedDate = DateTime.Now;
                usac.UserName = user.UserName;
                usac.Address = user.Address;
                usac.BranchName = user.BranchName;
                usac.PostalCode = user.PostalCode;

                _context.Users.Update(usac);
                _context.SaveChangesAsync();

                return usac;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update");
                return user;
            }
        }
    }
}
