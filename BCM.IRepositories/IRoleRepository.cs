using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCM.IRepositories
{
    public interface IRoleRepository : IBaseRepository<TbRole>
    {
        IQueryable<object> RoleModuleById(string roleid);
    }
}
