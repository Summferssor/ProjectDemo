using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;

namespace BCM.IRepositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<object> getEmployeeInfo(string id);
        int OrderNum(string companyid);
        Task<int> OrderNumAsync(string companyid);
        Task<PaginatedList<EmployeeInfo>> Page(string companyid, string employeename, string jobid, int page, int pageSize);

        IQueryable<EmployeeInfo> AllEmployees(string companyid, string employeename, string jobid);

        Task<PaginatedList<EmployeeCountAneNum>> PageCountNum(string companyid, string employeename, string jobid, int page, int pageSize);

        IQueryable<EmployeeCountAneNum> AllEmployeesCountNum(string companyid, string employeename, string jobid);

        Task<PaginatedList<PersonPayoffTableView>> PersonPayoffTable(string timestart, string timeend, string companyid, string employeename, int page, int pageSize);

        Task<PaginatedList<TopUpView>> PersonTopUpTable(string timestart, string timeend, string companyid, string employeename, int page, int pageSize);

        // IQueryable<PersonPayoffTableView> AllPersonPayoffTable(string companyid, string employeename);

    }
}