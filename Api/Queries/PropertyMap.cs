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
    public class PropertyMap
    {
        public class Query : DataRequest<MapItem>
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Radius { get; set; }
        }

        public class MapItem
        {
            public int PropertyId { get; set; }
            public string OwnerName { get; set; }
            public string RealtorName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int NumVisits { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Property, MapItem>()
                    .ForMember(dest => dest.NumVisits,
                        opt => opt.MapFrom(origin => origin.Visits.Count()));
            }

            public class QueryHandler : DataQueryHandler<Query, MapItem, Property>
            {
                public QueryHandler(IRepository<Property> repository, IMapper mapper) : base(repository, mapper) { }

                public async override Task<DataResult<MapItem>> Handle(Query request, CancellationToken token)
                {
                    return await GetData(request);
                }

                public override SortParameter<Property> DefaultSort
                {
                    get
                    {
                        return new SortParameter<Property>
                        {
                            Property = e => e.PropertyId,
                            SortDirection = SortDirection.Ascending
                        };
                    }
                }

                protected override IQueryable<Property> ApplyWhereClause(IQueryable<Property> query, Query filter)
                {
                    query = query.Where(x => x.Latitude < filter.Latitude + filter.Radius &&
                                            x.Latitude > filter.Latitude - filter.Radius &&
                                            x.Longitude < filter.Longitude + filter.Radius &&
                                            x.Longitude > filter.Longitude - filter.Radius);

                    return query;
                }
            }
        }
    }
}