using System;
using System.Collections.Generic;
using System.Linq;

namespace Hrms.Data.Core
{
    public interface IBaseRepository<T> : IDisposable
    {
        T Get(long id);

        IQueryable<T> GetAll();

        void AddItem(T item);

        void AddItemRange(IEnumerable<T> items);

        void Remove(T entity);

        void Remove(IEnumerable<T> entity);

        void Save();
    }
}
