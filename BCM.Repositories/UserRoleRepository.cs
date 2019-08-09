using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BCM.Repositories
{
    public class UserRoleRepository : BaseRepository<TbUserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public override void Update(TbUserRole entity)
        {
            EntityEntry<TbUserRole> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.UserRoleId).IsModified = false;
        }

        public void UpdateRange(string userid, string roleidstring)
        {
            string[] roleids = roleidstring.Split('_');
            List<TbUserRole> ModUserRoleList = new List<TbUserRole>();
            foreach (var rid in roleids)
            {
                ModUserRoleList.Add(new TbUserRole() { UserId = userid, RoleId = rid, UserRoleId = Method.GetGuid32() });
            }
            var DBuserRoleList = Context.Set<TbUserRole>().Where(u => u.UserId == userid);
            //查询需要add的部分
            var addUserRole = ModUserRoleList.Where(a => !DBuserRoleList.Where(t => a.RoleId == t.RoleId).Any()).ToList();
            Context.Set<TbUserRole>().AddRange(addUserRole);
            //查询需要del的部分
            var delUserRole = DBuserRoleList.Where(a => !ModUserRoleList.Where(t => a.RoleId == t.RoleId).Any()).ToList();
            foreach (var del in delUserRole)
            {
                Context.Set<TbUserRole>().Remove(del);
            }
            // //查询相同的部分
            // var sameUserRole = DBuserRoleList.Where(a => ModUserRoleList.Exists(t => a.RoleId.Equals(t.RoleId))).ToList();

            // EntityEntry<TbUserRole> dbEntityEntry = Context.Entry(entity);
            // dbEntityEntry.State = EntityState.Modified;
            // dbEntityEntry.Property(x => x.UserRoleId).IsModified = false;
        }
        public override TbUserRole GetSingle(string id)
        {
            return Context.Set<TbUserRole>().FirstOrDefault(x => x.UserRoleId == id);
        }
        public override async Task<TbUserRole> GetSingleAsync(string id)
        {
            return await Context.Set<TbUserRole>().FirstOrDefaultAsync(x => x.UserRoleId == id);
        }
    }
}