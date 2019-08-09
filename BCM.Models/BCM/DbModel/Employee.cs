using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class Employee
    {
        public string JobId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int? Numbers { get; set; }
        public string QrCodeUrl { get; set; }
        public string QrCodeNumber { get; set; }
        public string Remarks { get; set; }
        public DateTime? AddAt { get; set; }
        public int? Sort { get; set; }
        public int? AvailableCount { get; set; }
        public DateTime? UpdateMoneyAt { get; set; }
        public double? AvailableMoney { get; set; }
        public DateTime? UpdateCountAt { get; set; }
        public string CompanyId { get; set; }
    }
}
