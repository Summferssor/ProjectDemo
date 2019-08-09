using System;

namespace BCM.Models.BCM.DbModel
{
    public class Vw_UseRecord
    {
        public string UseRecordId { get; private set; }
        public DateTime? AddAt { get; private set; }
        public string GiveCountType { get; private set; }
        public int? UseNumber { get; private set; }
        public double? UseMoney { get; private set; }
        public string Remarks { get; private set; }
        public string JobId { get; private set; }
        public string UseType { get; private set; }
    }
}