using System;
using System.Collections.Generic;

namespace LegoasApp.Web.Models
{
    public partial class UserBranch
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool RowStatus { get; set; }

        public virtual Branch Branch { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
