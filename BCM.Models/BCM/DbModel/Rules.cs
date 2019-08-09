using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class Rules
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double? Price { get; set; }
        public string Remarks { get; set; }
        public int? Sort { get; set; }
    }
}
