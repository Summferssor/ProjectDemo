using System;
using System.Linq;
using System.Threading.Tasks;
using BCM.Common;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.BCMModels.ModelView;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BCM.Repositories
{
    public class GiveCountRepository : BaseRepository<GiveCount>, IGiveCountRepository
    {
        public GiveCountRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public override void Update(GiveCount entity)
        {
            EntityEntry<GiveCount> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.GiveRecordId).IsModified = false;
            if (string.IsNullOrEmpty(entity.AddAt.ToString()))
            {
                dbEntityEntry.Property(x => x.AddAt).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.GiveCountType))
            {
                dbEntityEntry.Property(x => x.GiveCountType).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.GiveNumber.ToString()))
            {
                dbEntityEntry.Property(x => x.GiveNumber).IsModified = false;
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
        public override GiveCount GetSingle(string id)
        {
            return Context.Set<GiveCount>().FirstOrDefault(x => x.GiveRecordId == id);
        }
        public override async Task<GiveCount> GetSingleAsync(string id)
        {
            return await Context.Set<GiveCount>().FirstOrDefaultAsync(x => x.GiveRecordId == id);
        }

        public override void Add(GiveCount entity)
        {
            entity.AddAt = DateTime.Now;
            Context.Set<GiveCount>().Add(entity);
        }

        public async Task<PaginatedList<EmployeeCountRecord>> GiveCountRecord(string jobid, string timestart, string timeend, string empname, string companyid, int page, int pageSize)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            var dictionary = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "A4000");
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(empname) && x.JobId.Contains(jobid)).OrderBy(y => y.Sort);
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
                          join givecount in Context.Set<GiveCount>()
                          on emp.JobId.Trim() equals givecount.JobId
                          where start <= givecount.AddAt && givecount.AddAt <= end
                          orderby givecount.AddAt descending
                          select new EmployeeCountRecord
                          {
                              jobId = emp.JobId.Trim(),
                              name = emp.Name.Trim(),
                              departmentName = company.Name.Trim(),
                              departmentId = company.Id.Trim(),
                              conpanyId = company.ParentId.Trim(),
                              companyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              giveTime = givecount.AddAt,
                              giveNumber = givecount.GiveNumber,
                              giveType = dictionary.Where(x => x.DictionaryCode == givecount.GiveCountType).FirstOrDefault().DictionaryName
                          };
            var Employee = await PaginatedList<EmployeeCountRecord>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return Employee;
        }

        public IQueryable<EmployeeCountRecord> AllGiveCountRecord(string jobid, string timestart, string timeend, string empname, string companyid)
        {
            DateTime start = string.IsNullOrEmpty(timestart) ? DateTime.Now.AddDays(-DateTime.Now.DayOfYear + 1) : Method.StampToDateTime(timestart);
            DateTime end = string.IsNullOrEmpty(timeend) ? DateTime.Now.AddYears(1).AddDays(-DateTime.Now.AddYears(1).DayOfYear) : Method.StampToDateTime(timeend);
            var dictionary = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "A4000");
            var employees = Context.Set<Employee>().Where(x => x.Name.Contains(empname) && x.JobId.Contains(jobid)).OrderBy(y => y.Sort);
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
                          join givecount in Context.Set<GiveCount>()
                          on emp.JobId.Trim() equals givecount.JobId
                          //   where emp.CompanyId.Trim().Contains(companyid.Trim())
                          where start <= givecount.AddAt && givecount.AddAt <= end
                          orderby givecount.AddAt descending
                          select new EmployeeCountRecord
                          {
                              jobId = emp.JobId.Trim(),
                              name = emp.Name.Trim(),
                              departmentName = company.Name.Trim(),
                              departmentId = company.Id.Trim(),
                              conpanyId = company.ParentId.Trim(),
                              companyName = Context.Set<Company>().Where(x => x.Id == company.ParentId).FirstOrDefault().Name.Trim(),
                              giveTime = givecount.AddAt,
                              giveNumber = givecount.GiveNumber,
                              giveType = dictionary.Where(x => x.DictionaryCode == givecount.GiveCountType).FirstOrDefault().DictionaryName
                          };
            return dbitems;
        }
    }
}