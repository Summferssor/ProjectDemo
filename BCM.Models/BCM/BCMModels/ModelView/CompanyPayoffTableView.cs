namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class CompanyPayoffTableView
    {
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public int? NumberOfBreakfast { get; set; }
        public double? BreakfastPrice { get; set; }
        public double? UseMoneyOfBreakfast { get; set; }
        public int? NumberOfLunch { get; set; }
        public double? LunchPrice { get; set; }
        public double? UseMoneyOfLunch { get; set; }
        public int? NumberOfDinner { get; set; }
        public double? DinnerPrice { get; set; }
        public double? UseMoneyOfDinner { get; set; }
        public int? NumberOfMidnightsnack { get; set; }
        public double? MidnightsnackPrice { get; set; }
        public double? UseMoneyOfMidnightsnack { get; set; }
        public double? AmountPayable { get; set; }
        public double? ActualAmount { get; set; }
        public string Remarks { get; set; }
    }
}