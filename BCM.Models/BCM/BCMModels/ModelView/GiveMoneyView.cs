using System;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class GiveMoneyView
    {
        public string GiveRecordId { get; set; }
        public DateTime? AddAt { get; set; }
        public string GiveMoneyType { get; set; }
        public double? GiveMoney1 { get; set; }
        public string Remarks { get; set; }
        public string JobId { get; set; }
    }
}