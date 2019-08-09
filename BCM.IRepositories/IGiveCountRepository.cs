using System.Linq;
using System.Threading.Tasks;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;

namespace BCM.IRepositories
{
    public interface IGiveCountRepository : IBaseRepository<GiveCount>
    {
        Task<PaginatedList<EmployeeCountRecord>> GiveCountRecord(string jobid, string timestart, string timeend, string empname, string companyid, int page, int pageSize);
        IQueryable<EmployeeCountRecord> AllGiveCountRecord(string jobid, string timestart, string timeend, string empname, string conpanyid);
    }
}