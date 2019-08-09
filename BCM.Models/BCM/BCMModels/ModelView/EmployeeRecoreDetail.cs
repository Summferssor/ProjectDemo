using System;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class EmployeeRecoreDetail
    {
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentId { get; set; }
        public string JobId { get; set; }
        public string EmployeeName { get; set; }
        public string GiveCountType { get; set; }
        public string ConsumptionType { get; set; }
        public DateTime? AddAt { get; set; }
        public string UsageType { get; set; }
        public double? UseNumber { get; set; }
        public double? UseMoney { get; set; }
    }
}