using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class UseRecord
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
