namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class TopUpView
    {
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
        public double? LastMonthCarryNumber { get; set; }
        public double? LastMonthCarryMoney { get; set; }
        public double? TopUpNumber { get; set; }
        public double? SubsidiesMoney { get; set; }
        public int? DeductionsBreakfast { get; set; }
        public int? DeductionsLunch { get; set; }
        public int? DeductionsDinner { get; set; }
        public int? DeductionsSnack { get; set; }
        public int? TotalDeductions { get; set; }
        public int? ThisMonthCarryNumber { get; set; }
        public double? ThisMonthCarryMoney { get; set; }

    }
}