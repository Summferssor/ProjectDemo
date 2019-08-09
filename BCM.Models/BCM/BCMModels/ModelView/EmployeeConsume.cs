using System;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class EmployeeConsume
    {
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentId { get; set; }
        public string JobId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? AddAt { get; set; }
        public string ConsumeType { get; set; }
        public string CountOrMoney { get; set; }
    }
}