using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BCM.Models.BCM.BCMModels.ModelModification
{
    public class UserModification
    {
        [MaxLength(32)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(32)]
        public string UserPassWord { get; set; }
        [MaxLength(20)]
        public string UserRealName { get; set; }
        [MaxLength(50)]
        public string UserSex { get; set; }
        [MaxLength(15)]
        public string UserCellphone { get; set; }
        [MaxLength(100)]
        public string UserAddress { get; set; }
        [MaxLength(100)]
        public string UserEmail { get; set; }
        [MaxLength(50)]
        public string UserStatus { get; set; }
        [MaxLength(50)]
        public string UserDepartmentId { get; set; }
        [MaxLength(200)]
        public string UserRemarks { get; set; }
    }
}
