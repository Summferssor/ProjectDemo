using AutoMapper;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelCreation;
using BCM.Models.BCM.BCMModels.ModelModification;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleModuleRepository _roleModuleRepository;

        public RoleController(IRoleRepository roleRepository, IMapper mapper, ILogger<RoleController> logger, IUnitOfWork unitOfWork, IRoleModuleRepository roleModuleRepository)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
            _roleModuleRepository = roleModuleRepository;
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Message> GetAllRoles()
        {
            var items = await _roleRepository.All.ToListAsync();
            var results = _mapper.Map<IEnumerable<RoleView>>(items);
            return Message.Ok().Add("roleList", results);
        }


        /// <summary>
        /// 根据角色名查询角色信息
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<Message> GetUsersBySearchString([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("roleName"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string rolename = (string)dictionary["roleName"];
            var items = await _roleRepository.FindBy(x=>x.RoleName.Contains(rolename)).ToListAsync();
            var results = _mapper.Map<IEnumerable<RoleView>>(items);
            return Message.Ok().Add("roleList", results);
        }

        /// <summary>
        /// 获取角色byRoleId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<Message> GetRole(string id)
        {
            var item = await _roleRepository.RoleModuleById(id).ToListAsync();
            if (item == null)
            {
                return Message.Fail();
            }
            // var result = _mapper.Map<RoleView>(item);
            return Message.Ok().Add("roleModuleTree", item);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="roleModuleIdString">角色拥有模块id字符串“_”隔开</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostRole([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("role") || !dictionary.ContainsKey("roleModuleIdString"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            JObject jrole = (JObject)dictionary["role"];
            RoleCreation roleCreation = jrole.ToObject<RoleCreation>();
            string roleModuleIdString = (string)dictionary["roleModuleIdString"];

            var dbitem = await _roleRepository.GetSingleAsync(x => x.RoleName == roleCreation.RoleName);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "角色名已存在");
            }

            string roleid = roleCreation.RoleId = Method.GetGuid32();
            if (string.IsNullOrEmpty(roleCreation.ParentRoleId))
            {
                roleCreation.ParentRoleId = "0";
            }
            if (string.IsNullOrEmpty(roleCreation.RoleLevel.ToString()))
            {
                roleCreation.RoleLevel = 0;
            }
            roleCreation.RoleStatus = "1";
            roleCreation.RoleOrderNum = await _roleRepository.CountAsync() + 1;
            string[] moduleIds = roleModuleIdString.Split('_');
            List<TbRoleModule> roleModules = new List<TbRoleModule>();
            foreach (string moduleid in moduleIds)
            {
                roleModules.Add(new TbRoleModule() { RoleModuleId = Method.GetGuid32(), RoleId = roleid, ModuleId = moduleid });
            }
            var newItem = _mapper.Map<TbRole>(roleCreation);
            _roleRepository.Add(newItem);
            _roleModuleRepository.AddRange(roleModules);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="role">角色实体</param>
        /// <param name="roleModuleIdString">角色拥有模块id字符串“_”隔开</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<Message> PutRole(string id, [FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("role") || !dictionary.ContainsKey("roleModuleIdString"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            JObject jrole = (JObject)dictionary["role"];
            RoleModification roleModification = jrole.ToObject<RoleModification>();
            string roleModuleIdString = (string)dictionary["roleModuleIdString"];

            var dbItem = await _roleRepository.GetSingleAsync(x => x.RoleId == id);
            if (dbItem == null)
            {
                return Message.NotFound();
            }
            roleModification.RoleId = id;
            _mapper.Map(roleModification, dbItem);
            _roleRepository.Update(dbItem);
            _roleModuleRepository.UpdateRange(id, roleModuleIdString);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<Message> Delete(string id)
        {
            var rolemodel = await _roleRepository.GetSingleAsync(x => x.RoleId == id);
            var roleModuleModel = _roleModuleRepository.FindBy(x => x.RoleId == id);
            if (rolemodel == null)
            {
                return Message.NotFound();
            }
            _roleRepository.Delete(rolemodel);
            _roleModuleRepository.DeleteRange(roleModuleModel);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }
    }
}