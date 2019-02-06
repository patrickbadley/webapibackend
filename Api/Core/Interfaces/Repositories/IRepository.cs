using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api.Core.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] eagerIncludes);
        Task<TEntity> Find(int id);
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task Delete(int id);
        Task Delete(TEntity entity);
    }
}