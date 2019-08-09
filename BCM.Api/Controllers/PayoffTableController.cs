using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BCM.Api.Controllers
{
    [Authorize]
    [EnableCors("CorsPolicy")] // ����
    [Route("BcmSystem/[controller]")]
    public class PayoffTableController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PayoffTableController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompanyRepository _companyRepository;
        public PayoffTableController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<PayoffTableController> logger, ICompanyRepository companyRepository, IEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _logger = logger;
            _companyRepository = companyRepository;
        }

        /// <summary>
        /// 个人就餐消费明细
        /// </summary>
        /// <param name="companyId">公司部门id</param>
        /// <param name="period">周期</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="pageIndex">当前页号</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [HttpPost("person")]
        public async Task<Message> IndexPage([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];
            var items = await _employeeRepository.PersonPayoffTable(start, end, companyid, employeename, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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
        /// 公司就餐消费明细
        /// </summary>
        /// <param name="companyId">公司部门id</param>
        /// <param name="pageIndex">当前页号</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [HttpPost("company")]
        public async Task<Message> CompanyPayoffPage([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            string companyid = (string)dictionary["companyId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];
            var items = await _companyRepository.CompanyPayoffTable(start, end, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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
        /// 充值及消费情况汇总统计表
        /// </summary>
        /// <param name="companyId">公司部门id</param>
        /// <param name="pageIndex">当前页号</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [HttpPost("topup")]
        public async Task<Message> TopUpPayoffPage([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];
            var items = await _employeeRepository.PersonTopUpTable(start, end, companyid, employeename, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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
        /// 就餐消费结算表
        /// </summary>
        /// <param name="companyId">公司部门id</param>
        /// <param name="pageIndex">当前页号</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [HttpPost("settlement")]
        public async Task<Message> SettlementTable([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            string companyid = (string)dictionary["companyId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];
            var items = await _companyRepository.Settlement(start, end, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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