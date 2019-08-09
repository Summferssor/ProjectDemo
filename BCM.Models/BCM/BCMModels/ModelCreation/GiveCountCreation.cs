using System;
using System.ComponentModel.DataAnnotations;

namespace BCM.Models.BCM.BCMModels.ModelCreation
{
    public class GiveCountCreation
    {
        [MaxLength(32)]
        public string GiveRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        [MaxLength(32)]
        public string GiveCountType { get; set; }
        public int? GiveNumber { get; set; }
        [MaxLength(200)]
        public string Remarks { get; set; }
        [MaxLength(10)]
        public string JobId { get; set; }
    }
}