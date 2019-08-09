using System;
using System.ComponentModel.DataAnnotations;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class EmployeeModification
    {
        [MaxLength(10)]
        public string JobId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        public int? Numbers { get; set; }
        [MaxLength(200)]
        public string QrCodeUrl { get; set; }
        [MaxLength(50)]
        public string QrCodeNumber { get; set; }
        [MaxLength(100)]
        public string Remarks { get; set; }

        public DateTime? AddAt { get; set; }
        public int? Sort { get; set; }

        public int? AvailableCount { get; set; }
        public DateTime? UpdateMoneyAt { get; set; }
        public double? AvailableMoney { get; set; }
        public DateTime? UpdateCountAt { get; set; }
        [MaxLength(32)]
        public string CompanyId { get; set; }
    }
}