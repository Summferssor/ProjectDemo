using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM.BCMModels.ModelCreation;
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
    public class GiveMoneyController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GiveMoneyController> _logger;
        private readonly IGiveMoneyRepository _giveMoneyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public GiveMoneyController(IMapper mapper, IUnitOfWork unitOfWork,IEmployeeRepository employeeRepository, ILogger<GiveMoneyController> logger, IGiveMoneyRepository giveMoneyRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _giveMoneyRepository = giveMoneyRepository;
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        [HttpPost("qwe")]
        public async Task<Message> addd()
        {
            List<GiveMoney> List = new List<GiveMoney>();
            Random r = new Random();
            for (int i = 0; i < 7000; i++)
            {
                GiveMoney company = new GiveMoney()
                {
                    GiveRecordId = Method.GetGuid32(),
                    AddAt = DateTime.Now.AddDays(r.Next(1,355)).AddHours(r.Next(0,23)).AddMinutes(r.Next(0,60)).AddSeconds(r.Next(0,60)),
                    GiveMoneyType = i % 2 == 1 ? "A500001" : "A500002",
                    
                    GiveMoney1 = r.Next(1,100),
                    Remarks = "测试",
                    JobId = i % 2 == 1 ? "2016121101" : "2016121101"
                };
                List.Add(company);
            }
            for (int i = 0; i < 300; i++)
            {
                GiveMoney company = new GiveMoney()
                {
                    GiveRecordId = Method.GetGuid32(),
                    AddAt = DateTime.Now.AddDays(r.Next(1,355)).AddHours(r.Next(0,23)).AddMinutes(r.Next(0,60)).AddSeconds(r.Next(0,60)),
                    GiveMoneyType = "A500003",
                    
                    GiveMoney1 = r.Next(1,100),
                    Remarks = "测试",
                    JobId = "2016121103"
                };
                List.Add(company);
            }
            _giveMoneyRepository.AddRange(List);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 金额发放
        /// </summary>
        /// <param name="giveMoneyCreation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostMoney([FromBody] GiveMoneyCreation giveMoneyCreation)
        {
            if (giveMoneyCreation == null)
            {
                return Message.Fail();
            }

            giveMoneyCreation.GiveRecordId = Method.GetGuid32();
            
            var dbEmp = await _employeeRepository.GetSingleAsync(x => x.JobId == giveMoneyCreation.JobId);
            if(dbEmp == null)
            {
                return Message.NotFound().Add("content", "工号错误");
            }
            dbEmp.AvailableMoney += giveMoneyCreation.GiveMoney1;
            dbEmp.UpdateMoneyAt = DateTime.Now;
            var newItem = _mapper.Map<GiveMoney>(giveMoneyCreation);
            _giveMoneyRepository.Add(newItem);
            _employeeRepository.Update(dbEmp);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 员工次数转钱包记录查询
        /// </summary>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("givemoneyrecord")]
        public async Task<Message> giveMoneyRecord([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            DateTime yaer1 = Method.StampToDateTime(start);
            DateTime year2 = Method.StampToDateTime(end);
            if (year2.Year != yaer1.Year)
            {
                return Message.Fail().Add("content", "不支持跨年查询，请选择年份相同的时间");
            }
            var items = await _giveMoneyRepository.GiveMoneyRecord(jobid, start, end, employeename, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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

            return Message.Ok().Add("pageData", items).Add("pageSize", pageSize).Add("pageCount", items.Count).Add("pageIndex", pageIndex).Add("totalCount", items.TotalCount).Add("totalPages", items.TotalPages).Add("prePage", prePage).Add("nextPage", nextPage);
        }
    }
}