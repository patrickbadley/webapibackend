using Api.Core.Interfaces.Repositories;
using Api.Core.Search;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Api.Queries.Shared.DataQueries.Handlers
{
    public class PagedDataRequest<T> : DataRequest<T>
    {
        public PagedDataRequest()
        {
            SortParameters = new List<SortParameter>();
        }

        public int? Skip { get; set; }
        public int? Top { get; set; }
        public List<SortParameter> SortParameters { get; set; }
    }

    public class PagedDataResult<T> : DataResult<T>
    {
        public long Total { get; set; }
    }


    public abstract class PagedDataQueryHandler<TRequest, TResult, TDbEntity> : DataQueryHandler<TRequest, TResult, TDbEntity>
        where TRequest : PagedDataRequest<TResult>
        where TDbEntity : class
    {
        private readonly IMapper _mapper;

        public PagedDataQueryHandler(IRepository<TDbEntity> repository, IMapper mapper) : base(repository, mapper)
        {
            _mapper = mapper;
        }

        protected async Task<PagedDataResult<TResult>> GetPagedData(TRequest request)
        {
            var query = GetQuery(request);

            // Get count prior to applying paging
            var allResultsCount = query.CountAsync();

            // Apply Sort Parameters
            query = ApplySortParameters(query, request);

            // Apply Paging
            query = ApplyPagingParameters(query, request.Skip, request.Top);

            return new PagedDataResult<TResult>
            {
                Data = await query.ProjectTo<TResult>(_mapper.ConfigurationProvider).ToListAsync(),
                Total = await allResultsCount
            };
        }
        protected virtual IQueryable<TDbEntity> ApplySortParameters(IQueryable<TDbEntity> query, TRequest request)
        {
            if (request.SortParameters.Count != 0)
            {
                string orderByString = request.SortParameters.FirstOrDefault()?.PropertyName + " " + request.SortParameters.FirstOrDefault()?.SortDirection;
                query = query.OrderBy(orderByString);
            }
            return query;
        }

        protected virtual IQueryable<TDbEntity> ApplyPagingParameters(IQueryable<TDbEntity> query, int? startIndex, int? rowsToFetch)
        {
            if (startIndex.HasValue)
            {
                query = query.Skip(startIndex.Value);
            }
            if (rowsToFetch.HasValue)
            {
                query = query.Take(rowsToFetch.Value);
            }

            return query;
        }
    }
}