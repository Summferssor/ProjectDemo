using System.Collections.Generic;
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
    public class ModuleRepository : BaseRepository<TbModule>, IModuleRepository
    {
        public ModuleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override IQueryable<TbModule> All => Context.Set<TbModule>().OrderBy(x => x.ModuleOrderNum);
        public override void Update(TbModule entity)
        {
            EntityEntry<TbModule> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.ModuleId).IsModified = false;
            if (string.IsNullOrEmpty(entity.ModuleName))
            {
                dbEntityEntry.Property(x => x.ModuleName).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ModuleUrl))
            {
                dbEntityEntry.Property(x => x.ModuleUrl).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ParentModuleId))
            {
                dbEntityEntry.Property(x => x.ParentModuleId).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ModuleStatus))
            {
                dbEntityEntry.Property(x => x.ModuleStatus).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ModuleOrderNum.ToString()))
            {
                dbEntityEntry.Property(x => x.ModuleOrderNum).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ModuleDescription))
            {
                dbEntityEntry.Property(x => x.ModuleDescription).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ModuleRemarks))
            {
                dbEntityEntry.Property(x => x.ModuleRemarks).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ModuleIcon))
            {
                dbEntityEntry.Property(x => x.ModuleIcon).IsModified = false;
            }
        }
        public override TbModule GetSingle(string id)
        {
            return Context.Set<TbModule>().FirstOrDefault(x => x.ModuleId == id);
        }
        public override async Task<TbModule> GetSingleAsync(string id)
        {
            return await Context.Set<TbModule>().FirstOrDefaultAsync(x => x.ModuleId == id);
        }

        public IQueryable<object> GetModuleTree(string username)
        {
            var usermodule = from user in Context.Set<TbUser>()
                             where user.UserName.Trim() == username
                             join userRole in Context.Set<TbUserRole>() on user.UserId.Trim() equals userRole.UserId.Trim()
                             join roleModule in Context.Set<TbRoleModule>() on userRole.RoleId.Trim() equals roleModule.RoleId.Trim()
                             join module in Context.Set<TbModule>() on roleModule.ModuleId.Trim() equals module.ModuleId.Trim()
                             select module;
            var queryModuleTree = from module1 in usermodule
                                  where module1.ParentModuleId.Trim() == "0"
                                  orderby module1.ModuleOrderNum ascending
                                  select new
                                  {
                                      moduleId = module1.ModuleId.Trim(),
                                      name = module1.ModuleName.Trim(),
                                      path = module1.ModuleUrl.Trim(),
                                      icon = module1.ModuleIcon.Trim(),
                                      children = from module2 in usermodule
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

        public IQueryable<object> GetModuleAllTree()
        {
            var queryModuleTree = from module1 in Context.Set<TbModule>()
                                  where module1.ParentModuleId.Trim() == "0"
                                  orderby module1.ModuleOrderNum ascending
                                  select new
                                  {
                                      moduleId = module1.ModuleId.Trim(),
                                      name = module1.ModuleName.Trim(),
                                      path = module1.ModuleUrl.Trim(),
                                      icon = module1.ModuleIcon.Trim(),
                                      children = from module2 in Context.Set<TbModule>()
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
        public int OrderNum(string parentid)
        {
            return Context.Set<TbModule>().Where(x => x.ParentModuleId == parentid).Count() + 1;
        }

        public async Task<int> OrderNumAsync(string parentid)
        {
            return await Context.Set<TbModule>().Where(x => x.ParentModuleId == parentid).CountAsync() + 1;
        }
        public override void Add(TbModule entity)
        {
            if (string.IsNullOrEmpty(entity.ModuleOrderNum.ToString()))
            {

                entity.ModuleOrderNum = Context.Set<TbModule>().Where(x => x.ParentModuleId == entity.ParentModuleId).Count();

            }
            Context.Set<TbModule>().Add(entity);
        }
    }
}