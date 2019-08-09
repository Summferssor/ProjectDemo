using System.Linq;
using System.Threading.Tasks;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BCM.Repositories
{
    public class RulesRepository : BaseRepository<Rules>, IRulesRepository
    {
        public RulesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }
        public override void Update(Rules entity)
        {
            EntityEntry<Rules> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.Id).IsModified = false;
            if(string.IsNullOrEmpty(entity.Name))
            {
                dbEntityEntry.Property(x => x.Name).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.StartTime))
            {
                dbEntityEntry.Property(x => x.StartTime).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.EndTime))
            {
                dbEntityEntry.Property(x => x.EndTime).IsModified = false;
            }
           
            if(string.IsNullOrEmpty(entity.Price.ToString()))
            {
            dbEntityEntry.Property(x => x.Price).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.Remarks))
            {
                dbEntityEntry.Property(x => x.Remarks).IsModified = false;
            }

            if(string.IsNullOrEmpty(entity.Sort.ToString()))
            {
                dbEntityEntry.Property(x => x.Sort).IsModified = false;
            }
        }
        public override Rules GetSingle(string id)
        {
            return Context.Set<Rules>().FirstOrDefault(x => x.Id == id);
        }
        public override async Task<Rules> GetSingleAsync(string id)
        {
            return await Context.Set<Rules>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaginatedList<Rules>> Page(string searchString, int page, int pageSize)
        {
            var rules = Context.Set<Rules>().Where(x => x.Name.Contains(searchString)).OrderBy(y => y.Sort);
            var rule = await PaginatedList<Rules>.CreatepagingAsync(rules.AsNoTracking(), page, pageSize);
            return rule;
        }
    }
}