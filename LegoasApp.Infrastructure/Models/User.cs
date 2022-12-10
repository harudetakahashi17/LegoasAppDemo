using System;
using System.Collections.Generic;

namespace LegoasApp.Web.Models
{
    public partial class User
    {
        public User()
        {
            UserBranches = new HashSet<UserBranch>();
        }

        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool RowStatus { get; set; }

        public virtual ICollection<UserBranch> UserBranches { get; set; }
    }
}
