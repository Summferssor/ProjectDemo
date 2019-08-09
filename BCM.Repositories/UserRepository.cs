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
    public class UserRepository : BaseRepository<TbUser>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public override void Update(TbUser entity)
        {
            EntityEntry<TbUser> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.UserId).IsModified = false;
            if(string.IsNullOrEmpty(entity.UserName))
            {
                dbEntityEntry.Property(x => x.UserName).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserPassWord))
            {
                dbEntityEntry.Property(x => x.UserPassWord).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserRealName))
            {
                dbEntityEntry.Property(x => x.UserRealName).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserSex))
            {
                dbEntityEntry.Property(x => x.UserSex).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserCellphone))
            {
                dbEntityEntry.Property(x => x.UserCellphone).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserAddress))
            {
                dbEntityEntry.Property(x => x.UserAddress).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserEmail))
            {
                dbEntityEntry.Property(x => x.UserEmail).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserStatus))
            {
                dbEntityEntry.Property(x => x.UserStatus).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserDepartmentId))
            {
                dbEntityEntry.Property(x => x.UserDepartmentId).IsModified = false;
            }
            if(string.IsNullOrEmpty(entity.UserRemarks))
            {
                dbEntityEntry.Property(x => x.UserRemarks).IsModified = false;
            }
        }
        public override TbUser GetSingle(string id)
        {
            return Context.Set<TbUser>().FirstOrDefault(x => x.UserId == id);
        }
        public override async Task<TbUser> GetSingleAsync(string id)
        {
            return await Context.Set<TbUser>().FirstOrDefaultAsync(x => x.UserId == id);
        }

        public bool Login(string name, string password)
        {
            var query = Context.Set<TbUser>().Where(x => x.UserName == name && x.UserPassWord == password);
            if (query.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public IQueryable<object> AllUserInfo()
        {
            var items = from user in Context.Set<TbUser>()
                        select new 
                        {
                            userId = user.UserId,
                            userName = user.UserName,
                            userPassWord = user.UserPassWord,
                            userRealName = user.UserRealName,
                            userRemarks = user.UserRemarks,
                            userSex = user.UserSex,
                            userCellphone = user.UserCellphone,
                            userAddress = user.UserAddress,
                            userEmail = user.UserEmail,
                            userStatus = user.UserStatus,
                            UserDepartmentId = user.UserDepartmentId,
                            roleIds = Context.Set<TbUserRole>().Where(x=>x.UserId.Equals(user.UserId)).Select(x=>x.RoleId) 
                        };
            return items;
        }

        public IQueryable<object> SearchUserInfo(string username, string userrealname)
        {
            var items = from user in Context.Set<TbUser>()
                        where string.IsNullOrEmpty(username) || user.UserName.Contains(username)
                        where string.IsNullOrEmpty(userrealname) || user.UserRealName.Contains(userrealname)
                        select new 
                        {
                            userId = user.UserId,
                            userName = user.UserName,
                            userPassWord = user.UserPassWord,
                            userRealName = user.UserRealName,
                            userRemarks = user.UserRemarks,
                            userSex = user.UserSex,
                            userCellphone = user.UserCellphone,
                            userAddress = user.UserAddress,
                            userEmail = user.UserEmail,
                            userStatus = user.UserStatus,
                            UserDepartmentId = user.UserDepartmentId,
                            roleIds = Context.Set<TbUserRole>().Where(x=>x.UserId.Equals(user.UserId)).Select(x=>x.RoleId) 
                        };
            return items;
        }
    }
}