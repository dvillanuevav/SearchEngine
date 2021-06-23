using MediatR;
using Microsoft.Extensions.Configuration;
using SearchEngine.Autocomplete.Application.Interfaces;
using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Application.Queries
{
    public class SearchRealEstateEntitiesByMarketQueryHandler : IRequestHandler<SearchRealEstateEntitiesByMarketQuery, SearchResult<RealEstateEntity>>
    {
        private readonly IRealEstateEntityService _realEstateEntityService;           

        public SearchRealEstateEntitiesByMarketQueryHandler(IRealEstateEntityService realEstateEntityService)
        {
            _realEstateEntityService = realEstateEntityService;
        }

        public async Task<SearchResult<RealEstateEntity>> Handle(SearchRealEstateEntitiesByMarketQuery request, CancellationToken cancellationToken)
        {                                    
            return await _realEstateEntityService.SearchByMarketAsync(request);
        }
    }
}
