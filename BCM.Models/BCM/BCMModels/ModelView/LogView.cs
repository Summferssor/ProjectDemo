using System;
using System.Collections.Generic;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class LogView
    {
        public string LogId { get; set; }
        public string LogType { get; set; }
        public string LogOperationUserName { get; set; }
        public string LogOperationIpv4 { get; set; }
        public string LogOperationIpv6 { get; set; }
        public DateTime? LogOperationTime { get; set; }
        public string LogOperationName { get; set; }
        public string LogOperationUrl { get; set; }
        public string LogRemarks { get; set; }
    }
}
