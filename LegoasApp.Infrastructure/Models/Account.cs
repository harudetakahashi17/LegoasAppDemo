using System;
using System.Collections.Generic;

namespace LegoasApp.Infrastructure.Models
{
    public partial class Account
    {
        public Account()
        {
            AccountRoles = new HashSet<AccountRole>();
        }

        public int Id { get; set; }
        public string AccountName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool RowStatus { get; set; }

        public virtual ICollection<AccountRole> AccountRoles { get; set; }
    }
}
