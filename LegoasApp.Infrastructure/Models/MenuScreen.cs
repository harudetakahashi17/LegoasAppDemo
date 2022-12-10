using System;
using System.Collections.Generic;

namespace LegoasApp.Infrastructure.Models
{
    public partial class MenuScreen
    {
        public MenuScreen()
        {
            RoleMenus = new HashSet<RoleMenu>();
        }

        public int Id { get; set; }
        public string ScreenCode { get; set; } = null!;
        public string ScreenName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool RowStatus { get; set; }

        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
    }
}
