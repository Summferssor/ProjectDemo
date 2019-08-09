using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;

namespace BCM.IRepositories
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<PaginatedList<Company>> Page(string searchString, int page, int pageSize);
        Task<PaginatedList<Company>> DepartmentPage(string searchString, int page, int pageSize, string parentid);
        IQueryable<object> GetCompanyList();

        IQueryable<object> GetDepartmentList(string parentid);
        IQueryable<object> GetCompanyTree();
        Task<PaginatedList<CompanyPayoffTableView>> CompanyPayoffTable(string timestart, string timeend, string companyid, int page, int pageSize);

        Task<PaginatedList<SettlementView>> Settlement(string timestart, string timeend, string companyid, int page, int pageSize);
    }
}