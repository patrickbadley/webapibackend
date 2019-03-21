using Api.Core.Interfaces.Repositories;
using Api.Core.Search;
using Api.Data;
using Api.Queries.Shared.DataQueries.Handlers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Queries
{
    public class PropertyDetails
    {
        public class Query : IRequest<ItemDetails>
        {
            public int PropertyId { get; set; }
        }

        public class ItemDetails
        {
            public int PropertyId { get; set; }
            public string OwnerName { get; set; }
            public string RealtorName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public int NumVisits { get; set; }
            public IEnumerable<VisitListItem> Visits { get; set; }
        }

        public class VisitListItem
        {
            public DateTime Date { get; set; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Property, ItemDetails>()
                    .ForMember(dest => dest.NumVisits,
                        opt => opt.MapFrom(origin => origin.Visits.Count()))
                    .ForMember(dest => dest.Visits,
                        opt => opt.MapFrom(origin => origin.Visits.OrderByDescending(x => x.Date)));
                CreateMap<Visit, VisitListItem>();
            }

            public class QueryHandler : IRequestHandler<Query, ItemDetails>
            {
                private readonly IRepository<Property> _repository;
                private readonly IMapper _mapper;

                public QueryHandler(IRepository<Property> repository, IMapper mapper)
                {
                    _repository = repository;
                    _mapper = mapper;
                }

                public async Task<ItemDetails> Handle(Query request, CancellationToken token)
                {
                    return await _repository.Query()
                                            .Where(x => x.PropertyId == request.PropertyId)
                                            .ProjectTo<ItemDetails>(_mapper.ConfigurationProvider)
                                            .FirstOrDefaultAsync();
                }
            }
        }
    }
}