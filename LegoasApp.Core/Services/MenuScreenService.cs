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
    public class MenuScreenService : IMenuScreenService
    {
        private LegoasAppContext _context;
        private ILogger<MenuScreenService> _logger;

        public MenuScreenService(LegoasAppContext context, ILogger<MenuScreenService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(MenuScreen menu)
        {
            try
            {
                var scr = _context.MenuScreens.FirstOrDefault(x => x.ScreenCode.ToUpper() == menu.ScreenCode.ToUpper()
                && x.ScreenName.ToUpper() == menu.ScreenName.ToUpper() && x.RowStatus);
                if (scr != null)
                {
                    throw new Exception("Menu is already exist");
                }

                _context.MenuScreens.Add(menu);
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
                var scr = _context.MenuScreens.FirstOrDefault(x => x.Id == id && x.RowStatus);
                if(scr == null)
                {
                    throw new Exception("Invalid menu");
                }

                scr.ModifiedBy = userLogin;
                scr.ModifiedDate = DateTime.Now;
                scr.RowStatus = false;

                _context.MenuScreens.Update(scr);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete");
            }
        }

        public async Task<IEnumerable<MenuScreen>> GetAll()
        {
            return await _context.MenuScreens.Where(x => x.RowStatus).ToListAsync();
        }

        public async Task<MenuScreen> GetById(int id)
        {
            return await _context.MenuScreens.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus);
        }

        public MenuScreen Update(int id, MenuScreen menu, string userLogin)
        {
            try
            {
                var scr = _context.MenuScreens.FirstOrDefault(x => x.Id ==id && x.RowStatus);
                if(scr == null)
                {
                    throw new Exception("Invalid menu");
                }

                scr.ModifiedBy = userLogin;
                scr.ModifiedDate = DateTime.Now;
                scr.ScreenName = menu.ScreenName;
                scr.ScreenCode = menu.ScreenCode;

                _context.MenuScreens.Update(scr);
                _context.SaveChangesAsync();

                return scr;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update");
                return menu;
            }
        }
    }
}
