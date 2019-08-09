using AutoMapper;
using BCM.Common;
using BCM.Common.TokenHelper;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelCreation;
using BCM.Models.BCM.BCMModels.ModelModification;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BCM.Api.Controllers
{
    [EnableCors("CorsPolicy")] // 跨域
    [Route("BcmSystem/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="hostingEnvironment"></param>
        public UserController(IUserRepository userRepository, IUserRoleRepository userRoleRepository, IMapper mapper, ILogger<UserController> logger, IUnitOfWork unitOfWork, IHostingEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Message> GetAllUsers()
        {
            var items = await _userRepository.AllUserInfo().ToListAsync();
            // var results = _mapper.Map<IEnumerable<UserView>>(items);
            return Message.Ok().Add("userList", items);
        }
        
        /// <summary>
        /// 根据用户名和用户真实姓名查询用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userRealName"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<Message> GetUsersBySearchString([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("userName") || !dictionary.ContainsKey("userRealName"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string username = (string)dictionary["userName"];
            string realname = (string)dictionary["userRealName"];
            var items = await _userRepository.SearchUserInfo(username, realname).ToListAsync();
            // var results = _mapper.Map<IEnumerable<UserView>>(items);
            return Message.Ok().Add("userList", items);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPut("mod/{id}")]
        public async Task<Message> PutEmp(string id, [FromBody] Dictionary<string, object> dictionary)
        {

            if (!dictionary.ContainsKey("password"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string password = (string)dictionary["password"];

            var dbitem = await _userRepository.GetSingleAsync(x => x.UserId == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            UserModification userModification = new UserModification();
            if(!string.IsNullOrEmpty(password) && password.Length < 6)
            {
                return Message.Fail().Add("content", "密码长度小于6");
            }
            userModification.UserId = id;
            userModification.UserPassWord = password;
            _mapper.Map(userModification, dbitem);
            _userRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<Message> LoginUser([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("userName") || !dictionary.ContainsKey("passWord") || !dictionary.ContainsKey("code"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string code = (string)dictionary["code"];
            string username = (string)dictionary["userName"];
            string password = (string)dictionary["passWord"];

            var dbitem = await _userRepository.GetSingleAsync(x => x.UserName == username && x.UserPassWord == password);
            if (dbitem == null)
            {
                return Message.NotFound().Add("content","用户不存在或密码错误");
            }
            var ckcode = Request.Cookies["verify_code"];
            if(Request.Cookies["verify_code"] == null)
            {
                ckcode = "未获取验证码";
            }
                            
            if (!code.Trim().Equals(ckcode.Trim()))
            {
                return Message.VerifyFail().Add("content","验证码错误");
            }
            Response.Cookies.Delete("verify_code");
            return Message.Ok().Add("user", new { userId = dbitem.UserId, userName = dbitem.UserName }).Add("token", TokenContext.GetToken(dbitem.UserId, dbitem.UserName));
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="userRoleIdString">用户角色id字符串 “_”隔开</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostUser([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("user") || !dictionary.ContainsKey("userRoleIdString"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            JObject juser = (JObject)dictionary["user"];
            UserCreation userCreation = juser.ToObject<UserCreation>();
            string userRoleIdString = (string)dictionary["userRoleIdString"];

            var dbitem = await _userRepository.GetSingleAsync(x => x.UserName == userCreation.UserName);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "用户名已存在");
            }

            string userid = userCreation.UserId = Method.GetGuid32();
            userCreation.UserStatus = "1";
            List<TbUserRole> userRoleList = new List<TbUserRole>();
            string[] roleIds = userRoleIdString.Split('_');
            foreach (var roleid in roleIds)
            {
                userRoleList.Add(new TbUserRole() { UserId = userid, RoleId = roleid, UserRoleId = Method.GetGuid32() });
            }

            var newItem = _mapper.Map<TbUser>(userCreation);
            _userRepository.Add(newItem);
            _userRoleRepository.AddRange(userRoleList);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 用户信息修改
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="user">用户实体</param>
        /// <param name="userRoleIdString">用户Id字符串 “_”隔开</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<Message> PutUser(string id, [FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("user") || !dictionary.ContainsKey("userRoleIdString"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            JObject juser = (JObject)dictionary["user"];
            UserModification userModification = juser.ToObject<UserModification>();
            string userRoleIdString = (string)dictionary["userRoleIdString"];

            var dbItem = await _userRepository.GetSingleAsync(x => x.UserId == id);
            if (dbItem == null)
            {
                return Message.NotFound();
            }
            userModification.UserId = id;
            _mapper.Map(userModification, dbItem);
            _userRepository.Update(dbItem);
            _userRoleRepository.UpdateRange(id, userRoleIdString);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<Message> DeleteUser(string id)
        {
            var model = await _userRepository.GetSingleAsync(x => x.UserId == id);
            var userRoleModel = _userRoleRepository.FindBy(x => x.UserId == id);
            if (model == null)
            {
                return Message.NotFound();
            }
            _userRepository.Delete(model);
            _userRoleRepository.DeleteRange(userRoleModel);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }
    }
}