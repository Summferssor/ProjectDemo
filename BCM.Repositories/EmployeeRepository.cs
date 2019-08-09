using System.Linq;
using System;
using System.Threading.Tasks;
using BCM.IRepositories;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BCM.Models.BCM;
using System.Collections.Generic;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Common;

namespace BCM.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public override void Update(Employee entity)
        {
            EntityEntry<Employee> dbEntityEntry = Context.Entry(entity);
            
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.JobId).IsModified = false;
            if (string.IsNullOrEmpty(entity.CompanyId))
            {
                dbEntityEntry.Property(x => x.CompanyId).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.Name))
            {
                dbEntityEntry.Property(x => x.Name).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.Password))
            {
                dbEntityEntry.Property(x => x.Password).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.Numbers.ToString()))
            {
                dbEntityEntry.Property(x => x.Numbers).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.QrCodeNumber))
            {
                dbEntityEntry.Property(x => x.QrCodeNumber).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.QrCodeUrl))
            {
                dbEntityEntry.Property(x => x.QrCodeUrl).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.Remarks))
            {
                dbEntityEntry.Property(x => x.Remarks).IsModified = false;
            }

            dbEntityEntry.Property(x => x.AddAt).IsModified = false;

            if (string.IsNullOrEmpty(entity.Sort.ToString()))
            {
                dbEntityEntry.Property(x => x.Sort).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.AvailableCount.ToString()))
            {

                dbEntityEntry.Property(x => x.AvailableCount).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.AvailableMoney.ToString()))
            {

                dbEntityEntry.Property(x => x.AvailableMoney).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.UpdateCountAt.ToString()))
            {
                dbEntityEntry.Property(x => x.UpdateCountAt).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.UpdateMoneyAt.ToString()))
            {
                dbEntityEntry.Property(x => x.UpdateMoneyAt).IsModified = false;
            }
        }
        public override Employee GetSingle(string id)
        {
            return Context.Set<Employee>().FirstOrDefault(x => x.JobId == id);
        }
        public override async Task<Employee> GetSingleAsync(string id)
        {
            return await Context.Set<Employee>().FirstOrDefaultAsync(x => x.JobId == id);
        }

        public override void Add(Employee entity)
        {
            if (string.IsNullOrEmpty(entity.Sort.ToString()))
            {

                entity.Sort = Context.Set<Employee>().Where(x => x.CompanyId == entity.CompanyId).Count();

            }
            entity.AvailableCount = entity.Numbers;
            entity.UpdateCountAt = DateTime.Now;
            entity.AvailableMoney = 0;
            entity.UpdateMoneyAt = DateTime.Now;
            entity.AddAt = DateTime.Now;
            Context.Set<Employee>().Add(entity);
        }

        public async Task<PaginatedList<EmployeeInfo>> Page(string companyid, string employeename, string jobid, int page, int pageSize)
        {
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(employeename) && x.JobId.Contains(jobid)).OrderBy(y => y.Sort);
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            var dbitems = from emp in employees
                          join company in cmps
                          on emp.CompanyId.Trim() equals company.Id.Trim()
                          //   where emp.CompanyId.Trim().Contains(companyid.Trim())
                          select new EmployeeInfo
                          {
                              sort = emp.Sort,
                              name = emp.Name.Trim(),
                              jobId = emp.JobId.Trim(),
                              numbers = emp.Numbers,
                              companyName = company.Name.Trim(),
                              companyId = company.Id.Trim(),
                              remarks = emp.Remarks.Trim(),
                              password = emp.Password.Trim(),
                              qrCode = emp.QrCodeNumber.Trim()
                          };
            var Employee = await PaginatedList<EmployeeInfo>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return Employee;
        }

        public IQueryable<EmployeeInfo> AllEmployees(string companyid, string employeename, string jobid)
        {

            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(employeename) && x.JobId.Contains(jobid)).OrderBy(y => y.Sort);
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            var dbitems = from emp in employees
                          join company in cmps
                          on emp.CompanyId.Trim() equals company.Id.Trim()
                          //   where emp.CompanyId.Trim().Contains(companyid.Trim())
                          select new EmployeeInfo
                          {
                              sort = emp.Sort,
                              name = emp.Name.Trim(),
                              jobId = emp.JobId.Trim(),
                              numbers = emp.Numbers,
                              companyName = company.Name.Trim(),
                              companyId = company.Id.Trim(),
                              remarks = emp.Remarks.Trim()
                          };
            return dbitems;
        }

        public int OrderNum(string companyid)
        {
            return Context.Set<Employee>().Where(x => x.CompanyId == companyid).Count() + 1;
        }

        public async Task<int> OrderNumAsync(string companyid)
        {
            return await Context.Set<Employee>().Where(x => x.CompanyId == companyid).CountAsync() + 1;
        }

        public async Task<PaginatedList<EmployeeCountAneNum>> PageCountNum(string companyid, string employeename, string jobid, int page, int pageSize)
        {
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(employeename) && x.JobId.Contains(jobid)).OrderBy(y => y.Sort);
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            var dbitems = from emp in employees
                          join company in cmps
                          on emp.CompanyId.Trim() equals company.Id.Trim()
                          //   where emp.CompanyId.Trim().Contains(companyid.Trim())
                          select new EmployeeCountAneNum
                          {
                              jobId = emp.JobId.Trim(),
                              employeeName = emp.Name.Trim(),
                              departmentName = company.Name.Trim(),
                              departmentId = company.Id.Trim(),
                              conpanyId = company.ParentId.Trim(),
                              companyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              availableCount = emp.AvailableCount,
                              availableMoney = emp.AvailableMoney
                          };
            var Employee = await PaginatedList<EmployeeCountAneNum>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return Employee;
        }

        public IQueryable<EmployeeCountAneNum> AllEmployeesCountNum(string companyid, string employeename, string jobid)
        {
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(employeename) && x.JobId.Contains(jobid)).OrderBy(y => y.Sort);
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            var dbitems = from emp in employees
                          join company in cmps
                          on emp.CompanyId.Trim() equals company.Id.Trim()
                          //   where emp.CompanyId.Trim().Contains(companyid.Trim())
                          select new EmployeeCountAneNum
                          {
                              jobId = emp.JobId.Trim(),
                              employeeName = emp.Name.Trim(),
                              departmentName = company.Name.Trim(),
                              departmentId = company.Id.Trim(),
                              conpanyId = company.ParentId.Trim(),
                              companyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              availableCount = emp.AvailableCount,
                              availableMoney = emp.AvailableMoney
                          };
            return dbitems;
        }

        public async Task<object> getEmployeeInfo(string id)
        {
            var employee = await Context.Set<Employee>().Where(x => x.JobId == id).FirstOrDefaultAsync();
            if (employee == null)
            {
                return null;
            }
            var department = await Context.Set<Company>().Where(x => x.Id == employee.CompanyId).FirstOrDefaultAsync();
            var company = await Context.Set<Company>().Where(x => x.Id == department.ParentId).FirstOrDefaultAsync();
            return new
            {
                jobId = employee.JobId,
                employeeName = employee.Name,
                companyId = company.Id,
                companyName = company.Name,
                departmentId = department.Id,
                departmentName = department.Name,
                availableCount = employee.AvailableCount,
                availableMoney = employee.AvailableMoney
            };
        }

        public async Task<PaginatedList<PersonPayoffTableView>> PersonPayoffTable(string timestart, string timeend, string companyid, string employeename, int page, int pageSize)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(employeename)).OrderBy(y => y.Sort);
            var records = (from emp in employees
                           join ur in Context.Vw_UseRecords
                           on emp.JobId.Trim() equals ur.JobId.Trim()
                           from rule in Context.Set<Rules>()
                           where start <= ur.AddAt && ur.AddAt <= end
                           where Method.isLegalTime(Convert.ToDateTime(ur.AddAt), rule.StartTime + "-" + rule.EndTime)
                           select new
                           {
                               recordId = ur.UseRecordId,
                               useNumber = ur.UseNumber,
                               useMoney = ur.UseMoney,
                               ruleId = rule.Id,
                               ruleName = rule.Name,
                               rulePrice = rule.Price,
                               jobId = emp.JobId,
                               type = ur.GiveCountType
                           }).ToList();
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            //breakfast 
            var breakfastId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("早餐")).FirstOrDefault().Id;
            //lunch
            var lunchId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("午餐")).FirstOrDefault().Id;
            //dinner
            var dinnerId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("晚餐")).FirstOrDefault().Id;
            //snake
            var snakeId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("夜宵")).FirstOrDefault().Id;
            var dbitems = from emp in employees
                          join company in cmps
                          on emp.CompanyId.Trim() equals company.Id.Trim()
                          orderby company.Sort ascending
                          orderby emp.Sort ascending
                          select new PersonPayoffTableView
                          {
                              DepartmentName = company.Name.Trim(),
                              CompanyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              EmployeeName = emp.Name,
                              NumberOfBreakfast = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(breakfastId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              MoneyOfBreakfast = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(breakfastId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber * x.rulePrice),
                              NumberOfLunch = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(lunchId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              MoneyOfLunch = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(lunchId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber * x.rulePrice),
                              NumberOfDinner = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(dinnerId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              MoneyOfDinner = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(dinnerId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber * x.rulePrice),
                              NumberOfMidnightsnack = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(snakeId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              MoneyOfMidnightsnack = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(snakeId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber * x.rulePrice),
                              OtherBreakfastCount = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(breakfastId) && x.type.Trim().Equals("A100002")).Count(),
                              OtherLunchCount = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(lunchId) && x.type.Trim().Equals("A100002")).Count(),
                              OtherDinnerCount = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(dinnerId) && x.type.Trim().Equals("A100002")).Count(),
                              OtherSnackCount = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(snakeId) && x.type.Trim().Equals("A100002")).Count(),
                              OtherBreakfastMoney = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(breakfastId) && x.type.Trim().Equals("A100002")).Sum(x => x.useMoney),
                              OtherLunchMoney = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(lunchId) && x.type.Trim().Equals("A100002")).Sum(x => x.useMoney),
                              OtherDinnerMoney = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(dinnerId) && x.type.Trim().Equals("A100002")).Sum(x => x.useMoney),
                              OtherSnackMoney = records.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(snakeId) && x.type.Trim().Equals("A100002")).Sum(x => x.useMoney),
                              TotalNumber = 0,
                              TotalMoney = 0
                          };
            var result = await PaginatedList<PersonPayoffTableView>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return result;
        }


        public async Task<PaginatedList<TopUpView>> PersonTopUpTable(string timestart, string timeend, string companyid, string employeename, int page, int pageSize)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            // DateTime dt = DateTime.Now;
            // //本月第一天时间      
            // DateTime dt_First = dt.AddDays(1 - (dt.Day));
            // //获得某年某月的天数    
            // int year = dt.Date.Year;
            // int month = dt.Date.Month;
            // int dayCount = DateTime.DaysInMonth(year, month);
            // //本月最后一天时间    
            // DateTime dt_Last = dt_First.AddDays(dayCount - 1);
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(employeename)).OrderBy(y => y.Sort);
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            var countToMoney = Convert.ToInt32(Context.Set<TbDictionary>().Where(x => x.DictionaryCode.Trim() == "A300001").FirstOrDefault().DictionaryName);
            //breakfast 
            var breakfastId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("早餐")).FirstOrDefault().Id;
            //lunch
            var lunchId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("午餐")).FirstOrDefault().Id;
            //dinner
            var dinnerId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("晚餐")).FirstOrDefault().Id;
            //snake
            var snakeId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("夜宵")).FirstOrDefault().Id;
            var dbGive = (from emp in employees
                          join give in Context.Vw_GiveRecords
                          on emp.JobId.Trim() equals give.JobId.Trim()
                          where start <= give.AddAt && give.AddAt <= end
                          select give).ToList();
            var dbRecord = (from emp in employees
                            join ur in Context.Vw_UseRecords
                            on emp.JobId.Trim() equals ur.JobId.Trim()
                            from rule in Context.Set<Rules>()
                            where start <= ur.AddAt && ur.AddAt <= end
                            where Method.isLegalTime(Convert.ToDateTime(ur.AddAt), rule.StartTime + "-" + rule.EndTime)
                            select new
                            {
                                recordId = ur.UseRecordId,
                                useNumber = ur.UseNumber,
                                ruleId = rule.Id,
                                ruleName = rule.Name,
                                jobId = emp.JobId,
                                type = ur.GiveCountType
                            }).ToList();

            var dbitems = from emp in employees
                          join company in cmps
                          on emp.CompanyId.Trim() equals company.Id.Trim()
                          orderby company.Sort ascending
                          orderby emp.Sort ascending
                          select new TopUpView
                          {
                              DepartmentName = company.Name.Trim(),
                              CompanyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              EmployeeName = emp.Name,
                              LastMonthCarryNumber = dbRecord.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.type.Trim().Equals("A100003")).Sum(x => x.useNumber),
                              LastMonthCarryMoney = dbGive.Where(x => x.JobId.Trim().Equals(emp.JobId.Trim()) && x.GiveMoneyOrNumType.Trim().Equals("A500001")).Sum(x => x.GiveMoneyOrNum),
                              TopUpNumber = dbGive.Where(x => x.JobId.Trim().Equals(emp.JobId.Trim()) && x.GiveMoneyOrNumType.Trim().Equals("A400003")).Sum(x => x.GiveMoneyOrNum),
                              SubsidiesMoney = 0, //dbGive.Where(x => x.JobId.Trim().Equals(emp.JobId.Trim()) && x.GiveMoneyOrNumType.Trim().Equals("A500002")).Sum(x => x.GiveMoneyOrNum),
                              DeductionsBreakfast = dbRecord.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(breakfastId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              DeductionsLunch = dbRecord.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(lunchId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              DeductionsDinner = dbRecord.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.ruleId.Equals(dinnerId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              DeductionsSnack = dbRecord.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) &&  x.ruleId.Equals(snakeId) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              TotalDeductions = dbRecord.Where(x => x.jobId.Trim().Equals(emp.JobId.Trim()) && x.type.Trim().Equals("A100001")).Sum(x => x.useNumber),
                              ThisMonthCarryNumber = emp.AvailableCount,
                              ThisMonthCarryMoney = emp.AvailableCount * countToMoney

                          };
            var result = await PaginatedList<TopUpView>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return result;
        }
    }
}