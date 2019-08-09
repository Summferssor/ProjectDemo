using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class GiveMoney
    {
        public string GiveRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        public string GiveMoneyType { get; set; }
        public double? GiveMoney1 { get; set; }
        public string Remarks { get; set; }
        public string JobId { get; set; }
    }
}
