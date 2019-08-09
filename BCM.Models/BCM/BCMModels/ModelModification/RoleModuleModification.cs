using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class RoleModuleModification
    {
        [MaxLength(32)]
        public string RoleModuleId { get; set; }
        [MaxLength(32)]
        public string RoleId { get; set; }
        [MaxLength(32)]
        public string ModuleId { get; set; }
    }
}
