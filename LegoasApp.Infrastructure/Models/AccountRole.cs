using System;
using System.Collections.Generic;

namespace LegoasApp.Infrastructure.Models
{
    public partial class AccountRole
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool RowStatus { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
