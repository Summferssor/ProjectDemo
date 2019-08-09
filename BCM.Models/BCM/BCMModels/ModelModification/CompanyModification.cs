using System.ComponentModel.DataAnnotations;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class CompanyModification
    {
        [MaxLength(32)]
        public string Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(1)]
        public string CountOrNot { get; set; }
        [MaxLength(32)]
        public string ParentId { get; set; }
        [MaxLength(100)]
        public string Remarks { get; set; }
        
        public int? Sort { get; set; }
    }
}