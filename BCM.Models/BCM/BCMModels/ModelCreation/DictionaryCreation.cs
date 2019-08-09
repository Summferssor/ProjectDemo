using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelCreation
{
    public class DictionaryCreation
    {
        [MaxLength(32)]
        public string DictionaryId { get; set; }
        [MaxLength(50)]
        public string DictionaryName { get; set; }
        [MaxLength(50)]
        public string DictionaryCode { get; set; }
        [MaxLength(50)]
        public string ParentDictionaryCode { get; set; }
        public int? DictionaryOrderNum { get; set; }
        [MaxLength(200)]
        public string DictionaryRemarks { get; set; }
    }
}
