using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class TbDictionary
    {
        public string DictionaryId { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryCode { get; set; }
        public string ParentDictionaryCode { get; set; }
        public int? DictionaryOrderNum { get; set; }
        public string DictionaryRemarks { get; set; }
    }
}
