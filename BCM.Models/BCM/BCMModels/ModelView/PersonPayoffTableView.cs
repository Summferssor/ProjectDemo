using BCM.Models.BCM.DbModel;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class PersonPayoffTableView
    {
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
        public int? NumberOfBreakfast { get; set; }
        public double? MoneyOfBreakfast { get; set; }
        public int? NumberOfLunch { get; set; }
        public double? MoneyOfLunch { get; set; }
        public int? NumberOfDinner { get; set; }
        public double? MoneyOfDinner { get; set; }
        public int? NumberOfMidnightsnack { get; set; }
        public double? MoneyOfMidnightsnack { get; set; }
        public int? TotalNumber { get; set; }
        public double? TotalMoney { get; set; }

        public int? OtherBreakfastCount { get; set; }
        public double? OtherBreakfastMoney { get; set; }
        public int? OtherLunchCount { get; set; }
        public double? OtherLunchMoney { get; set; }
        public int? OtherDinnerCount { get; set; }
        public double? OtherDinnerMoney { get; set; }
        public int? OtherSnackCount { get; set; }
        public double? OtherSnackMoney { get; set; }
        
    }
}