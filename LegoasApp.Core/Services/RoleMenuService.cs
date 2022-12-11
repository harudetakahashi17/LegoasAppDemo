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
    public class RoleMenuService : IRoleMenuService
    {
        private LegoasAppContext _context;
        private ILogger<RoleMenuService> _logger;

        public RoleMenuService(LegoasAppContext context, ILogger<RoleMenuService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(List<RoleMenu> roleMenus)
        {
            try
            {
                foreach(var roleMenu in roleMenus)
                {
                    var rom = _context.RoleMenus.FirstOrDefault(x => x.RoleId == roleMenu.RoleId && x.MenuScreenId == roleMenu.MenuScreenId && x.RowStatus);
                    if(rom != null)
                    {
                        throw new Exception("This role already has the assigned menu");
                    }
                }

                _context.RoleMenus.AddRange(roleMenus);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to save");
            }
        }

        public async Task<IEnumerable<RoleMenu>> GetByRoleId(int id)
        {
            return await _context.RoleMenus.Where(x => x.RoleId == id && x.RowStatus).ToListAsync();
        }

        public void Update(List<RoleMenu> roleMenus)
        {
            try
            {
                int roleId = roleMenus.First().RoleId;
                var tobeDeleted = _context.RoleMenus.Where(x => x.RoleId == roleId).ToList();
                _context.RoleMenus.RemoveRange(tobeDeleted);

                _context.AddRange(roleMenus);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save");
            }
        }
    }
}
