using Nest;
using Newtonsoft.Json;
using SearchEngine.Autocomplete.Application.Commands;
using SearchEngine.Autocomplete.Application.Extensions;
using SearchEngine.Autocomplete.Application.Interfaces;
using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Application.Queries;
using SearchEngine.Autocomplete.Domain;
using SearchEngine.Autocomplete.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Infrastructure.Services
{
    public class RealEstateEntityService : IRealEstateEntityService
    {
        private const string IndexName = "real-estate-entities";       

        readonly IElasticClient _elasticClient;

        public RealEstateEntityService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task BulkIndexAsync(IEnumerable<RealEstateEntity> documents)
        {
            await _elasticClient.IndexManyAsync<RealEstateEntity>(documents, IndexName);
        }

        public async Task<bool> CreateIndexAsync()
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(IndexName);

            if (existsResponse.Exists)
            {
                throw new Exception("The index is already created");
            }

            var createIndexDescriptor = new CreateIndexDescriptor(IndexName)
               .Settings(s => s
                   .Analysis(a => a
                       .Analyzers(an => an
                           .Custom("autocomplete", ca => ca
                               .Tokenizer("edge_ngram_tokenizer")
                               .Filters("english_stop", "lowercase")
                           )
                            .Custom("autocomplete_search", ca => ca
                               .Tokenizer("lowercase")
                               .Filters("english_stop")
                           )
                       )
                       .Tokenizers(to => to
                           .EdgeNGram("edge_ngram_tokenizer", ng => ng
                           .MaxGram(30)
                           .MinGram(2)
                           .TokenChars(new[] { TokenChar.Letter, TokenChar.Digit }))
                       )
                       .TokenFilters(tk => tk
                           .Stop("english_stop", sw => sw.IgnoreCase(true).StopWords("_english_"))
                       )
                   )
               )
               .Map<RealEstateEntity>(m => m
                   .Properties(props => props
                       .Text(t => t
                           .Name(p => p.Name)
                           .Analyzer("autocomplete")
                           .SearchAnalyzer("autocomplete_search")
                       )
                       .Text(t => t
                           .Name(p => p.FormerName)
                           .Analyzer("autocomplete")
                           .SearchAnalyzer("autocomplete_search")
                       )
                       .Text(t => t
                           .Name(p => p.State)
                           .Analyzer("autocomplete")
                           .SearchAnalyzer("autocomplete_search")
                       )
                       .Text(t => t
                           .Name(p => p.City)
                           .Analyzer("autocomplete")
                           .SearchAnalyzer("autocomplete_search")
                       )
                       .Text(t => t
                           .Name(p => p.StreetAddress)
                           .Analyzer("autocomplete")
                           .SearchAnalyzer("autocomplete_search")
                       )
                       .Keyword(t => t
                           .Name(p => p.Market)
                       )
                   )
               );


            var createIndexResponse = await _elasticClient.Indices.CreateAsync(createIndexDescriptor);

            if (!createIndexResponse.IsValid)
            {
                throw new Exception(createIndexResponse.DebugInformation);
            }

            return createIndexResponse.IsValid;
        }

        public async Task<bool> DeleteIndex()
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(IndexName);

            if (!existsResponse.Exists)
            {
                return true;
            }

            var deleteIndexResponse = await _elasticClient.Indices.DeleteAsync(IndexName);

            if (!deleteIndexResponse.IsValid)
            {
                throw new Exception(deleteIndexResponse.DebugInformation);
            }

            return deleteIndexResponse.IsValid;            
        }

        public async Task<SearchResult<RealEstateEntity>> SearchByMarketAsync(SearchRealEstateEntitiesByMarketQuery request)
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
                                                                            .Fields(
                                                                                f1 => f1.Name, 
                                                                                f2 => f2.FormerName,
                                                                                f3 => f3.State, 
                                                                                f4 => f4.City, 
                                                                                f5 => f5.StreetAddress))                                                                        
                                                                        .Fuzziness(Fuzziness.Auto)
                                                                ) && q
                                                                .Bool(bq => bq.Filter(filters))
                                                            )                                                            
                                                            .From(request.Offsset)
                                                            .Size(request.PageSize));

            return new SearchResult<RealEstateEntity>(result.Documents, result.Total, request.PageIndex, request.PageSize);            
        }      
    }
}
