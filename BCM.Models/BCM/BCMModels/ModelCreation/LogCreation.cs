using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BCM.Models.BCM.BCMModels.ModelCreation
{
    public class LogCreation
    {
        [MaxLength(32)]
        public string LogId { get; set; }
        [MaxLength(32)]
        public string LogType { get; set; }
        [MaxLength(20)]
        public string LogOperationUserName { get; set; }
        [MaxLength(50)]
        public string LogOperationIpv4 { get; set; }
        [MaxLength(50)]
        public string LogOperationIpv6 { get; set; }
        public DateTime? LogOperationTime { get; set; }
        [MaxLength(50)]
        public string LogOperationName { get; set; }
        [MaxLength(200)]
        public string LogOperationUrl { get; set; }
        [MaxLength(200)]
        public string LogRemarks { get; set; }
    }
}
