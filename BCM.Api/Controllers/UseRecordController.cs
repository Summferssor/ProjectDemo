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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BCM.Api.Controllers
{
    [EnableCors("CorsPolicy")] // ����
    [Route("BcmSystem/[controller]")]
    [ApiController]
    [Authorize]
    public class UseRecordController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UseRecordController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUseRecordRepository _useRecordRepository;
        public UseRecordController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UseRecordController> logger, IEmployeeRepository employeeRepository, IUseRecordRepository useRecordRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _useRecordRepository = useRecordRepository;
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        [HttpPost("qwe")]
        public async Task<Message> addd()
        {
            List<UseRecord> List = new List<UseRecord>();
            Random r = new Random();
            for (int i = 0; i < 10000; i++)
            {
                UseRecord company = new UseRecord()
                {
                    // AddAt = DateTime.Now,
                    UseRecordId = Method.GetGuid32(),
                    AddAt = DateTime.Now.AddDays(r.Next(1,355)).AddHours(r.Next(0,23)).AddMinutes(r.Next(0,60)).AddSeconds(r.Next(0,60)),
                    GiveCountType = i % 2 == 1 ? "A100001" : "A100002",
                    UseNumber = 1,
                    UseMoney = 5,
                    Remarks = "测试",
                    JobId = i % 2 == 1 ?"2016124038":"2016124039"
                };
                List.Add(company);
            }
            _useRecordRepository.AddRange(List);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 员工记录明细
        /// </summary>
        /// <param name="usageType"></param>
        /// <param name="consumptionType"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("recorddetail")]
        public async Task<Message> GetRecordDetail([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("usageType") || !dictionary.ContainsKey("consumptionType") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string usagetype = (string)dictionary["usageType"];
            string consumptiontype = (string)dictionary["consumptionType"];
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            string jobid = (string)dictionary["jobId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];
            DateTime yaer1 = Method.StampToDateTime(start);
            DateTime year2 = Method.StampToDateTime(end);
            if (year2.Year != yaer1.Year)
            {
                return Message.Fail().Add("content", "不支持跨年查询，请选择年份相同的时间");
            }
            var items = await _useRecordRepository.RecordDetail(jobid, start, end, usagetype, consumptiontype, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            bool prePage = false;
            bool nextPage = false;
            // return Message.Ok().Add("list", items);
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


        /// <summary>
        /// 员工消费记录
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("consumerecord")]
        public async Task<Message> consumeRecord([FromBody] Dictionary<string, object> dictionary)
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
            var items = await _useRecordRepository.ConsumeRecord(jobid, start, end, employeename, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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

        /// <summary>
        /// 扫码消费
        /// </summary>
        /// <param name="useRecordCreation"></param>
        /// <returns></returns>
        [HttpPost("use")]
        public async Task<Message> useCount(UseRecordCreation useRecordCreation)
        {
            if (useRecordCreation == null || string.IsNullOrEmpty(useRecordCreation.JobId))
            {
                return Message.Fail();
            }
            var dbemp = await _employeeRepository.GetSingleAsync(x => x.JobId == useRecordCreation.JobId);
            if (dbemp == null)
            {
                return Message.Fail().Add("content", "工号不存在");
            }
            
            if (useRecordCreation.GiveCountType.Trim() == "A100002")
            {
                if (useRecordCreation.UseMoney < 0)
                {
                    return Message.Fail().Add("content", "金额小于0");
                }
                dbemp.AvailableMoney -= useRecordCreation.UseMoney;
                if (dbemp.AvailableMoney < 0)
                {
                    return Message.Fail().Add("content", "余额不足");
                }
                dbemp.UpdateMoneyAt = DateTime.Now;

                useRecordCreation.UseRecordId = Method.GetGuid32();
                useRecordCreation.AddAt = DateTime.Now;
                useRecordCreation.UseNumber = 0;
            }
            if (useRecordCreation.GiveCountType.Trim() == "A100001")
            {
                if (useRecordCreation.UseNumber < 0)
                {
                    return Message.Fail().Add("content", "次数小于0");
                }
                dbemp.AvailableCount -= useRecordCreation.UseNumber;
                if (dbemp.AvailableCount < 0)
                {
                    return Message.Fail().Add("content", "次数不足");
                }
                dbemp.UpdateCountAt = DateTime.Now;
                useRecordCreation.UseRecordId = Method.GetGuid32();
                useRecordCreation.AddAt = DateTime.Now;
                useRecordCreation.UseMoney = 0;
            }
            var newItem = _mapper.Map<UseRecord>(useRecordCreation);
            _useRecordRepository.Add(newItem);
            _employeeRepository.Update(dbemp);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }
    }
}