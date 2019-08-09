using System;
using System.Collections.Generic;

namespace BCM.Models.BCM.DbModel
{
    public partial class TbUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassWord { get; set; }
        public string UserRealName { get; set; }
        public string UserSex { get; set; }
        public string UserCellphone { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }
        public string UserStatus { get; set; }
        public string UserDepartmentId { get; set; }
        public string UserRemarks { get; set; }
    }
}
