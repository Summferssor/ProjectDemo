using System;
using System.Collections.Generic;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class ModuleView
    {
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleUrl { get; set; }
        public string ParentModuleId { get; set; }
        public string ModuleStatus { get; set; }
        public int? ModuleOrderNum { get; set; }
        public string ModuleDescription { get; set; }
        public string ModuleRemarks { get; set; }
        public string ModuleIcon { get; set; }
    }
}
