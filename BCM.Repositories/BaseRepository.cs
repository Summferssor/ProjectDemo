﻿using BCM.IRepositories;
using BCM.Models.BCM.DbModel;
using BCM.Models.BCM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BCM.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, new()
    {
        #region Properties
        protected BCMContext Context { get; }

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            Context = unitOfWork as BCMContext;
        }
        #endregion
        public virtual IQueryable<T> All => Context.Set<T>();
        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
        public virtual int Count()
        {
            return Context.Set<T>().Count();
        }
        public async Task<int> CountAsync()
        {
            return await Context.Set<T>().CountAsync();
        }


        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().FirstOrDefault(predicate);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(predicate).FirstOrDefault();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate).FirstOrDefaultAsync(); 
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate);
        }

        public virtual void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }
        public virtual void Update(T entity)
        {
            EntityEntry<T> dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
            //dbEntityEntry.Property(x => x.Id).IsModified = false;
        }
        public virtual void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

         public virtual void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Context.Set<T>().Remove(entity);
            }
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = Context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                Context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }
        public void Attach(T entity)
        {
            Context.Set<T>().Attach(entity);
        }

        public void AttachRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Attach(entity);
            }
        }

        public void Detach(T entity)
        {
            Context.Entry<T>(entity).State = EntityState.Detached;
        }

        public void DetachRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Detach(entity);
            }
        }

        public void AttachAsModified(T entity)
        {
            Attach(entity);
            Update(entity);
        }

        public virtual T GetSingle(string id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> GetSingleAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
