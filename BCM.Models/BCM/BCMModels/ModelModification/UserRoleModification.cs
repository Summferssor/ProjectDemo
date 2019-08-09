using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class UserRoleModification
    {
        [Required]
        [MaxLength(32)]
        public string UserRoleId { get; set; }
        [Required]
        [MaxLength(32)]
        public string UserId { get; set; }
        [Required]
        [MaxLength(32)]
        public string RoleId { get; set; }
    }
}
