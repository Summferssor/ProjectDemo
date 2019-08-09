using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BCM.Api.Controllers
{
    [EnableCors("CorsPolicy")] // 跨域
    [Route("BcmSystem/[controller]")]
    [ApiController]
    public class XlsxController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUseRecordRepository _useRecordRepository;
        private readonly IGiveCountRepository _giveCountRepository;
        private readonly IGiveMoneyRepository _giveMoneyRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public XlsxController(IHostingEnvironment hostingEnvironment, IUnitOfWork unitOfWork, IGiveMoneyRepository giveMoneyRepository, IGiveCountRepository giveCountRepository, IEmployeeRepository employeeRepository, ICompanyRepository companyRepository, IUseRecordRepository useRecordRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _useRecordRepository = useRecordRepository;
            _giveCountRepository = giveCountRepository;
            _giveMoneyRepository = giveMoneyRepository;
        }

        /// <summary>
        /// 导入员工
        /// </summary>
        /// <param name="excelfile">文件</param>
        /// <returns></returns>
        [HttpPost("importEmployee")]
        public async Task<Message> ImportEmployee(IFormFile excelfile)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            string sFileName = $"职工_导入表.xlsx";
            //  string sFileName = $"{Method.GetGuid32()}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            try
            {
                if (file.Exists)
                {
                    file.Delete();
                    file = new FileInfo(path);
                }
                using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                {
                    excelfile.CopyTo(fs);
                    fs.Flush();
                }
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[1]; //第1张Sheet
                    int rowCount = ws.Dimension.Rows;
                    int ColCount = ws.Dimension.Columns;
                    List<Employee> employees = new List<Employee>();
                    Dictionary<string, string> fail_import = new Dictionary<string, string>();
                    for (int row = 2; row <= rowCount; row++) //第1行是列名,跳过
                    {
                        var employee = new Employee();
                        employee.JobId = ws.Cells[row, 1].Value.ToString();
                        var dbitem = await _employeeRepository.GetSingleAsync(x => x.JobId == employee.JobId);
                        if (dbitem != null)
                        {
                            fail_import.Add(dbitem.JobId, "工号重复");
                            continue;
                        }
                        employee.Name = ws.Cells[row, 2].Value == null ? "" : ws.Cells[row, 2].Value.ToString();
                        employee.Remarks = ws.Cells[row, 7].Value == null ? "" : ws.Cells[row, 6].Value.ToString();
                        var companyname = ws.Cells[row, 3].Value == null ? "" : ws.Cells[row, 3].Value.ToString();
                        var departmentname = ws.Cells[row, 4].Value == null ? "" : ws.Cells[row, 4].Value.ToString();
                        employee.Numbers = ws.Cells[row, 5].Value == null ? 0 : Convert.ToInt32(ws.Cells[row, 5].Value);
                        employee.QrCodeNumber =  ws.Cells[row, 6].Value == null ? "" : ws.Cells[row, 6].Value.ToString();
                        var dbcompany = await _companyRepository.GetSingleAsync(x => x.Name == companyname);
                        if (dbcompany == null)
                        {
                            fail_import.Add(employee.JobId, "公司错误");
                            continue;
                        }
                        var dbdepartment = await _companyRepository.GetSingleAsync(x => x.Name == departmentname && x.ParentId == dbcompany.Id);
                        if (dbdepartment == null)
                        {
                            fail_import.Add(employee.JobId, "部门错误");
                            continue;
                        }

                        employee.CompanyId = dbdepartment.Id;
                        employee.Sort = await _employeeRepository.OrderNumAsync(employee.CompanyId) + row - 3;
                        employee.AddAt = DateTime.Now;
                        employee.Password = Method.MD5Encryption(employee.JobId.Trim());
                        employee.AvailableCount = employee.Numbers;
                        employee.UpdateCountAt = DateTime.Now;
                        employee.AvailableMoney = 0;
                        employee.UpdateMoneyAt = DateTime.Now;
                        employees.Add(employee);
                    }
                    int totle = rowCount - 1;
                    int failCount = fail_import.Count;
                    _employeeRepository.AddRange(employees);
                    if (!await _unitOfWork.SaveAsync())
                    {
                        Message.ServerError();
                    }
                    return Message.Ok().Add("fail_import", fail_import).Add("totle", totle).Add("failCount", failCount);

                }
            }
            catch (Exception ex)
            {
                return Message.ServerError().Add("exception", ex.Message);
            }
        }

        /// <summary>
        /// 导出当前页员工
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("exportEmployee")]
        public async Task<IActionResult> Exportemployee([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return BadRequest();
            }

            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];

            var items = await _employeeRepository.Page(companyid, employeename, jobid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"职工_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("EmployeeExport");
                worksheet.Cells[1, 1].Value = "序号";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "姓名";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "发放次数";
                worksheet.Cells[1, 5].Value = "所属公司";
                worksheet.Cells[1, 6].Value = "备注";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].sort;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].name;
                    worksheet.Cells[2 + i, 3].Value = items[i].jobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].numbers;
                    worksheet.Cells[2 + i, 5].Value = items[i].companyName;
                    worksheet.Cells[2 + i, 6].Value = items[i].remarks;
                    // worksheet.Cells[2 + i, 7].Value = users[i].CreateTime.ToString();//日期tostring一下，不然是乱的
                    // worksheet.Cells[2 + i, 8].Value = users[i].UpdateTime.ToString();
                }
                // worksheet.Cells.LoadFromCollection(items, true);
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }

        /// <summary>
        /// 导出所有员工
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost("exportAllEmployee")]
        public async Task<IActionResult> ExportAllEmployee([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId"))
            {
                return BadRequest();
            }

            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];


            var items = await _employeeRepository.AllEmployees(companyid, employeename, jobid).ToListAsync();
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"职工_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("EmployeeExport");
                // ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("EmployeeExport");
                worksheet.Cells[1, 1].Value = "序号";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "姓名";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "发放次数";
                worksheet.Cells[1, 5].Value = "所属公司";
                worksheet.Cells[1, 6].Value = "备注";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].sort;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].name;
                    worksheet.Cells[2 + i, 3].Value = items[i].jobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].numbers;
                    worksheet.Cells[2 + i, 5].Value = items[i].companyName;
                    worksheet.Cells[2 + i, 6].Value = items[i].remarks;
                    // worksheet.Cells[2 + i, 7].Value = users[i].CreateTime.ToString();//日期tostring一下，不然是乱的
                    // worksheet.Cells[2 + i, 8].Value = users[i].UpdateTime.ToString();
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <returns></returns>
        [HttpPost("Template")]
        public IActionResult DownloadTemplate()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"Template.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Template");
                // ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("EmployeeExport");
                worksheet.Cells[1, 1].Value = "工号";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "姓名";
                worksheet.Cells[1, 3].Value = "公司";
                worksheet.Cells[1, 4].Value = "部门";
                worksheet.Cells[1, 5].Value = "发放次数";
                worksheet.Cells[1, 6].Value = "二维码特征码";
                worksheet.Cells[1, 7].Value = "备注";
                worksheet.Cells[2, 1].Value = "ps:第一行不可改动，删除此行！";//这的*是字段名
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"Template.xlsx");

        }

        /// <summary>
        /// 导出员工次数与金额当前页
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("exportcountandnum")]
        public async Task<IActionResult> ExportCountAndNum([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return BadRequest();
            }

            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            Int64 pageIndex = (Int64)dictionary["pageIndex"];
            Int64 pageSize = (Int64)dictionary["pageSize"];

            var items = await _employeeRepository.PageCountNum(companyid, employeename, jobid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工次数和金额_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("EmployeeExport");
                worksheet.Cells[1, 1].Value = "工号";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "姓名";
                worksheet.Cells[1, 3].Value = "公司名称";
                worksheet.Cells[1, 4].Value = "部门名称";
                worksheet.Cells[1, 5].Value = "剩余次数";
                worksheet.Cells[1, 6].Value = "钱包金额";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].jobId;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].employeeName;
                    worksheet.Cells[2 + i, 3].Value = items[i].companyName;
                    worksheet.Cells[2 + i, 4].Value = items[i].departmentName;
                    worksheet.Cells[2 + i, 5].Value = items[i].availableCount;
                    worksheet.Cells[2 + i, 6].Value = items[i].availableMoney;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_count_num_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }


        /// <summary>
        /// 导出所有员工次数与金额
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost("exportallcountandnum")]
        public async Task<IActionResult> ExportAllCountAndNum([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId"))
            {
                return BadRequest();
            }

            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];

            var items = await _employeeRepository.AllEmployeesCountNum(companyid, employeename, jobid).ToListAsync();
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工次数和金额_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("EmployeeExport");
                // ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("EmployeeExport");
                worksheet.Cells[1, 1].Value = "工号";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "姓名";
                worksheet.Cells[1, 3].Value = "公司名称";
                worksheet.Cells[1, 4].Value = "部门名称";
                worksheet.Cells[1, 5].Value = "剩余次数";
                worksheet.Cells[1, 6].Value = "钱包金额";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].jobId;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].employeeName;
                    worksheet.Cells[2 + i, 3].Value = items[i].companyName;
                    worksheet.Cells[2 + i, 4].Value = items[i].departmentName;
                    worksheet.Cells[2 + i, 5].Value = items[i].availableCount;
                    worksheet.Cells[2 + i, 6].Value = items[i].availableMoney;
                    // worksheet.Cells[2 + i, 7].Value = users[i].CreateTime.ToString();//日期tostring一下，不然是乱的
                    // worksheet.Cells[2 + i, 8].Value = users[i].UpdateTime.ToString();
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_count_num_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        }

        /// <summary>
        /// 导出员工记录明细当前页
        /// </summary>
        /// <param name="usageType"></param>
        /// <param name="consumptionType"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportrecorddetail")]
        public async Task<IActionResult> ExportRecordDetail([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("usageType") || !dictionary.ContainsKey("consumptionType") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return BadRequest();
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
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _useRecordRepository.RecordDetail(jobid, start, end, usagetype, consumptiontype, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工记录明细_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "时间";
                worksheet.Cells[1, 6].Value = "消费类型";
                worksheet.Cells[1, 7].Value = "使用类型";
                worksheet.Cells[1, 8].Value = "发放类型";
                worksheet.Cells[1, 9].Value = "次数";
                worksheet.Cells[1, 10].Value = "金额";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].CompanyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].DepartmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].JobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].EmployeeName;
                    worksheet.Cells[2 + i, 5].Value = items[i].AddAt.ToString(); ;
                    worksheet.Cells[2 + i, 6].Value = items[i].ConsumptionType;
                    worksheet.Cells[2 + i, 7].Value = items[i].UsageType;
                    worksheet.Cells[2 + i, 8].Value = items[i].GiveCountType;
                    worksheet.Cells[2 + i, 9].Value = items[i].UseNumber;
                    worksheet.Cells[2 + i, 10].Value = items[i].UseMoney;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"use_record_detail_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }


        /// <summary>
        /// 导出员工记录明细所有
        /// </summary>
        /// <param name="usageType"></param>
        /// <param name="consumptionType"></param>
        /// <param name="jobId"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportallrecorddetail")]
        public async Task<IActionResult> ExportAllRecordDetail([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("usageType") || !dictionary.ContainsKey("consumptionType") || !dictionary.ContainsKey("jobId"))
            {
                return BadRequest();
            }
            string usagetype = (string)dictionary["usageType"];
            string consumptiontype = (string)dictionary["consumptionType"];
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            string jobid = (string)dictionary["jobId"];
            DateTime yaer1 = Method.StampToDateTime(start);
            DateTime year2 = Method.StampToDateTime(end);
            if (year2.Year != yaer1.Year)
            {
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _useRecordRepository.AllRecordDetail(jobid, start, end, usagetype, consumptiontype).ToListAsync();
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工记录明细_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "时间";
                worksheet.Cells[1, 6].Value = "消费类型";
                worksheet.Cells[1, 7].Value = "使用类型";
                worksheet.Cells[1, 8].Value = "发放类型";
                worksheet.Cells[1, 9].Value = "次数";
                worksheet.Cells[1, 10].Value = "金额";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].CompanyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].DepartmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].JobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].EmployeeName;
                    worksheet.Cells[2 + i, 5].Value = items[i].AddAt.ToString(); ;
                    worksheet.Cells[2 + i, 6].Value = items[i].ConsumptionType;
                    worksheet.Cells[2 + i, 7].Value = items[i].UsageType;
                    worksheet.Cells[2 + i, 8].Value = items[i].GiveCountType;
                    worksheet.Cells[2 + i, 9].Value = items[i].UseNumber;
                    worksheet.Cells[2 + i, 10].Value = items[i].UseMoney;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"use_record_detail_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        }



        /// <summary>
        /// 导出员工发放记录当前页
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportgivecountrecord")]
        public async Task<IActionResult> ExportGiveCountRecord([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return BadRequest();
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
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _giveCountRepository.GiveCountRecord(jobid, start, end, employeename, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工发放记录_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "发放时间";
                worksheet.Cells[1, 6].Value = "发放次数";
                worksheet.Cells[1, 7].Value = "发放类型";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].companyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].departmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].jobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].name;
                    worksheet.Cells[2 + i, 5].Value = items[i].giveTime.ToString();
                    worksheet.Cells[2 + i, 6].Value = items[i].giveNumber;
                    worksheet.Cells[2 + i, 7].Value = items[i].giveType;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_count_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }


        /// <summary>
        /// 导出员工发放记录所有
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportallgivecountrecord")]
        public async Task<IActionResult> ExportAllGiveCountRecord([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId"))
            {
                return BadRequest();
            }
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            DateTime yaer1 = Method.StampToDateTime(start);
            DateTime year2 = Method.StampToDateTime(end);
            if (year2.Year != yaer1.Year)
            {
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _giveCountRepository.AllGiveCountRecord(jobid, start, end, employeename, companyid).ToListAsync();
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工发放记录_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "发放时间";
                worksheet.Cells[1, 6].Value = "发放次数";
                worksheet.Cells[1, 7].Value = "发放类型";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].companyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].departmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].jobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].name;
                    worksheet.Cells[2 + i, 5].Value = items[i].giveTime.ToString();
                    worksheet.Cells[2 + i, 6].Value = items[i].giveNumber;
                    worksheet.Cells[2 + i, 7].Value = items[i].giveType;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_count_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        }

        /// <summary>
        /// 导出员工次数转钱包当前页
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportgivemoneyrecord")]
        public async Task<IActionResult> ExportGiveMoneyRecord([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return BadRequest();
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
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _giveMoneyRepository.GiveMoneyRecord(jobid, start, end, employeename, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工次数转钱包记录_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "发放时间";
                worksheet.Cells[1, 6].Value = "金额";
                worksheet.Cells[1, 7].Value = "发放类型";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].companyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].departmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].jobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].name;
                    worksheet.Cells[2 + i, 5].Value = items[i].giveTime.ToString();
                    worksheet.Cells[2 + i, 6].Value = items[i].giveMoney;
                    worksheet.Cells[2 + i, 7].Value = items[i].giveType;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_money_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }
        
        /// <summary>
        /// 导出员工次数转钱包所有
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportallgivemoneyrecord")]
        public async Task<IActionResult> ExportAllExportGiveMoneyRecord([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId"))
            {
                return BadRequest();
            }
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            DateTime yaer1 = Method.StampToDateTime(start);
            DateTime year2 = Method.StampToDateTime(end);
            if (year2.Year != yaer1.Year)
            {
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _giveMoneyRepository.AllGiveMoneyRecord(jobid, start, end, employeename, companyid).ToListAsync();
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工次数转钱包记录_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "发放时间";
                worksheet.Cells[1, 6].Value = "金额";
                worksheet.Cells[1, 7].Value = "发放类型";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].companyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].departmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].jobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].name;
                    worksheet.Cells[2 + i, 5].Value = items[i].giveTime.ToString();
                    worksheet.Cells[2 + i, 6].Value = items[i].giveMoney;
                    worksheet.Cells[2 + i, 7].Value = items[i].giveType;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_money_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        }


        /// <summary>
        /// 导出员工消费记录当前页
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportconsumerecord")]
        public async Task<IActionResult> ExportConsumeRecord([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
            {
                return BadRequest();
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
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _useRecordRepository.ConsumeRecord(jobid, start, end, employeename, companyid, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工消费记录_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "消费时间";
                worksheet.Cells[1, 6].Value = "金额/次数";
                worksheet.Cells[1, 7].Value = "消费类型";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].CompanyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].DepartmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].JobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].EmployeeName;
                    worksheet.Cells[2 + i, 5].Value = items[i].AddAt.ToString();
                    worksheet.Cells[2 + i, 6].Value = items[i].CountOrMoney;
                    worksheet.Cells[2 + i, 7].Value = items[i].ConsumeType;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_consume_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }


        /// <summary>
        /// 导出员工消费记录所有
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeName"></param>
        /// <param name="jobId"></param>
        /// <param name="timeEnd"></param>
        /// <param name="timeStart"></param>
        /// <returns></returns>
        [HttpPost("exportallconsumerecord")]
        public async Task<IActionResult> ExportAllConsumeRecord([FromBody] Dictionary<string, object> dictionary)
        {
            if (!dictionary.ContainsKey("timeEnd") || !dictionary.ContainsKey("timeStart") || !dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("jobId"))
            {
                return BadRequest();
            }
            string companyid = (string)dictionary["companyId"];
            string employeename = (string)dictionary["employeeName"];
            string jobid = (string)dictionary["jobId"];
            string end = (string)dictionary["timeEnd"];
            string start = (string)dictionary["timeStart"];
            DateTime yaer1 = Method.StampToDateTime(start);
            DateTime year2 = Method.StampToDateTime(end);
            if (year2.Year != yaer1.Year)
            {
                return BadRequest("不支持跨年导出，请选择年份相同的时间");
            }
            var items = await _useRecordRepository.AllConsumeRecord(jobid, start, end, employeename, companyid).ToListAsync();
            string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
            // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            string sFileName = $"员工消费记录_导出表.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
                worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
                worksheet.Cells[1, 2].Value = "部门";
                worksheet.Cells[1, 3].Value = "工号";
                worksheet.Cells[1, 4].Value = "姓名";
                worksheet.Cells[1, 5].Value = "消费时间";
                worksheet.Cells[1, 6].Value = "金额/次数";
                worksheet.Cells[1, 7].Value = "消费类型";
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[2 + i, 1].Value = items[i].CompanyName;//这的*是字段名
                    worksheet.Cells[2 + i, 2].Value = items[i].DepartmentName;
                    worksheet.Cells[2 + i, 3].Value = items[i].JobId;
                    worksheet.Cells[2 + i, 4].Value = items[i].EmployeeName;
                    worksheet.Cells[2 + i, 5].Value = items[i].AddAt.ToString();
                    worksheet.Cells[2 + i, 6].Value = items[i].CountOrMoney;
                    worksheet.Cells[2 + i, 7].Value = items[i].ConsumeType;
                }
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_consume_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        }







        // /// <summary>
        // /// 导出个人就餐消费明细当前页
        // /// </summary>
        // /// <param name="companyId">公司部门id</param>
        // /// <param name="period">周期</param>
        // /// <param name="employeeName">员工姓名</param>
        // /// <param name="pageIndex">当前页号</param>
        // /// <param name="pageSize">页大小</param>
        // /// <returns></returns>
        // [HttpPost("exportpersonconsume")]
        // public async Task<IActionResult> ExportPersonConsumeRecord([FromBody] Dictionary<string, object> dictionary)
        // {
        //     if (!dictionary.ContainsKey("period") ||!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName") || !dictionary.ContainsKey("pageIndex") || !dictionary.ContainsKey("pageSize"))
        //     {
        //         return BadRequest();
        //     }
        //     string companyid = (string)dictionary["companyId"];
        //     string employeename = (string)dictionary["employeeName"];
        //     Int64 pageIndex = (Int64)dictionary["pageIndex"];
        //     Int64 pageSize = (Int64)dictionary["pageSize"];
        //     string period = (string)dictionary["period"];
        //     var items = await _employeeRepository.PersonPayoffTable(companyid, employeename, period, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize));
        //     string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
        //     // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
        //     string sFileName = $"个人消费明细.xlsx";
        //     var path = Path.Combine(sWebRootFolder, sFileName);
        //     FileInfo file = new FileInfo(path);

        //     if (file.Exists)
        //     {
        //         file.Delete();
        //         file = new FileInfo(path);
        //     }
        //     using (ExcelPackage package = new ExcelPackage(file))
        //     {
        //         //创建sheet
        //         ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
        //         worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
        //         worksheet.Cells[1, 2].Value = "部门";
        //         worksheet.Cells[1, 3].Value = "姓名";
        //         worksheet.Cells[1, 4].Value = "早餐次数";
        //         worksheet.Cells[1, 5].Value = "金额（元）";
        //         worksheet.Cells[1, 6].Value = "午餐次数";
        //         worksheet.Cells[1, 7].Value = "金额（元）";
        //         worksheet.Cells[1, 8].Value = "晚餐次数";
        //         worksheet.Cells[1, 9].Value = "金额（元）";
        //         worksheet.Cells[1, 10].Value = "夜宵次数";
        //         worksheet.Cells[1, 11].Value = "金额（元）";
        //         worksheet.Cells[1, 12].Value = "消费次数合计（次）";
        //         worksheet.Cells[1, 13].Value = "消费金额合计（元）";
        //         for (int i = 0; i < items.Count; i++)
        //         {
        //             worksheet.Cells[2 + i, 1].Value = items[i].CompanyName;//这的*是字段名
        //             worksheet.Cells[2 + i, 2].Value = items[i].DepartmentName;
        //             worksheet.Cells[2 + i, 3].Value = items[i].EmployeeName;
        //             worksheet.Cells[2 + i, 4].Value = items[i].NumberOfBreakfast;
        //             worksheet.Cells[2 + i, 5].Value = items[i].MoneyOfBreakfast;
        //             worksheet.Cells[2 + i, 6].Value = items[i].NumberOfLunch;
        //             worksheet.Cells[2 + i, 7].Value = items[i].MoneyOfLunch;
        //             worksheet.Cells[2 + i, 8].Value = items[i].NumberOfDinner;
        //             worksheet.Cells[2 + i, 9].Value = items[i].MoneyOfDinner;
        //             worksheet.Cells[2 + i, 10].Value = items[i].NumberOfMidnightsnack;
        //             worksheet.Cells[2 + i, 11].Value = items[i].MoneyOfMidnightsnack;
        //             worksheet.Cells[2 + i, 12].Value = items[i].TotalNumber;
        //             worksheet.Cells[2 + i, 13].Value = items[i].TotalMoney;
        //         }
        //         package.Save(); //Save the workbook.
        //     }
        //     return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_consume_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        // }


        // /// <summary>
        // /// 导出个人就餐消费明细所有
        // /// </summary>
        // /// <param name="companyId">公司部门id</param>
        // /// <param name="period">周期</param>
        // /// <param name="employeeName">员工姓名</param>
        // /// <returns></returns>
        // [HttpPost("exportallpersonconsume")]
        // public async Task<IActionResult> ExportAllPersonConsumeRecord([FromBody] Dictionary<string, object> dictionary)
        // {
        //     if (!dictionary.ContainsKey("period") ||!dictionary.ContainsKey("companyId") || !dictionary.ContainsKey("employeeName"))
        //     {
        //         return BadRequest();
        //     }
        //     string companyid = (string)dictionary["companyId"];
        //     string employeename = (string)dictionary["employeeName"];
        //     string period = (string)dictionary["period"];
        //     var items = await _employeeRepository.AllPersonPayoffTable(companyid, employeename, period).ToListAsync();
        //     string sWebRootFolder = _hostingEnvironment.WebRootPath + @"/excel";
        //     // string sFileName = $@"UserExport{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
        //     string sFileName = $"个人消费明细.xlsx";
        //     var path = Path.Combine(sWebRootFolder, sFileName);
        //     FileInfo file = new FileInfo(path);

        //     if (file.Exists)
        //     {
        //         file.Delete();
        //         file = new FileInfo(path);
        //     }
        //     using (ExcelPackage package = new ExcelPackage(file))
        //     {
        //         //创建sheet
        //         ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RecordDetail");
        //         worksheet.Cells[1, 1].Value = "公司";//这的*号表示的是导出后列的名称
        //         worksheet.Cells[1, 2].Value = "部门";
        //         worksheet.Cells[1, 3].Value = "姓名";
        //         worksheet.Cells[1, 4].Value = "早餐次数";
        //         worksheet.Cells[1, 5].Value = "金额（元）";
        //         worksheet.Cells[1, 6].Value = "午餐次数";
        //         worksheet.Cells[1, 7].Value = "金额（元）";
        //         worksheet.Cells[1, 8].Value = "晚餐次数";
        //         worksheet.Cells[1, 9].Value = "金额（元）";
        //         worksheet.Cells[1, 10].Value = "夜宵次数";
        //         worksheet.Cells[1, 11].Value = "金额（元）";
        //         worksheet.Cells[1, 12].Value = "消费次数合计（次）";
        //         worksheet.Cells[1, 13].Value = "消费金额合计（元）";
        //         for (int i = 0; i < items.Count; i++)
        //         {
        //             worksheet.Cells[2 + i, 1].Value = items[i].CompanyName;//这的*是字段名
        //             worksheet.Cells[2 + i, 2].Value = items[i].DepartmentName;
        //             worksheet.Cells[2 + i, 3].Value = items[i].EmployeeName;
        //             worksheet.Cells[2 + i, 4].Value = items[i].NumberOfBreakfast;
        //             worksheet.Cells[2 + i, 5].Value = items[i].MoneyOfBreakfast;
        //             worksheet.Cells[2 + i, 6].Value = items[i].NumberOfLunch;
        //             worksheet.Cells[2 + i, 7].Value = items[i].MoneyOfLunch;
        //             worksheet.Cells[2 + i, 8].Value = items[i].NumberOfDinner;
        //             worksheet.Cells[2 + i, 9].Value = items[i].MoneyOfDinner;
        //             worksheet.Cells[2 + i, 10].Value = items[i].NumberOfMidnightsnack;
        //             worksheet.Cells[2 + i, 11].Value = items[i].MoneyOfMidnightsnack;
        //             worksheet.Cells[2 + i, 12].Value = items[i].TotalNumber;
        //             worksheet.Cells[2 + i, 13].Value = items[i].TotalMoney;
        //         }
        //         package.Save(); //Save the workbook.
        //     }
        //     return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"employee_consume_record{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

        // }
    }
}
