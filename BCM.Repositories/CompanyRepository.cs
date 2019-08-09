using System;
using System.Collections.Generic;
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
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public override void Update(Company entity)
        {
            EntityEntry<Company> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.Id).IsModified = false;
            if (string.IsNullOrEmpty(entity.Name))
            {
                dbEntityEntry.Property(x => x.Name).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.CountOrNot))
            {
                dbEntityEntry.Property(x => x.CountOrNot).IsModified = false;
            }
            if (string.IsNullOrEmpty(entity.ParentId))
            {
                dbEntityEntry.Property(x => x.ParentId).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.Remarks))
            {
                dbEntityEntry.Property(x => x.Remarks).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.Sort.ToString()))
            {
                dbEntityEntry.Property(x => x.Sort).IsModified = false;
            }
        }
        public override Company GetSingle(string id)
        {
            return Context.Set<Company>().FirstOrDefault(x => x.Id == id);
        }
        public override async Task<Company> GetSingleAsync(string id)
        {
            return await Context.Set<Company>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaginatedList<Company>> Page(string searchString, int page, int pageSize)
        {
            var companys = Context.Set<Company>().Where(x => x.Name.Contains(searchString) && x.ParentId == "0").OrderBy(y => y.Sort);
            var company = await PaginatedList<Company>.CreatepagingAsync(companys.AsNoTracking(), page, pageSize);
            return company;
        }

        public async Task<PaginatedList<Company>> DepartmentPage(string searchString, int page, int pageSize, string parentid)
        {
            var companys = Context.Set<Company>().Where(x => x.Name.Contains(searchString) && x.ParentId == parentid).OrderBy(y => y.Sort);
            var company = await PaginatedList<Company>.CreatepagingAsync(companys.AsNoTracking(), page, pageSize);
            return company;
        }

        public IQueryable<object> GetCompanyList()
        {
            var items = from item in Context.Set<Company>()
                        where item.ParentId == "0"
                        orderby item.Sort ascending
                        select new
                        {
                            companyId = item.Id,
                            companyName = item.Name
                        };
            return items;
        }
        public async Task<PaginatedList<SettlementView>> Settlement(string timestart, string timeend, string companyid, int page, int pageSize)
        {
            DateTime start = Method.StampToDateTime(timestart);
            DateTime end = Method.StampToDateTime(timeend);
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            var price = Convert.ToDouble(Context.Set<TbDictionary>().Where(x => x.DictionaryCode.Trim() == "X1000").FirstOrDefault().DictionaryName);
            var dbitems = from cmp in cmps
                          orderby cmp.Sort ascending
                          select new SettlementView
                          {
                              DepartmentName = cmp.Name.Trim(),
                              CompanyName = Context.Set<Company>().Where(x => x.Id == cmp.ParentId).FirstOrDefault().Name.Trim(),
                              TotalPersonNumber = Context.Set<Employee>().Where(x=>x.CompanyId.Trim().Equals(cmp.Id.Trim())).Count(),
                              PriceOfSettlement = price,
                              AmountOfSettlement = Context.Set<Employee>().Where(x=>x.CompanyId.Trim().Equals(cmp.Id.Trim())).Count() * price,
                              ActualSettlementAmount = 0
                          };
            var result = await PaginatedList<SettlementView>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return result;
        }

        public async Task<PaginatedList<CompanyPayoffTableView>> CompanyPayoffTable(string timestart, string timeend, string companyid, int page, int pageSize)
        {
            DateTime start = Method.StampToDateTime(timestart);
            DateTime end = Method.StampToDateTime(timeend);
            IQueryable<Company> cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid));
            if (cmps.FirstOrDefault().ParentId.Trim() == "0" && cmps.Count() == 1)
            {
                cmps = Context.Set<Company>().Where(x => x.ParentId.Equals(companyid));
            }
            else
            {
                cmps = Context.Set<Company>().Where(x => x.Id.Contains(companyid) && x.ParentId.Trim() != "0");
            }
            var records = (from ur in Context.Vw_UseRecords
                           from rule in Context.Set<Rules>()
                           from cmp in cmps
                           join emp in Context.Set<Employee>()
                          on cmp.Id.Trim() equals emp.CompanyId.Trim()
                           where ur.JobId.Trim() == emp.JobId.Trim()
                           where start <= ur.AddAt && ur.AddAt <= end
                           where ur.GiveCountType.Trim() == "A100001"
                           where Method.isLegalTime(Convert.ToDateTime(ur.AddAt), rule.StartTime + "-" + rule.EndTime)
                           select new
                           {
                               recordId = ur.UseRecordId,
                               useNumber = ur.UseNumber,
                               useMoney = ur.UseMoney,
                               ruleId = rule.Id,
                               ruleName = rule.Name,
                               rulePrice = rule.Price,
                               companyId = cmp.Id,
                           }).ToList();

            var breakfastId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("早餐")).FirstOrDefault().Id;
            //lunch
            var lunchId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("午餐")).FirstOrDefault().Id;
            //dinner
            var dinnerId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("晚餐")).FirstOrDefault().Id;
            //snake
            var snakeId = Context.Set<Rules>().Where(x => x.Name.Trim().Equals("夜宵")).FirstOrDefault().Id;
            var dbitems = from cmp in cmps
                          orderby cmp.Sort ascending
                          select new CompanyPayoffTableView
                          {
                              DepartmentName = cmp.Name.Trim(),
                              CompanyName = Context.Set<Company>().Where(x => x.Id == cmp.ParentId).FirstOrDefault().Name.Trim(),
                              NumberOfBreakfast = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(breakfastId)).Sum(x => x.useNumber),
                              BreakfastPrice = Context.Set<Rules>().Where(x => x.Id.Equals(breakfastId)).FirstOrDefault().Price,
                              UseMoneyOfBreakfast = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(breakfastId)).Sum(x => x.useNumber * x.rulePrice),
                              NumberOfLunch = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(lunchId)).Sum(x => x.useNumber),
                              LunchPrice = Context.Set<Rules>().Where(x => x.Id.Equals(lunchId)).FirstOrDefault().Price,
                              UseMoneyOfLunch = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(lunchId)).Sum(x => x.useNumber * x.rulePrice),
                              NumberOfDinner = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(dinnerId)).Sum(x => x.useNumber),
                              DinnerPrice = Context.Set<Rules>().Where(x => x.Id.Equals(dinnerId)).FirstOrDefault().Price,
                              UseMoneyOfDinner = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(dinnerId)).Sum(x => x.useNumber * x.rulePrice),
                              NumberOfMidnightsnack = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(snakeId)).Sum(x => x.useNumber),
                              MidnightsnackPrice = Context.Set<Rules>().Where(x => x.Id.Equals(snakeId)).FirstOrDefault().Price,
                              UseMoneyOfMidnightsnack = records.Where(x => x.companyId.Trim().Equals(cmp.Id.Trim()) && x.ruleId.Equals(snakeId)).Sum(x => x.useNumber * x.rulePrice),
                              AmountPayable = 0,
                              ActualAmount = 0,
                              Remarks = ""

                          };
            var result = await PaginatedList<CompanyPayoffTableView>.CreatepagingAsync(dbitems.AsNoTracking(), page, pageSize);

            return result;
        }
        public IQueryable<object> GetDepartmentList(string parentid)
        {
            var items = from item in Context.Set<Company>()
                        where item.ParentId.Trim() == parentid.Trim()
                        orderby item.Sort ascending
                        select new
                        {
                            departmentId = item.Id,
                            departmentName = item.Name
                        };
            return items;
        }

        public override void Add(Company entity)
        {
            if (string.IsNullOrEmpty(entity.Sort.ToString()))
            {

                entity.Sort = Context.Set<Company>().Where(x => x.ParentId == entity.ParentId).Count();

            }
            Context.Set<Company>().Add(entity);
        }

        public IQueryable<object> GetCompanyTree()
        {
            // var DbParentDic = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "0");
            var CompanyTree = from cmp1 in Context.Set<Company>()
                              where cmp1.ParentId.Trim() == "0"
                              orderby cmp1.Sort ascending
                              select new
                              {
                                  companyId = cmp1.Id.Trim(),
                                  companyName = cmp1.Name.Trim(),
                                  children = from cmp2 in Context.Set<Company>()
                                             where cmp2.ParentId.Trim() == cmp1.Id.Trim()
                                             orderby cmp2.Sort ascending
                                             select new
                                             {
                                                 departmentId = cmp2.Id.Trim(),
                                                 departmentName = cmp2.Name.Trim(),
                                             }
                              };
            return CompanyTree;

        }


    }
}