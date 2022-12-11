using LegoasApp.Core.Interfaces;
using LegoasApp.Infrastructure.Data;
using LegoasApp.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Core.Services
{
    public class BranchService : IBranchService
    {
        private LegoasAppContext _context;
        private ILogger<BranchService> _logger;

        public BranchService(LegoasAppContext context, ILogger<BranchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(Branch branch)
        {
            try
            {
                var br = _context.Branches.FirstOrDefault(x => x.BranchName.ToUpper() == branch.BranchName.ToUpper() 
                && x.BranchCode.ToUpper() == branch.BranchCode.ToUpper() && x.RowStatus);
                if (br != null) 
                {
                    throw new Exception("Branch already exist");
                }

                _context.Branches.Add(branch);
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
                var branch = _context.Branches.FirstOrDefault(x => x.Id == id && x.RowStatus);
                if(branch == null)
                {
                    throw new Exception("Invalid branch");
                }

                branch.ModifiedBy = userLogin;
                branch.ModifiedDate = DateTime.Now;
                branch.RowStatus = false;
                _context.Branches.Update(branch);
                _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
        }

        public async Task<IEnumerable<Branch>> GetAll()
        {
            return await _context.Branches.Where(x => x.RowStatus).ToListAsync();
        }

        public async Task<Branch> GetbyId(int id)
        {
            return await _context.Branches.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus);
        }

        public Branch Update(int id, Branch branch, string userLogin)
        {
            try
            {
                var br = _context.Branches.FirstOrDefault(x => x.Id ==id && x.RowStatus);
                if(br == null)
                {
                    throw new Exception("Invalid Branch");
                }

                br.BranchName = branch.BranchName;
                br.BranchCode = branch.BranchCode;
                br.ModifiedBy = userLogin;
                br.ModifiedDate = DateTime.Now;
                
                _context.Branches.Update(br);
                _context.SaveChangesAsync();

                return br;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to update");
                return branch;
            }
        }
    }
}
