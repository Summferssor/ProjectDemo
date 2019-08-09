using System;
using System.Linq;
using System.Threading.Tasks;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Common;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BCM.Repositories
{
    public class UseRecordRepository : BaseRepository<UseRecord>, IUseRecordRepository
    {
        public UseRecordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public override void Update(UseRecord entity)
        {
            EntityEntry<UseRecord> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.UseRecordId).IsModified = false;
            dbEntityEntry.Property(x => x.AddAt).IsModified = false;
            if (string.IsNullOrEmpty(entity.GiveCountType))
            {
                dbEntityEntry.Property(x => x.GiveCountType).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.UseMoney.ToString()))
            {
                dbEntityEntry.Property(x => x.UseMoney).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.UseNumber.ToString()))
            {
                dbEntityEntry.Property(x => x.UseNumber).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.Remarks))
            {
                dbEntityEntry.Property(x => x.Remarks).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.JobId))
            {
                dbEntityEntry.Property(x => x.JobId).IsModified = false;
            }

        }
        public override UseRecord GetSingle(string id)
        {
            return Context.Set<UseRecord>().FirstOrDefault(x => x.UseRecordId == id);
        }
        public override async Task<UseRecord> GetSingleAsync(string id)
        {
            return await Context.Set<UseRecord>().FirstOrDefaultAsync(x => x.UseRecordId == id);
        }

        public async Task<PaginatedList<EmployeeRecoreDetail>> RecordDetail(string jobid, string timestart, string timeend, string usagetype, string consumptiontype, int page, int pageSize)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            var dictionary = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "A1000" || x.ParentDictionaryCode == "A5000" || x.ParentDictionaryCode == "A4000");
            var employee = await Context.Set<Employee>().Where(x => x.JobId == jobid).FirstOrDefaultAsync();
            var department = await Context.Set<Company>().Where(x => x.Id == employee.CompanyId).FirstOrDefaultAsync();
            var company = await Context.Set<Company>().Where(x => x.Id == department.ParentId).FirstOrDefaultAsync();
            var givedbitems = from give in Context.Vw_GiveRecords   //Context.Vw_GiveRecords                                                                  //   join use in Context.Set<Vw_UseRecord>()
                              where give.JobId.Trim() == jobid.Trim()
                              where start <= give.AddAt && give.AddAt <= end
                              where string.IsNullOrEmpty(usagetype) || usagetype.Trim() == give.UseType.Trim()                     //A100001次数 A100002钱包
                              where string.IsNullOrEmpty(consumptiontype) || give.GiveType == consumptiontype             //A600001发放 A600002消费
                              orderby give.AddAt descending
                              select new EmployeeRecoreDetail
                              {
                                  CompanyName = company.Name.Trim(),
                                  CompanyId = company.Id.Trim(),
                                  DepartmentName = department.Name.Trim(),
                                  DepartmentId = department.Id.Trim(),
                                  JobId = employee.JobId.Trim(),
                                  EmployeeName = employee.Name.Trim(),
                                  AddAt = give.AddAt,
                                  GiveCountType = dictionary.Where(x => x.DictionaryCode == give.GiveMoneyOrNumType).FirstOrDefault().DictionaryName,
                                  UsageType = give.UseType,
                                  ConsumptionType = dictionary.Where(x => x.DictionaryCode == give.GiveType).FirstOrDefault().DictionaryName,
                                  UseMoney = give.GiveType.Equals("A100002") ? give.GiveMoneyOrNum : 0,
                                  UseNumber = give.GiveType.Equals("A100001") ? give.GiveMoneyOrNum : 0
                              };
            var usedbitems = from use in Context.Vw_UseRecords   //Context.Vw_GiveRecords                                                        //   join use in Context.Set<Vw_UseRecord>()
                             where use.JobId.Trim() == jobid.Trim()
                             where start <= use.AddAt && use.AddAt <= end
                             where string.IsNullOrEmpty(usagetype) || usagetype.Trim() == use.UseType.Trim()                     //A100001次数 A100002钱包
                             where string.IsNullOrEmpty(consumptiontype) || use.GiveCountType == consumptiontype             //A600001发放 A600002消费
                             orderby use.AddAt descending
                             select new EmployeeRecoreDetail
                             {
                                 CompanyName = company.Name.Trim(),
                                 CompanyId = company.Id.Trim(),
                                 DepartmentName = department.Name.Trim(),
                                 DepartmentId = department.Id.Trim(),
                                 JobId = employee.JobId.Trim(),
                                 EmployeeName = employee.Name.Trim(),
                                 AddAt = use.AddAt,
                                 GiveCountType = null,
                                 UsageType = use.UseType,
                                 ConsumptionType = dictionary.Where(x => x.DictionaryCode == use.GiveCountType).FirstOrDefault().DictionaryName,
                                 UseMoney = use.UseMoney,
                                 UseNumber = use.UseNumber
                             };
            var items = givedbitems.Union(usedbitems);
            var Employee = await PaginatedList<EmployeeRecoreDetail>.CreatepagingAsync(items.AsNoTracking(), page, pageSize);

            return Employee;
        }

        public IQueryable<EmployeeRecoreDetail> AllRecordDetail(string jobid, string timestart, string timeend, string usagetype, string consumptiontype)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            var dictionary = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "A1000" || x.ParentDictionaryCode == "A5000" || x.ParentDictionaryCode == "A4000");
            var employee = Context.Set<Employee>().Where(x => x.JobId == jobid).FirstOrDefault();
            var department = Context.Set<Company>().Where(x => x.Id == employee.CompanyId).FirstOrDefault();
            var company = Context.Set<Company>().Where(x => x.Id == department.ParentId).FirstOrDefault();
            var givedbitems = from give in Context.Vw_GiveRecords   //Context.Vw_GiveRecords                                         //   join use in Context.Set<Vw_UseRecord>()
                              where give.JobId.Trim() == jobid.Trim()
                              where start <= give.AddAt && give.AddAt <= end
                              where string.IsNullOrEmpty(usagetype) || usagetype.Trim() == give.UseType.Trim()                     //A100001次数 A100002钱包
                              where string.IsNullOrEmpty(consumptiontype) || give.GiveMoneyOrNumType == consumptiontype             //A600001发放 A600002消费
                              orderby give.AddAt descending
                              select new EmployeeRecoreDetail
                              {
                                  CompanyName = company.Name.Trim(),
                                  CompanyId = company.Id.Trim(),
                                  DepartmentName = department.Name.Trim(),
                                  DepartmentId = department.Id.Trim(),
                                  JobId = employee.JobId.Trim(),
                                  EmployeeName = employee.Name.Trim(),
                                  AddAt = give.AddAt,
                                  GiveCountType = dictionary.Where(x => x.DictionaryCode == give.GiveMoneyOrNumType).FirstOrDefault().DictionaryName,
                                  UsageType = give.UseType,
                                  ConsumptionType = dictionary.Where(x => x.DictionaryCode == give.GiveType).FirstOrDefault().DictionaryName,
                                  UseMoney = give.GiveType.Equals("A100002") ? give.GiveMoneyOrNum : 0,
                                  UseNumber = give.GiveType.Equals("A100001") ? give.GiveMoneyOrNum : 0
                              };
            var usedbitems = from use in Context.Vw_UseRecords   //Context.Vw_GiveRecords                                      //   join use in Context.Set<Vw_UseRecord>()
                             where use.JobId.Trim() == jobid.Trim()
                             where start <= use.AddAt && use.AddAt <= end
                             where string.IsNullOrEmpty(usagetype) || usagetype.Trim() == use.UseType.Trim()                     //A100001次数 A100002钱包
                             where string.IsNullOrEmpty(consumptiontype) || use.GiveCountType == consumptiontype             //A600001发放 A600002消费
                             orderby use.AddAt descending
                             select new EmployeeRecoreDetail
                             {
                                 CompanyName = company.Name.Trim(),
                                 CompanyId = company.Id.Trim(),
                                 DepartmentName = department.Name.Trim(),
                                 DepartmentId = department.Id.Trim(),
                                 JobId = employee.JobId.Trim(),
                                 EmployeeName = employee.Name.Trim(),
                                 AddAt = use.AddAt,
                                 GiveCountType = null,
                                 UsageType = use.UseType,
                                 ConsumptionType = dictionary.Where(x => x.DictionaryCode == use.GiveCountType).FirstOrDefault().DictionaryName,
                                 UseMoney = use.UseMoney,
                                 UseNumber = use.UseNumber
                             };
            var items = givedbitems.Union(usedbitems);
            return items;
        }

        public async Task<PaginatedList<EmployeeConsume>> ConsumeRecord(string jobid, string timestart, string timeend, string empname, string companyid, int page, int pageSize)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            var dictionary = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "A1000");
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(empname) && x.JobId.Contains(jobid));
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
                          join record in Context.Set<UseRecord>()
                          on emp.JobId.Trim() equals record.JobId
                        //   where emp.CompanyId.Trim().Contains(companyid.Trim())
                          where start <= record.AddAt && record.AddAt <= end
                          orderby record.AddAt descending
                          select new EmployeeConsume
                          {
                              JobId = emp.JobId.Trim(),
                              EmployeeName = emp.Name.Trim(),
                              DepartmentName = company.Name.Trim(),
                              DepartmentId = company.Id.Trim(),
                              CompanyId = company.ParentId.Trim(),
                              CompanyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              AddAt = record.AddAt,
                              CountOrMoney = (record.UseNumber + record.UseMoney).ToString(),
                              ConsumeType = dictionary.Where(x => x.DictionaryCode == record.GiveCountType).FirstOrDefault().DictionaryName
                          };
            var Employee = await PaginatedList<EmployeeConsume>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return Employee;
        }

        public IQueryable<EmployeeConsume> AllConsumeRecord(string jobid, string timestart, string timeend, string empname, string companyid)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            var dictionary = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "A1000");
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(empname) && x.JobId.Contains(jobid));
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
                          join record in Context.Set<UseRecord>()
                          on emp.JobId.Trim() equals record.JobId
                        //   where emp.CompanyId.Trim().Contains(conpanyid.Trim())
                          where start <= record.AddAt && record.AddAt <= end
                          orderby record.AddAt descending
                          select new EmployeeConsume
                          {
                              JobId = emp.JobId.Trim(),
                              EmployeeName = emp.Name.Trim(),
                              DepartmentName = company.Name.Trim(),
                              DepartmentId = company.Id.Trim(),
                              CompanyId = company.ParentId.Trim(),
                              CompanyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              AddAt = record.AddAt,
                              CountOrMoney = (record.UseNumber + record.UseMoney).ToString(),
                              ConsumeType = dictionary.Where(x => x.DictionaryCode == record.GiveCountType).FirstOrDefault().DictionaryName
                          };

            return dbitems;
        }

        public IQueryable<object> GetVs()
        {
            return Context.Vw_GiveRecords;
        }

    }
}