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
    public class LogRepository : BaseRepository<TbLog>, ILogRepository
    {
        public LogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public override void Update(TbLog entity)
        {
            EntityEntry<TbLog> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.LogId).IsModified = false;
            if(string.IsNullOrEmpty(entity.LogType))
            {
                dbEntityEntry.Property(x => x.LogType).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.LogOperationUserName))
            {
                dbEntityEntry.Property(x => x.LogOperationUserName).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.LogOperationIpv4))
            {
                dbEntityEntry.Property(x => x.LogOperationIpv4).IsModified = false;
            }
           
            if(string.IsNullOrEmpty(entity.LogOperationIpv6))
            {
            dbEntityEntry.Property(x => x.LogOperationIpv6).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.LogOperationTime.ToString()))
            {
                dbEntityEntry.Property(x => x.LogOperationTime).IsModified = false;
            }

            if(string.IsNullOrEmpty(entity.LogOperationName))
            {
                dbEntityEntry.Property(x => x.LogOperationName).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.LogOperationUrl))
            {
                dbEntityEntry.Property(x => x.LogOperationUrl).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.LogRemarks))
            {
                dbEntityEntry.Property(x => x.LogRemarks).IsModified = false;
            }
        }
        public override TbLog GetSingle(string id)
        {
            return Context.Set<TbLog>().FirstOrDefault(x => x.LogId == id);
        }
        public override async Task<TbLog> GetSingleAsync(string id)
        {
            return await Context.Set<TbLog>().FirstOrDefaultAsync(x => x.LogId == id);
        }
    }
}