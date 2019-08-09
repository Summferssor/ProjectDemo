using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class TbRole
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
