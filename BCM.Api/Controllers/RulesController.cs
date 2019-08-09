using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM.BCMModels.ModelCreation;
using BCM.Models.BCM.BCMModels.ModelModification;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BCM.Api.Controllers
{
    [EnableCors("CorsPolicy")] // ����
    [Route("BcmSystem/[controller]")]
    [ApiController]
    [Authorize]
    public class RulesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RulesController> _logger;
        private readonly IRulesRepository _rulesRepository;
        public RulesController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<RulesController> logger, IRulesRepository rulesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _rulesRepository = rulesRepository;
            _logger = logger;
        }
        
        /// <summary>
        /// 查询规则
        /// </summary>
        /// <param name="dictionary">参数集合</param>
        /// <returns></returns>
        [HttpPost("page")]
        public async Task<Message> IndexPage([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("searchString") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string searchString = (string)dictionary["searchString"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];

            var items = await _rulesRepository.Page(searchString, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            bool prePage = false;
            bool nextPage = false;
            if (items.HasPreViousPage)
            {
                prePage = true;
            }

            if (items.HasNextPage)
            {
                nextPage = true;
            }

            return Message.Ok().Add("pageData", items).Add("pageSize", pageSize).Add("pageCount", items.Count).Add("pageIndex", pageIndex).Add("totalCount", items.TotalCount).Add("totalPages", items.TotalPages).Add("prePage",prePage).Add("nextPage",nextPage);
        }

        /// <summary>
        /// 新增规则
        /// </summary>
        /// <param name="rulesCreation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostRules([FromBody] RulesCreation rulesCreation)
        {
            if (rulesCreation == null)
            {
                return Message.Fail();
            }

            var dbitem = await _rulesRepository.GetSingleAsync(x => x.Name == rulesCreation.Name);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "规则名重复");
            }
            rulesCreation.Id = Method.GetGuid32();
            rulesCreation.Sort = await _rulesRepository.CountAsync();
            var newItem = _mapper.Map<Rules>(rulesCreation);
            _rulesRepository.Add(newItem);

            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 修改规则
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rulesModification"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<Message> PutRules(string id, [FromBody] RulesModification rulesModification)
        {
            if (rulesModification == null)
            {
                return Message.Fail();
            }


            var dbitem = await _rulesRepository.GetSingleAsync(x => x.Id == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            rulesModification.Id = id;
            _mapper.Map(rulesModification, dbitem);
            _rulesRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<Message> DeleteRules(string id)
        {
            var module = await _rulesRepository.GetSingleAsync(x => x.Id == id);
            if (module == null)
            {
                return Message.NotFound();
            }
            _rulesRepository.Delete(module);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }
    }
}