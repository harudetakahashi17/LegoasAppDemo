using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoasApp.Infrastructure.DTO
{
    public class AccountSettingDTO
    {
        public int? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? Province { get; set; }
        public List<int>? Branches { get; set; }
        public List<int>? Roles { get; set; }
        public List<int>? MenuAccess { get; set; }
    }
}
