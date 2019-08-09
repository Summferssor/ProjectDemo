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
    public class GiveCountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GiveCountController> _logger;
        private readonly IGiveCountRepository _giveCountRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public GiveCountController(IMapper mapper, IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository, ILogger<GiveCountController> logger, IGiveCountRepository giveCountRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _giveCountRepository = giveCountRepository;
            _logger = logger;
            _employeeRepository = employeeRepository;
        }


        [HttpPost("qwe")]
        public async Task<Message> addd()
        {
            List<GiveCount> List = new List<GiveCount>();
            Random r = new Random();
            for (int i = 0; i < 7000; i++)
            {
                GiveCount company = new GiveCount()
                {
                    GiveRecordId = Method.GetGuid32(),
                    AddAt = DateTime.Now.AddDays(r.Next(1, 355)).AddHours(r.Next(0, 23)).AddMinutes(r.Next(0, 60)).AddSeconds(r.Next(0, 60)),
                    GiveCountType = i % 2 == 1 ? "A400001" : "A400002",

                    GiveNumber = r.Next(1, 100),
                    Remarks = "测试",
                    JobId = i % 2 == 1 ? "2016121101" : "2016121101"
                };
                List.Add(company);
            }
            for (int i = 0; i < 300; i++)
            {
                GiveCount company = new GiveCount()
                {
                    GiveRecordId = Method.GetGuid32(),
                    AddAt = DateTime.Now.AddDays(r.Next(1, 355)).AddHours(r.Next(0, 23)).AddMinutes(r.Next(0, 60)).AddSeconds(r.Next(0, 60)),
                    GiveCountType = "A400003",

                    GiveNumber = r.Next(1, 100),
                    Remarks = "测试",
                    JobId = "2016121103"
                };
                List.Add(company);
            }
            _giveCountRepository.AddRange(List);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 次数发放
        /// </summary>
        /// <param name="giveCountCreation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostMoney([FromBody] GiveCountCreation giveCountCreation)
        {
            if (giveCountCreation == null)
            {
                return Message.Fail();
            }

            giveCountCreation.GiveRecordId = Method.GetGuid32();

            var dbEmp = await _employeeRepository.GetSingleAsync(x => x.JobId == giveCountCreation.JobId);
            if (dbEmp == null)
            {
                return Message.NotFound().Add("content", "工号错误");
            }
            dbEmp.AvailableCount += giveCountCreation.GiveNumber;
            dbEmp.UpdateCountAt = DateTime.Now;
            var newItem = _mapper.Map<GiveCount>(giveCountCreation);
            _giveCountRepository.Add(newItem);
            _employeeRepository.Update(dbEmp);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 员工发放记录查询
        /// </summary>

        /// <returns></returns>
        [HttpPost("givecountrecord")]
        public async Task<Message> giveCountRecord([FromBody] Dictionary<string, object> dictionary)
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
            var items = await _giveCountRepository.GiveCountRecord(jobid, start, end, employeename, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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