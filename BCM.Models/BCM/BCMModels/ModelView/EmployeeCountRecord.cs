using System;

namespace BCM.Models.BCM.BCMModels.ModelView
{
    public class EmployeeCountRecord
    {
        public string name { get; set; }
        public string jobId { get; set; }
        public string conpanyId { get; set; }
        public string companyName { get; set; }
        public string departmentId { get; set; }
        public string departmentName { get; set; }
        public DateTime? giveTime { get; set; }
        public int? giveNumber { get; set; }
        public string giveType { get; set; }
    }
}