using System;

namespace BCM.Models.BCM.DbModel
{
    /// <summary>
    /// 发放记录视图
    /// </summary>
    public class Vw_GiveRecord
    {
        public string GiveRecordId { get; private set; }
        public DateTime? AddAt { get; private set; }
        public string GiveMoneyOrNumType { get; private set; }
        public double? GiveMoneyOrNum { get; private set; }
        public string Remarks { get; private set; }
        public string JobId { get; private set; }
        public string UseType { get; private set; }
        public string GiveType { get; private set; }
    }
}