using System;
using System.ComponentModel.DataAnnotations;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class GiveMoneyModification
    {
        [MaxLength(32)]
        public string GiveRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        [MaxLength(32)]
        public string GiveMoneyType { get; set; }
        public double? GiveMoney1 { get; set; }
        [MaxLength(200)]
        public string Remarks { get; set; }
        [MaxLength(10)]
        public string JobId { get; set; }
    }
}