using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCM.IRepositories;
using BCM.Models.BCM;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BCM.Repositories
{
    public class DictionaryRepository : BaseRepository<TbDictionary>, IDictionaryRepository
    {
        public DictionaryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override IQueryable<TbDictionary> All => Context.Set<TbDictionary>().OrderBy(x => x.DictionaryOrderNum);
        public override void Update(TbDictionary entity)
        {
            EntityEntry<TbDictionary> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            dbEntityEntry.Property(x => x.DictionaryId).IsModified = false;
            if (string.IsNullOrEmpty(entity.DictionaryName))
            {
                dbEntityEntry.Property(x => x.DictionaryName).IsModified = false;
            }

            dbEntityEntry.Property(x => x.DictionaryCode).IsModified = false;


            dbEntityEntry.Property(x => x.ParentDictionaryCode).IsModified = false;


            if (string.IsNullOrEmpty(entity.DictionaryOrderNum.ToString()))
            {
                dbEntityEntry.Property(x => x.DictionaryOrderNum).IsModified = false;
            }

            if (string.IsNullOrEmpty(entity.DictionaryRemarks))
            {
                dbEntityEntry.Property(x => x.DictionaryRemarks).IsModified = false;
            }

        }
        public override TbDictionary GetSingle(string id)
        {
            return Context.Set<TbDictionary>().FirstOrDefault(x => x.DictionaryId == id);
        }
        public override async Task<TbDictionary> GetSingleAsync(string id)
        {
            return await Context.Set<TbDictionary>().FirstOrDefaultAsync(x => x.DictionaryId == id);
        }

        public int OrderNum(string parentcode)
        {
            return Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == parentcode).Count() + 1;
        }

        public async Task<int> OrderNumAsync(string parentcode)
        {
            return await Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == parentcode).CountAsync() + 1;
        }
        public override void Add(TbDictionary entity)
        {
            if (string.IsNullOrEmpty(entity.DictionaryOrderNum.ToString()))
            {

                entity.DictionaryOrderNum = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == entity.ParentDictionaryCode).Count();

            }
            Context.Set<TbDictionary>().Add(entity);
        }
        public IQueryable<object> GetDictionaryTree()
        {
            // var DbParentDic = Context.Set<TbDictionary>().Where(x => x.ParentDictionaryCode == "0");
            var dictionaryTree = from dic1 in Context.Set<TbDictionary>()
                                 where dic1.ParentDictionaryCode.Trim() == "0"
                                 orderby dic1.DictionaryOrderNum ascending
                                 select new
                                 {
                                     dicitonaryId = dic1.DictionaryId.Trim(),
                                     dicitonaryName = dic1.DictionaryName.Trim(),
                                     dictionaryCode = dic1.DictionaryCode.Trim(),
                                     parentDictionaryCode = dic1.ParentDictionaryCode.Trim(),
                                     dictionaryRemarks = dic1.DictionaryRemarks.Trim(),
                                     dictionaryOrderNum = dic1.DictionaryOrderNum,
                                     children = from dic2 in Context.Set<TbDictionary>()
                                                where dic2.ParentDictionaryCode.Trim() == dic1.DictionaryCode.Trim()
                                                orderby dic2.DictionaryOrderNum ascending
                                                select new
                                                {
                                                    dicitonaryId = dic2.DictionaryId.Trim(),
                                                    dicitonaryName = dic2.DictionaryName.Trim(),
                                                    dictionaryCode = dic2.DictionaryCode.Trim(),
                                                    parentDictionaryCode = dic2.ParentDictionaryCode.Trim(),
                                                    dictionaryRemarks = dic2.DictionaryRemarks.Trim(),
                                                    dictionaryOrderNum = dic2.DictionaryOrderNum
                                                }
                                 };
            return dictionaryTree;
        }
        public IQueryable<object> GetNumberLevel()
        {
            var items = from item in Context.Set<TbDictionary>()
                        where item.ParentDictionaryCode == "A2000"
                        orderby item.DictionaryOrderNum ascending
                        select new
                        {
                            dictionaryId = item.DictionaryId,
                            dictionaryName = item.DictionaryName,
                            dicitonaryCode = item.DictionaryCode
                        };
            return items;
        }

        public IQueryable<object> GetUsageType()
        {
            var items = from item in Context.Set<TbDictionary>()
                        where item.ParentDictionaryCode == "A1000"
                        orderby item.DictionaryOrderNum ascending
                        select new
                        {
                            dictionaryId = item.DictionaryId,
                            dictionaryName = item.DictionaryName,
                            dicitonaryCode = item.DictionaryCode
                        };
            return items;
        }

        public IQueryable<object> GetConsumptionType()
        {
            var items = from item in Context.Set<TbDictionary>()
                        where item.ParentDictionaryCode == "A6000"
                        orderby item.DictionaryOrderNum ascending
                        select new
                        {
                            dictionaryId = item.DictionaryId,
                            dictionaryName = item.DictionaryName,
                            dicitonaryCode = item.DictionaryCode
                        };
            return items;
        }

        public IQueryable<object> GetGiveNumberType()
        {
            var items = from item in Context.Set<TbDictionary>()
                        where item.ParentDictionaryCode == "A4000"
                        orderby item.DictionaryOrderNum ascending
                        select new
                        {
                            dictionaryId = item.DictionaryId,
                            dictionaryName = item.DictionaryName,
                            dicitonaryCode = item.DictionaryCode
                        };
            return items;
        }

        public IQueryable<object> GetGiveMoneyType()
        {
            var items = from item in Context.Set<TbDictionary>()
                        where item.ParentDictionaryCode == "A5000"
                        orderby item.DictionaryOrderNum ascending
                        select new
                        {
                            dictionaryId = item.DictionaryId,
                            dictionaryName = item.DictionaryName,
                            dicitonaryCode = item.DictionaryCode
                        };
            return items;
        }

        // public IQueryable<TbDictionary> GetAll()
        // {
        //     var items = Context.Set<TbDictionary>().OrderBy(x=>x.DictionaryCode);
        //     return items;
        // }
    }
}