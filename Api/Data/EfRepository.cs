using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Api.Core.Interfaces.Repositories;

namespace Api.Infrastructure.Data
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        private readonly ILogger<EfRepository<TEntity>> _logger;

        protected DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

        public EfRepository(DbContext context, ILogger<EfRepository<TEntity>> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] eagerIncludes)
        {
            var query = DbSet.AsQueryable();

            if (eagerIncludes.Any())
                query = eagerIncludes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query;
        }


        public virtual async Task<TEntity> Find(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity> Create(TEntity entity)
        {

            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task Delete(int id)
        {
            try
            {
                var entity = await Find(id);
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger?.LogError("Caught exception deleting Entity.  Rethrowing UnableToDeleteException for proper handling.", e);
                throw;
            }
        }

        public virtual async Task Delete(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger?.LogError("Caught exception deleting Entity.  Rethrowing UnableToDeleteException for proper handling.", e);
                throw;
            }
        }
    }
}