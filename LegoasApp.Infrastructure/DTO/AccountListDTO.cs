using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Infrastructure.DTO
{
    public class AccountListDTO
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string CreatedDate { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string BranchNames { get; set; }
    }
}
