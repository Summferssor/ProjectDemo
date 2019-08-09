using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelCreation;
using BCM.Models.BCM.BCMModels.ModelModification;
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
    public class CompanyController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyRepository _companyRepository;
        public CompanyController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<CompanyController> logger, ICompanyRepository companyRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _companyRepository = companyRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取公司列表
        /// </summary>
        /// <returns>companyList</returns>
        [HttpGet("list")]
        public async Task<Message> CompanyList()
        {
            var items = await _companyRepository.GetCompanyList().ToListAsync();
            return Message.Ok().Add("companyList", items);
        }

        /// <summary>
        /// 获取公司部门列表
        /// </summary>
        /// <param name="id">companyId</param>
        /// <returns>departmentList</returns>
        [HttpGet("department/list/{id}")]
        public async Task<Message> DepartmentList(string id)
        {
            var items = await _companyRepository.GetDepartmentList(id).ToListAsync();
            return Message.Ok().Add("departmentList", items);
        }

        /// <summary>
        /// 获取公司部门列表树
        /// </summary>
        /// <returns>companyTree</returns>
        [HttpGet("tree")]
        public async Task<Message> Tree()
        {
            var items = await _companyRepository.GetCompanyTree().ToListAsync();
            return Message.Ok().Add("companyTree", items);
        }

        /// <summary>
        /// 公司分页查询
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
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

            var items = await _companyRepository.Page(searchString, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
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
        /// 部门分页查询
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="parentId">上级部门Id</param>
        /// <returns></returns>
        [HttpPost("department/page")]
        public async Task<Message> IndexDepartmentPage([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("searchString") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize") || !dictionary.ContainsKey("parentId"))
            {
                return Message.Fail().Add("content", "参数错误");
            }
            string searchString = (string)dictionary["searchString"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];
            string parentId = (string)dictionary["parentId"];
            var items = await _companyRepository.DepartmentPage(searchString, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), parentId);
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

        // [HttpPost("qwe")]
        // public async Task<Message> addd()
        // {
        //     List<Company> comList = new List<Company>();
        //     for (int i = 0; i < 1000; i++)
        //     {
        //         Company company = new Company()
        //         {
        //             Id = Method.GetGuid32(),
        //             ParentId = "0",
        //             Name = "公" + i
        //         };
        //         comList.Add(company);
        //         _companyRepository.AddRange(comList);
        //     }
        //     if (!await _unitOfWork.SaveAsync())
        //     {
        //         return Message.ServerError();
        //     }
        //     return Message.Ok();
        // }
        
        /// <summary>
        /// 添加公司
        /// </summary>
        /// <param name="companyCreation"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Message> PostCompany([FromBody] CompanyCreation companyCreation)
        {
            if (companyCreation == null)
            {
                return Message.Fail();
            }
            var dbitem = await _companyRepository.GetSingleAsync(x => x.Name == companyCreation.Name);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "公司重复");
            }
            companyCreation.Id = Method.GetGuid32();
            companyCreation.ParentId = "0";


            var newItem = _mapper.Map<Company>(companyCreation);
            _companyRepository.Add(newItem);

            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="companyCreation"></param>
        /// <returns></returns>
        [HttpPost("department")]
        public async Task<Message> Postdepartment([FromBody] CompanyCreation companyCreation)
        {
            if (companyCreation == null || string.IsNullOrEmpty(companyCreation.ParentId))
            {
                return Message.Fail().Add("content", "未选择上级部门");
            }
            var dbcom = await _companyRepository.GetSingleAsync(x => x.Id == companyCreation.ParentId);
            if (dbcom == null)
            {
                return Message.Fail().Add("content", "上级部门不存在");
            }
            var dbitem = await _companyRepository.GetSingleAsync(x => x.Name == companyCreation.Name);
            if (dbitem != null)
            {
                return Message.Fail().Add("content", "部门重复");
            }
            companyCreation.Id = Method.GetGuid32();
            // companyCreation.ParentId = "0";
            var newItem = _mapper.Map<Company>(companyCreation);
            _companyRepository.Add(newItem);

            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }


        /// <summary>
        /// 修改公司信息
        /// </summary>
        /// <param name="id">公司Id</param>
        /// <param name="companyModification">实体信息</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<Message> PutCompany(string id, [FromBody] CompanyModification companyModification)
        {
            if (companyModification == null)
            {
                return Message.Fail();
            }


            var dbitem = await _companyRepository.GetSingleAsync(x => x.Id == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }


            var dbitemname = await _companyRepository.GetSingleAsync(x => x.Name == companyModification.Name && x.Id != id);
            if (dbitemname != null)
            {
                return Message.Fail().Add("content", "公司重复");
            }
            companyModification.Id = id;
            _mapper.Map(companyModification, dbitem);
            _companyRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="id">部门Id</param>
        /// <param name="companyModification"></param>
        /// <returns></returns>
        [HttpPut("department/{id}")]
        public async Task<Message> Putdepartment(string id, [FromBody] CompanyModification companyModification)
        {
            if (companyModification == null)
            {
                return Message.Fail();
            }


            var dbitem = await _companyRepository.GetSingleAsync(x => x.Id == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            var dbcom = await _companyRepository.GetSingleAsync(x => x.Id == companyModification.ParentId);
            if (dbcom == null)
            {
                return Message.Fail().Add("content", "上级部门不存在");
            }
            var dbitemname = await _companyRepository.GetSingleAsync(x => x.Name == companyModification.Name && x.Id != id);
            if (dbitemname != null)
            {
                return Message.Fail().Add("content", "部门重复");
            }
            companyModification.Id = id;
            _mapper.Map(companyModification, dbitem);
            _companyRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 修改排序
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="companyModification"></param>
        /// <returns></returns>
        [HttpPut("sort/{id}")]
        public async Task<Message> PutSort(string id, [FromBody] CompanyModification companyModification)
        {
            var dbitem = await _companyRepository.GetSingleAsync(x => x.Id == id);
            if (dbitem == null)
            {
                return Message.NotFound();
            }
            companyModification.Id = id;
            _mapper.Map(companyModification, dbitem);
            _companyRepository.Update(dbitem);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 删除公司
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<Message> DeleteCompany(string id)
        {
            var departmentmodule = await _companyRepository.FindBy(x => x.ParentId == id).ToListAsync();
            var module = await _companyRepository.GetSingleAsync(x=>x.Id == id);
            if (module == null)
            {
                return Message.NotFound();
            }
            _companyRepository.DeleteRange(departmentmodule);
            _companyRepository.Delete(module);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        [HttpDelete("department/{id}")]
        public async Task<Message> Deletedepartment(string id)
        {
            var module = await _companyRepository.GetSingleAsync(x => x.Id == id);
            if (module == null)
            {
                return Message.NotFound();
            }
            _companyRepository.Delete(module);
            if (!await _unitOfWork.SaveAsync())
            {
                return Message.ServerError();
            }
            return Message.Ok();
        }
    }
}