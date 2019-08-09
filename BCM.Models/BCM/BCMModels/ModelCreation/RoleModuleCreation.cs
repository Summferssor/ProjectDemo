using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelCreation
{
    public class RoleModuleCreation
    {
        [MaxLength(32)]
        public string RoleModuleId { get; set; }
        [MaxLength(32)]
        public string RoleId { get; set; }
        [MaxLength(32)]
        public string ModuleId { get; set; }
    }
}
