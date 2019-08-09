using System;
using System.ComponentModel.DataAnnotations;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class UseRecordModification
    {
        [MaxLength(32)]
        public string UseRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        [MaxLength(32)]
        public string GiveCountType { get; set; }
        public int? UseNumber { get; set; }
        public double? UseMoney { get; set; }
        [MaxLength(200)]
        public string Remarks { get; set; }
        [MaxLength(10)]
        public string JobId { get; set; }
    }
}