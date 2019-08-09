using System;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class GiveCountView
    {
        public string GiveRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        public string GiveCountType { get; set; }
        public int? GiveNumber { get; set; }
        public string Remarks { get; set; }
        public string JobId { get; set; }
    }
}