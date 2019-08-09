using System.Collections.Generic;
using System.Threading.Tasks;
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

namespace BCM.Api.Controllers
{
    [EnableCors("CorsPolicy")] // ����
    [Route("BcmSystem/[controller]")]
    [ApiController]
    [Authorize]
    public class ModuleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModuleController> _logger;
        private readonly IModuleRepository _moduleRepository;
        public ModuleController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ModuleController> logger, IModuleRepository moduleRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _moduleRepository = moduleRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取模块All
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Message> GetAllModule()
        {
            var items = await _moduleRepository.All.ToListAsync();
            var results = _mapper.Map<IEnumerable<ModuleView>>(items);
            return Message.Ok().Add("moduleList", results);
        }
        
        /// <summary>
        /// 搜索框（moduleName）
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<Message> GetModuleBySearchString([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("moduleName"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string modulename = (string)dictionary["moduleName"];
            var items = await _moduleRepository.FindBy(x => x.ModuleName.Contains(modulename)).ToListAsync();
            // var results = _mapper.Map<IEnumerable<UserView>>(items);
            return Message.Ok().Add("moduleList", items);
        }

        /// <summary>
        /// 获取模块树all
        /// </summary>
        /// <returns></returns>
        [HttpGet("alltree")]
        public async Task<Message> GetAllModuleTree()
        {
            var items = await _moduleRepository.GetModuleAllTree().ToListAsync();
            return Message.Ok().Add("moduleList", items);
        }
        
        /// <summary>
        /// 获取用户模块树
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        [HttpGet("{username}")]
        public async Task<Message> GetModuleTree(string username)
        {
            var items = await _moduleRepository.GetModuleTree(username).ToListAsync();
            return Message.Ok().Add("moduleTree", items);
        }

        /// <summary>
        /// 新增模块
        /// </summary>
        /// <param name="moduleCreation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostModule([FromBody] ModuleCreation moduleCreation)
        {
            if (moduleCreation == null)
            {
                return Message.Fail();
            }


            var dbitem = await _moduleRepository.GetSingleAsync(x => x.ModuleName == moduleCreation.ModuleName);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "模块名已存在");
            }

            moduleCreation.ModuleId = Method.GetGuid32();
            moduleCreation.ModuleStatus = "1";
            // moduleCreation.ModuleOrderNum = await _moduleRepository.OrderNumAsync(moduleCreation.ParentModuleId);

            var newItem = _mapper.Map<TbModule>(moduleCreation);
            _moduleRepository.Add(newItem);

            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 更新模块
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="moduleModification"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<Message> PutModuel(string id, [FromBody] ModuleModification moduleModification)
        {
            if (moduleModification == null)
            {
                return Message.Fail();
            }


            var dbitem = await _moduleRepository.GetSingleAsync(x => x.ModuleId == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            moduleModification.ModuleId = id;
            _mapper.Map(moduleModification, dbitem);
            _moduleRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<Message> DeleteModule(string id)
        {
            var module = await _moduleRepository.GetSingleAsync(x => x.ModuleId == id);
            if (module == null)
            {
                return Message.NotFound();
            }
            _moduleRepository.Delete(module);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }
    }
}