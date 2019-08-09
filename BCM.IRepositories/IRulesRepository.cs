using System.Threading.Tasks;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;

namespace BCM.IRepositories
{
    public interface IRulesRepository : IBaseRepository<Rules>
    {
        Task<PaginatedList<Rules>> Page(string searchString,int page, int pageSize);
    }
}