using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCM.Models.BCM.DbModel;

namespace BCM.IRepositories
{
    public interface IDictionaryRepository : IBaseRepository<TbDictionary>
    {
        int OrderNum(string parentcode);
        Task<int> OrderNumAsync(string parentcode);
        // IQueryable<TbDictionary> GetAll();
        IQueryable<object> GetDictionaryTree();
        IQueryable<object> GetNumberLevel();
        IQueryable<object> GetUsageType();
        IQueryable<object> GetConsumptionType();
        IQueryable<object> GetGiveNumberType();
        IQueryable<object> GetGiveMoneyType();
    }
}