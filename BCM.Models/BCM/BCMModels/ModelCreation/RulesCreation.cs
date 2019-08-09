using System.ComponentModel.DataAnnotations;

namespace BCM.Models.BCM.BCMModels.ModelCreation
{
    public class RulesCreation
    {
        [MaxLength(32)]
        public string Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string StartTime { get; set; }
        [MaxLength(50)]
        public string EndTime { get; set; }
        public double? Price { get; set; }
        [MaxLength(100)]
        public string Remarks { get; set; }
        public int? Sort { get; set; }
    }
}