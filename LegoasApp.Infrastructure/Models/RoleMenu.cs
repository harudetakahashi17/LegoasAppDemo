using System;
using System.Collections.Generic;

namespace LegoasApp.Web.Models
{
    public partial class RoleMenu
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuScreenId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool RowStatus { get; set; }

        public virtual MenuScreen MenuScreen { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
