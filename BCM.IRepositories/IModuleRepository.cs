using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;

namespace BCM.IRepositories
{
    public interface IModuleRepository : IBaseRepository<TbModule>
    {
        IQueryable<object> GetModuleTree(string username);

        IQueryable<object> GetModuleAllTree();
        int OrderNum(string parentid);
        Task<int> OrderNumAsync(string parentid);
    }
}