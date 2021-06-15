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

        private readonly IConfiguration _configuration;        

        public SearchRealEstateEntitiesByMarketQueryHandler(IRealEstateEntityService realEstateEntityService, IConfiguration configuration)
        {
            _realEstateEntityService = realEstateEntityService;
            _configuration = configuration;
        }

        public async Task<SearchResult<RealEstateEntity>> Handle(SearchRealEstateEntitiesByMarketQuery request, CancellationToken cancellationToken)
        {                                    
            return await _realEstateEntityService.SearchByMarketAsync(request, cancellationToken);
        }
    }
}
