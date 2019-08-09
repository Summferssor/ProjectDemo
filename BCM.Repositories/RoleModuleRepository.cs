using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCM.Repositories
{
    public class RoleModuleRepository : BaseRepository<TbRoleModule>, IRoleModuleRepository
    {
        public RoleModuleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public void UpdateRange(string roleid, string moduleidstring)
        {
            string[] moduleids = moduleidstring.Split('_');
            List<TbRoleModule> ModRolemoduleList = new List<TbRoleModule>();
            foreach (string moduleid in moduleids)
            {
                ModRolemoduleList.Add(new TbRoleModule() { ModuleId = moduleid, RoleId = roleid, RoleModuleId = Method.GetGuid32() });
            }
            var DbRoleModuleList = Context.Set<TbRoleModule>().Where(x => x.RoleId == roleid);
            //add
            var addRoleModule = ModRolemoduleList.Where(a => !DbRoleModuleList.Where(t => t.ModuleId == a.ModuleId).Any()).ToList();
            Context.Set<TbRoleModule>().AddRange(addRoleModule);
            //del\
            var delRoleModule = DbRoleModuleList.Where(a => !ModRolemoduleList.Where(t => t.ModuleId == a.ModuleId).Any()).ToList();
            foreach (var del in delRoleModule)
            {
                Context.Set<TbRoleModule>().Remove(del);
            }
        }
        public override void Update(TbRoleModule entity)
        {
            EntityEntry<TbRoleModule> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.RoleModuleId).IsModified = false;
        }
        public override TbRoleModule GetSingle(string id)
        {
            return Context.Set<TbRoleModule>().FirstOrDefault(x => x.RoleModuleId == id);
        }
        public override async Task<TbRoleModule> GetSingleAsync(string id)
        {
            return await Context.Set<TbRoleModule>().FirstOrDefaultAsync(x => x.RoleModuleId == id);
        }
    }
}