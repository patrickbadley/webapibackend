using MediatR;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Api.Core.Interfaces.Repositories;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Threading;
using Api.Core.Search;
using AutoMapper;

namespace Api.Queries.Shared.DataQueries.Handlers
{
    public class DataRequest<T> : IRequest<DataResult<T>>
    {
    }

    public class DataResult<T>
    {
        public IEnumerable<T> Data { get; set; }
    }

    public abstract class DataQueryHandler<TRequest, TResult, TDbEntity> : IRequestHandler<TRequest, DataResult<TResult>>
        where TRequest : DataRequest<TResult>
        where TDbEntity : class
    {
        protected IRepository<TDbEntity> Repository;
        private readonly IMapper _mapper;

        public abstract SortParameter<TDbEntity> DefaultSort { get; }

        public DataQueryHandler(IRepository<TDbEntity> repository, IMapper mapper)
        {
            Repository = repository;
            _mapper = mapper;
        }

        public abstract Task<DataResult<TResult>> Handle(TRequest request, CancellationToken cancellationToken);

        protected virtual IQueryable<TDbEntity> GetQuery(TRequest request)
        {
            var query = Repository.Query();

            // Apply Where Clause
            query = ApplyWhereClause(query, request);

            // Apply Sort Parameters
            query = ApplySortParameters(query, new List<SortParameter<TDbEntity>>());

            return query;
        }

        protected async Task<DataResult<TResult>> GetData(TRequest request)
        {
            var query = GetQuery(request);

            return new DataResult<TResult>
            {
                Data = await query.ProjectTo<TResult>(_mapper.ConfigurationProvider).ToListAsync()
            };
        }

        protected virtual IQueryable<TDbEntity> ApplySortParameters(IQueryable<TDbEntity> query, IList<SortParameter<TDbEntity>> sortParameters)
        {
            if (!sortParameters.Any())
            {
                sortParameters.Add(DefaultSort);
            }

            var index = 0;
            foreach (var sp in sortParameters)
            {
                var typeParams = new[] { Expression.Parameter(typeof(TDbEntity), "") };
                var parameterName = sp.Property.Parameters[0].Name;
                var propertyName = sp.Property.Body is UnaryExpression
                                       ? ((UnaryExpression)sp.Property.Body).Operand.ToString()
                                       : sp.Property.Body.ToString();
                var pi = typeof(TDbEntity).GetProperty(propertyName.Replace(parameterName + ".", ""));


                query = (IQueryable<TDbEntity>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        GetSortMethodName(index, sp.SortDirection),
                        new Type[] { typeof(TDbEntity), pi.PropertyType },
                        query.Expression,
                        Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
                );

                index++;
            }
            return query;
        }

        protected abstract IQueryable<TDbEntity> ApplyWhereClause(IQueryable<TDbEntity> query, TRequest filter);

        private static string GetSortMethodName(int index, SortDirection sortDirection)
        {
            if (index == 0)
            {
                return sortDirection == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
            }

            return sortDirection == SortDirection.Ascending ? "ThenBy" : "ThenByDescending";
        }
    }
}