using Api.Core.Interfaces.Repositories;
using Api.Core.Search;
using Api.Data;
using Api.Queries.Shared.DataQueries.Handlers;
using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Queries
{
    public class PropertyList
    {
        public class Query : PagedDataRequest<ListItem>
        {
        }

        public class ListItem
        {
            public int PropertyId { get; set; }
            public string OwnerName { get; set; }
            public string RealtorName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public int NumVisits { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Property, ListItem>()
                    .ForMember(dest => dest.NumVisits,
                        opt => opt.MapFrom(origin => origin.Visits.Count()));
            }

            public class QueryHandler : PagedDataQueryHandler<Query, ListItem, Property>
            {
                public QueryHandler(IRepository<Property> repository, IMapper mapper) : base(repository, mapper) { }

                public async override Task<DataResult<ListItem>> Handle(Query request, CancellationToken token)
                {
                    return await GetPagedData(request);
                }

                public override SortParameter<Property> DefaultSort
                {
                    get
                    {
                        return new SortParameter<Property>
                        {
                            Property = e => e.Address,
                            SortDirection = SortDirection.Ascending
                        };
                    }
                }

                protected override IQueryable<Property> ApplyWhereClause(IQueryable<Property> query, Query filter)
                {
                    //You could do some filtering here if you wanted.
                    //query.Where(x => x.OwnerName == "john");
                    return query;
                }
                protected override IQueryable<Property> ApplySortParameters(IQueryable<Property> query, Query request)
                {
                    var numVisitsSort = request.SortParameters.FirstOrDefault(sort => sort.PropertyName.Equals("NumVisits", StringComparison.OrdinalIgnoreCase));
                    if (numVisitsSort != null)
                    {
                        if (numVisitsSort.SortDirection == SortDirection.Ascending)
                            query = query.OrderBy(x => x.Visits.Count);
                        else
                            query = query.OrderByDescending(x => x.Visits.Count);
                    }
                    else
                    {
                        query = base.ApplySortParameters(query, request);
                    }
                    return query;
                }

            }
        }
    }
}