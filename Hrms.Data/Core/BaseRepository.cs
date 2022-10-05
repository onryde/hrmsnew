using System;
using System.Collections.Generic;
using System.Linq;
using Hrms.Data.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Data.Core
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected HrmsContext dbContext
        {
            get;
            private set;
        }

        public BaseRepository(HrmsContext context)
        {
            this.dbContext = (HrmsContext)(context ?? throw new ArgumentNullException("context", "The context cannot be null."));
        }

        public T Get(int id)
        {
            return dbContext.Set<T>().Find(id);
        }

        public void Remove(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public virtual void Dispose()
        {
            if (this.dbContext != null)
                this.dbContext.Dispose();
            this.dbContext = null;
        }

        public T Get(long id)
        {
            return dbContext.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return dbContext.Set<T>();
        }

        public void Remove(IEnumerable<T> entity)
        {
            dbContext.Set<T>().RemoveRange(entity);
        }

        public void AddItem(T item)
        {
            dbContext.Set<T>().Add(item);
        }

        public void AddItemRange(IEnumerable<T> items)
        {
            dbContext.Set<T>().AddRange(items);
        }
    }
}
