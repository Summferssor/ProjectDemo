namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class SettlementView
    {
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public int? TotalPersonNumber { get; set; }
        public double? PriceOfSettlement { get; set; }
        public double? AmountOfSettlement { get; set; }
        public double? ActualSettlementAmount { get; set; }
    }
}