using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;
using System.Collections.Generic;

namespace BCM.IRepositories
{
    public interface IUserRoleRepository : IBaseRepository<TbUserRole>
    {
        void UpdateRange(string userid, string roleidstring);
    }
}