using MediatR;
using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Domain;
using System.Collections.Generic;

namespace SearchEngine.Autocomplete.Application.Queries
{
    public class SearchRealEstateEntitiesByMarketQuery : Pagination, IRequest<SearchResult<RealEstateEntity>>
    {
        public string Index { get; private set; } = "real-estate-entities";

        public List<string> Markets { get; set; }

        public string Keyword { get; set; }

        public int Offsset
        {
            get
            {
                return (PageIndex - 1) * PageSize;
            }
        }
    }
}