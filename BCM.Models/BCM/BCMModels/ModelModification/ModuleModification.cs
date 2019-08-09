using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class ModuleModification
    {
        [MaxLength(32)]
        public string ModuleId { get; set; }
        [Required]
        [MaxLength(20)]
        public string ModuleName { get; set; }
        [MaxLength(200)]
        public string ModuleUrl { get; set; }
        [MaxLength(32)]
        public string ParentModuleId { get; set; }
        [MaxLength(50)]
        public string ModuleStatus { get; set; }
        public int? ModuleOrderNum { get; set; }
        [MaxLength(200)]
        public string ModuleDescription { get; set; }
        [MaxLength(200)]
        public string ModuleRemarks { get; set; }
        [MaxLength(50)]
        public string ModuleIcon { get; set; }
    }
}
