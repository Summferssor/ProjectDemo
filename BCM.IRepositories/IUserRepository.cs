using System.Linq;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;

namespace BCM.IRepositories
{
    public interface IUserRepository : IBaseRepository<TbUser>
    {
        bool Login(string name, string password);
        IQueryable<object> AllUserInfo();
        IQueryable<object> SearchUserInfo(string username, string userrealname);
    }
}