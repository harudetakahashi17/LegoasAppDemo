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
    public class RoleService : IRoleService
    {
        private LegoasAppContext _context;
        private ILogger<RoleService> _logger;

        public RoleService(LegoasAppContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Role role)
        {
            try
            {
                var ro = _context.Roles.FirstOrDefault(x => x.RoleCode.ToUpper() == role.RoleCode.ToUpper() && x.RoleName.ToUpper() == role.RoleName.ToUpper() && x.RowStatus);
                if (ro != null)
                {
                    throw new Exception("Role is already exist");
                }

                _context.Roles.Add(role);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to save");
            }
        }

        public void Delete(int id, string userLogin)
        {
            try
            {
                var role = _context.Roles.FirstOrDefault(x => x.Id == id && x.RowStatus);
                if(role == null)
                {
                    throw new Exception("Invalid role");
                }

                role.ModifiedDate = DateTime.Now;
                role.ModifiedBy = userLogin;
                role.RowStatus = false;

                _context.Roles.Update(role);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete");
            }
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            return await _context.Roles.Where(x => x.RowStatus).ToListAsync();
        }

        public async Task<Role> GetById(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus);
        }

        public Role Update(int id, Role role, string userLogin)
        {
            try
            {
                var ro = _context.Roles.FirstOrDefault(x => x.Id == role.Id && x.RowStatus);
                if(ro == null)
                {
                    throw new Exception("Invalid role");
                }

                ro.ModifiedDate = DateTime.Now;
                ro.ModifiedBy = userLogin;
                ro.RoleName = role.RoleName;
                ro.RoleCode = role.RoleCode;

                _context.Roles.Update(ro);
                _context.SaveChangesAsync();

                return ro;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update");
                return role;
            }
        }
    }
}
