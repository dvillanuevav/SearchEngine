using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Application.Queries;
using SearchEngine.Autocomplete.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Application.Interfaces
{
    public interface IRealEstateEntityService
    {
        Task<SearchResult<RealEstateEntity>> SearchByMarketAsync(SearchRealEstateEntitiesByMarketQuery request, CancellationToken cancellationToken);       
    }
}
