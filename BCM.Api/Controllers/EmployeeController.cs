using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BCM.Common;
using BCM.Common.TokenHelper;
using BCM.IRepositories;
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
    [Authorize]
    [EnableCors("CorsPolicy")] // ����
    [Route("BcmSystem/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<EmployeeController> logger, IEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有员工
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Message> GetAllEmployee()
        {
            var items = await _employeeRepository.All.ToListAsync();
            var results = _mapper.Map<IEnumerable<EmployeeView>>(items);
            return Message.Ok().Add("employeeList", results);
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">工号</param>
        /// <returns></returns>
        [HttpPost("resetpwd/{id}")]
        public async Task<Message> resetpwd(string id)
        {
            var dbitem = await _employeeRepository.GetSingleAsync(x => x.JobId == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            dbitem.Password = Method.MD5Encryption(dbitem.JobId.Trim());
            // employeeModification.JobId = id;
            // _mapper.Map(employeeModification, dbitem);
            _employeeRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 员工登录
        /// </summary>
        /// <param name="jobId">工号</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<Message> LoginUser([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("passWord"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string jobid = (string)dictionary["jobId"];
            string password = (string)dictionary["passWord"];

            var dbitem = await _employeeRepository.GetSingleAsync(x => x.JobId == jobid && x.Password == password);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            return Message.Ok().Add("employee", new
            {
                name = dbitem.Name,
                jobid = dbitem.JobId,
                qrcode = dbitem.QrCodeNumber
            }).Add("token", TokenContext.GetToken(dbitem.JobId, dbitem.Name));
        }

        /// <summary>
        /// 员工信息查询
        /// </summary>
        /// <param name="companyId">公司部门id</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="jobId">工号</param>
        /// <param name="pageIndex">当前页号</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [HttpPost("page")]
        public async Task<Message> IndexPage([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];

            var items = await _employeeRepository.Page(companyid, employeename, jobid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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
        /// 新增员工
        /// </summary>
        /// <param name="employeeCreation">员工实体</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostEmp([FromBody] EmployeeCreation employeeCreation)
        {
            if (employeeCreation == null)
            {
                return Message.Fail();
            }

            var dbitem = await _employeeRepository.GetSingleAsync(x => x.JobId == employeeCreation.JobId);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "工号已存在");
            }
            if (string.IsNullOrEmpty(employeeCreation.Password))
            {
                employeeCreation.Password = employeeCreation.JobId;
            }
            employeeCreation.Password = Method.MD5Encryption(employeeCreation.Password.Trim());
            var newItem = _mapper.Map<Employee>(employeeCreation);
            _employeeRepository.Add(newItem);

            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 修改员工
        /// </summary>
        /// <param name="id">工号</param>
        /// <param name="employeeModification">员工实体</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<Message> PutEmp(string id, [FromBody] EmployeeModification employeeModification)
        {
            if (employeeModification == null)
            {
                return Message.Fail();
            }


            var dbitem = await _employeeRepository.GetSingleAsync(x => x.JobId == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            employeeModification.JobId = dbitem.JobId;
            _mapper.Map(employeeModification, dbitem);
            _employeeRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">工号</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPut("mod/{id}")]
        public async Task<Message> PutEmp(string id, [FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("password"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string employeename = (string)dictionary["employeeName"];
            string password = (string)dictionary["password"];

            var dbitem = await _employeeRepository.GetSingleAsync(x => x.JobId == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            EmployeeModification employeeModification = new EmployeeModification();
            if(!string.IsNullOrEmpty(password) && password.Length < 8)
            {
                return Message.Fail().Add("content", "密码长度小于8");
            }
            employeeModification.JobId = id;
            employeeModification.Password = password;
            employeeModification.Name = employeename;
            _mapper.Map(employeeModification, dbitem);
            _employeeRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="id">工号</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<Message> DeleteEmp(string id)
        {
            var module = await _employeeRepository.GetSingleAsync(x => x.JobId == id);
            if (module == null)
            {
                return Message.NotFound();
            }
            _employeeRepository.Delete(module);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 员工次数和金额查询
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("countAndNum")]
        public async Task<Message> countAndNum([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];

            var items = await _employeeRepository.PageCountNum(companyid, employeename, jobid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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
        /// 获得员工信息
        /// </summary>
        /// <param name="id">工号</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Message> getEmpInfo(string id)
        {
            var item = await _employeeRepository.getEmployeeInfo(id);
            if (item == null)
            {
                return Message.NotFound().Add("content", "工号不存在");
            }
            return Message.Ok().Add("employee", item);
        }
    }
}