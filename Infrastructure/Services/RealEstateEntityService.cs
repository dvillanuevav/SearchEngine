using Nest;
using SearchEngine.Autocomplete.Application.Interfaces;
using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Application.Queries;
using SearchEngine.Autocomplete.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SearchEngine.Autocomplete.Infrastructure.Services
{
    public class RealEstateEntityService : IRealEstateEntityService
    {
        readonly IElasticClient _elasticClient;

        public RealEstateEntityService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<SearchResult<RealEstateEntity>> SearchByMarketAsync(SearchRealEstateEntitiesByMarketQuery request, CancellationToken cancellationToken)
        {
            var filters = new List<Func<QueryContainerDescriptor<RealEstateEntity>, QueryContainer>>();

            if (request.Markets.Any())
            {
                filters.Add(fq => fq.Terms(t => t.Field(f => f.Market).Terms(request.Markets)));
            }

            ISearchResponse<RealEstateEntity> result = await _elasticClient.SearchAsync<RealEstateEntity>(s => s
                                                            .Index(request.Index)
                                                            .Query(q => q
                                                                .MultiMatch(mp => mp
                                                                    .Query(request.Keyword)
                                                                    .Operator(Operator.And)
                                                                        .Fields(f => f
                                                                            .Fields(f1 => f1.Name, f2 => f2.FormerName, f3 => f3.State, f4 => f4.City, f5 => f5.StreetAddress))                                                                        
                                                                        .Fuzziness(Fuzziness.Auto)
                                                                ) && q
                                                                .Bool(bq => bq.Filter(filters))
                                                            )                                                            
                                                            .From(request.Offsset)
                                                            .Size(request.PageSize), 
                                                            cancellationToken);

            return new SearchResult<RealEstateEntity>(result.Documents, result.Total, request.PageIndex, request.PageSize);            
        }
    }
}
