using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Application.Queries;
using SearchEngine.Autocomplete.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Application.Interfaces
{
    public interface IRealEstateEntityService
    {
        Task<SearchResult<RealEstateEntity>> SearchByMarketAsync(SearchRealEstateEntitiesByMarketQuery request);

        Task<bool> CreateIndexAsync();

        Task<bool> DeleteIndex();

        Task BulkIndexAsync(IEnumerable<RealEstateEntity> documents);
    }
}