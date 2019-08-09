using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelCreation
{
    public class RoleCreation
    {
        [MaxLength(32)]
        public string RoleId { get; set; }
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }
        [Required]
        [MaxLength(32)]
        public string ParentRoleId { get; set; }
        [MaxLength(50)]
        public string RoleStatus { get; set; }
        public int? RoleOrderNum { get; set; }
        [MaxLength(200)]
        public string RoleDescription { get; set; }
        public int? RoleLevel { get; set; }
        [MaxLength(200)]
        public string RoleRemarks { get; set; }
    }
}
