using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCM.Repositories
{
    public class RoleRepository : BaseRepository<TbRole>, IRoleRepository
    {
        public RoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public override void Update(TbRole entity)
        {
            EntityEntry<TbRole> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.RoleId).IsModified = false;
            if (string.IsNullOrEmpty(entity.RoleDescription))
            {
                dbEntityEntry.Property(x => x.RoleDescription).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.RoleName))
            {
                dbEntityEntry.Property(x => x.RoleName).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ParentRoleId))
            {
                dbEntityEntry.Property(x => x.ParentRoleId).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.RoleStatus))
            {
                dbEntityEntry.Property(x => x.RoleStatus).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.RoleOrderNum.ToString()))
            {
                dbEntityEntry.Property(x => x.RoleOrderNum).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.RoleLevel.ToString()))
            {
                dbEntityEntry.Property(x => x.RoleLevel).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.RoleRemarks))
            {
                dbEntityEntry.Property(x => x.RoleRemarks).IsModified = false;
            }
        }
        public override TbRole GetSingle(string id)
        {
            return Context.Set<TbRole>().FirstOrDefault(x => x.RoleId == id);
        }
        public override async Task<TbRole> GetSingleAsync(string id)
        {
            return await Context.Set<TbRole>().FirstOrDefaultAsync(x => x.RoleId == id);
        }

        public override void Add(TbRole entity)
        {
            if (string.IsNullOrEmpty(entity.RoleOrderNum.ToString()))
            {

                entity.RoleOrderNum = Context.Set<TbRole>().Where(x => x.ParentRoleId == entity.ParentRoleId).Count();

            }
            Context.Set<TbRole>().Add(entity);
        }

        public IQueryable<object> RoleModuleById(string roleid)
        {
            var roleModule = from rm in Context.Set<TbRoleModule>()
                             join m in Context.Set<TbModule>()
                             on rm.ModuleId equals m.ModuleId
                             where rm.RoleId.Trim() == roleid.Trim()
                             select m;
            
            var queryModuleTree = from module1 in roleModule
                                  where module1.ParentModuleId.Trim() == "0"
                                  orderby module1.ModuleOrderNum ascending
                                  select new
                                  {
                                      moduleId = module1.ModuleId.Trim(),
                                      name = module1.ModuleName.Trim(),
                                      path = module1.ModuleUrl.Trim(),
                                      icon = module1.ModuleIcon.Trim(),
                                      children = from module2 in roleModule
                                                 where module2.ParentModuleId.Trim() == module1.ModuleId.Trim()
                                                 orderby module2.ModuleOrderNum ascending
                                                 select new
                                                 {
                                                     moduleId = module2.ModuleId.Trim(),
                                                     name = module2.ModuleName.Trim(),
                                                     path = module2.ModuleUrl.Trim(),
                                                     icon = module2.ModuleIcon.Trim()
                                                 }
                                  };
            return queryModuleTree;
        }
    }
}
