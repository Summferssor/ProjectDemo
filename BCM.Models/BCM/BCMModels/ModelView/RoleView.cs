using System;
using System.Collections.Generic;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class RoleView
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string ParentRoleId { get; set; }
        public string RoleStatus { get; set; }
        public int? RoleOrderNum { get; set; }
        public string RoleDescription { get; set; }
        public int? RoleLevel { get; set; }
        public string RoleRemarks { get; set; }
    }
}
