using System;
using System.Collections.Generic;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class DictionaryView
    {
        public string DictionaryId { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryCode { get; set; }
        public string ParentDictionaryCode { get; set; }
        public int? DictionaryOrderNum { get; set; }
        public string DictionaryRemarks { get; set; }
    }
}
