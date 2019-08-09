using System;
using System.Threading.Tasks;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;
using System.Linq;

namespace BCM.IRepositories
{
    public interface IUseRecordRepository : IBaseRepository<UseRecord>
    {
        Task<PaginatedList<EmployeeRecoreDetail>> RecordDetail(string jobid, string timestart, string timeend, string usagetype, string consumptiontype, int page, int pageSize);
        IQueryable<EmployeeRecoreDetail> AllRecordDetail(string jobid, string timestart, string timeend, string usagetype, string consumptiontype);

        Task<PaginatedList<EmployeeConsume>> ConsumeRecord(string jobid, string timestart, string timeend, string empname, string companyid, int page, int pageSize);
        IQueryable<EmployeeConsume> AllConsumeRecord(string jobid, string timestart, string timeend, string empname, string conpanyid);

    }
}