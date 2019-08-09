using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountOrNot { get; set; }
        public string ParentId { get; set; }
        public string Remarks { get; set; }
        public int? Sort { get; set; }
    }
}
