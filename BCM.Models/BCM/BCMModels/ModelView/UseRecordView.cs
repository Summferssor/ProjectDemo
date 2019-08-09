using System;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class UseRecordView
    {
        public string UseRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        public string GiveCountType { get; set; }
        public int? UseNumber { get; set; }
        public double? UseMoney { get; set; }
        public string Remarks { get; set; }
        public string JobId { get; set; }
    }
}