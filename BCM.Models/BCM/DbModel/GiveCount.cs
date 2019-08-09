using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class GiveCount
    {
        public string GiveRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        public string GiveCountType { get; set; }
        public int? GiveNumber { get; set; }
        public string Remarks { get; set; }
        public string JobId { get; set; }
    }
}
