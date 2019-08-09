using System;
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

namespace BCM.Api.Controllers
{
    [EnableCors("CorsPolicy")] // ����
    [Route("BcmSystem/[controller]")]
    [ApiController]
    [Authorize]
    public class DictionaryController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DictionaryController> _logger;
        private readonly IDictionaryRepository _dictionaryRepository;
        public DictionaryController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DictionaryController> logger, IDictionaryRepository dictionaryRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dictionaryRepository = dictionaryRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<Message> GetAllDictionary()
        {
            var items = await _dictionaryRepository.All.ToListAsync();
            var results = _mapper.Map<IEnumerable<DictionaryView>>(items);
            return Message.Ok().Add("dictionaryList", results);
        }

        /// <summary>
        /// 搜索框
        /// </summary>
        /// <param name="dictionaryCode">字典码</param>
        /// <param name="dictionaryName">字典名</param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<Message> GetDictionaryBySearchString([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("dictionaryName") || !dictionary.ContainsKey("dictionaryCode")  || !dictionary.ContainsKey("parentCode") )
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string dicname = (string)dictionary["dictionaryName"];
            string diccode = (string)dictionary["dictionaryCode"];
            string parentcode = (string)dictionary["parentCode"];
            var items = await _dictionaryRepository.FindBy(x => x.DictionaryCode.Contains(diccode) && x.DictionaryName.Contains(dicname) && x.ParentDictionaryCode.Contains(parentcode)).ToListAsync();
            // var results = _mapper.Map<IEnumerable<UserView>>(items);
            return Message.Ok().Add("dictionaryList", items);
        }

        /// <summary>
        /// 获取字典树
        /// </summary>
        /// <returns></returns>
        [HttpGet("tree")]
        public async Task<Message> GetDictionarytree()
        {
            var items = await _dictionaryRepository.GetDictionaryTree().ToListAsync();
            return Message.Ok().Add("dictionaryTree", items);
        }

        /// <summary>
        /// 获得发放次数数级别字典码
        /// </summary>
        /// <returns></returns>
        [HttpGet("numbersLevel")]
        public async Task<Message> GetNumbersLevel()
        {
            var items = await _dictionaryRepository.GetNumberLevel().ToListAsync();
            return Message.Ok().Add("dictionaryTree", items);
        }

        /// <summary>
        /// 使用类型树
        /// </summary>
        /// <returns></returns>
        [HttpGet("usagetype")]
        public async Task<Message> GetUsagetype()
        {
            var items = await _dictionaryRepository.GetUsageType().ToListAsync();
            return Message.Ok().Add("dictionaryTree", items);
        }

        /// <summary>
        /// 消费类型树
        /// </summary>
        /// <returns></returns>
        [HttpGet("consumptiontype")]
        public async Task<Message> GetConsumptiontype()
        {
            var items = await _dictionaryRepository.GetConsumptionType().ToListAsync();
            return Message.Ok().Add("dictionaryTree", items);
        }

        /// <summary>
        /// 钱包发放类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("moneytype")]
        public async Task<Message> Getmoneytype()
        {
            var items = await _dictionaryRepository.GetGiveMoneyType().ToListAsync();
            return Message.Ok().Add("dictionaryTree", items);
        }

        /// <summary>
        /// 次数发放类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("numbertype")]
        public async Task<Message> Getnumbertype()
        {
            var items = await _dictionaryRepository.GetGiveNumberType().ToListAsync();
            return Message.Ok().Add("dictionaryTree", items);
        }

        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="dictionaryCreation">字典实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostDic([FromBody] DictionaryCreation dictionaryCreation)
        {
            if (dictionaryCreation == null)
            {
                return Message.Fail();
            }
            var dbitem = await _dictionaryRepository.GetSingleAsync(x => x.DictionaryName == dictionaryCreation.DictionaryName || x.DictionaryCode == dictionaryCreation.DictionaryCode);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "字典码或字典名重复");
            }
            dictionaryCreation.DictionaryId = Method.GetGuid32();
            // dictionaryCreation.DictionaryOrderNum = await _dictionaryRepository.OrderNumAsync(dictionaryCreation.ParentDictionaryCode);

            var newItem = _mapper.Map<TbDictionary>(dictionaryCreation);
            _dictionaryRepository.Add(newItem);

            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 修改字典
        /// </summary>
        /// <param name="id">字典Id</param>
        /// <param name="dictionaryModification">字典实体</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<Message> PutDic(string id, [FromBody] DictionaryModification dictionaryModification)
        {
            if (dictionaryModification == null)
            {
                return Message.Fail();
            }


            var dbitem = await _dictionaryRepository.GetSingleAsync(x => x.DictionaryId == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            dictionaryModification.DictionaryId = id;
            _mapper.Map(dictionaryModification, dbitem);
            _dictionaryRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }


        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="id">字典Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<Message> DeleteDic(string id)
        {
            var module = await _dictionaryRepository.GetSingleAsync(x => x.DictionaryId == id);
            if (module == null)
            {
                return Message.NotFound();
            }
            _dictionaryRepository.Delete(module);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }
    }


}