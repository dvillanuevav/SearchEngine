using Nest;
using Newtonsoft.Json;
using SearchEngine.Autocomplete.Domain;
using SearchEngine.Autocomplete.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Api.Utils
{
    public class ElasticIndexService
    {
        private const string IndexName = "real-estate-entities";

        private const string FilesPath = "Data";

        private const string PropertiesFileName = "properties.json";

        private const string ManagementFileName = "mgmt.json";

        private const int MaxBatch = 1000;

        private readonly IElasticClient _elasticClient;

        public ElasticIndexService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task CreateIndexAsync(int maxItems)
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
                               .Filters("lowercase")
                           )
                            .Custom("autocomplete_search", ca => ca                               
                               .Tokenizer("lowercase")
                               .Filters("english_stop")
                           )
                       )
                       .Tokenizers(to => to
                           .EdgeNGram("edge_ngram_tokenizer", ng => 
                           ng.MaxGram(30)
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

            await BulkIndexAsync($"{FilesPath}/{ManagementFileName}", maxItems);
            await BulkIndexAsync($"{FilesPath}/{PropertiesFileName}", maxItems);            
        }

        public async Task DeleteIndexAsync()
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(IndexName);

            if (existsResponse.Exists)
            {
                await _elasticClient.Indices.DeleteAsync(IndexName);
            }
        }

            private async Task BulkIndexAsync(string inputUrl, int maxItems)
        {                    
            foreach (var batches in LoadDataFromFile(inputUrl).Take(maxItems).Batch(MaxBatch))
            {                                
                await _elasticClient.IndexManyAsync<RealEstateEntity>(batches, IndexName);                
            }
        }

        private IEnumerable<RealEstateEntity> LoadDataFromFile(string inputUrl)
        {            
            using (StreamReader sr = new StreamReader(inputUrl))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                reader.SupportMultipleContent = true;
                
                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {                           
                        var rootEntity = serializer.Deserialize<RootModel>(reader);

                        if (rootEntity.Item == null)
                            continue;

                        yield return new RealEstateEntity
                        {
                            Code = rootEntity.Item.Id,
                            City = rootEntity.Item.City,
                            FormerName = rootEntity.Item.FormerName,
                            Market = rootEntity.Item.Market,
                            Name = rootEntity.Item.Name,
                            State = rootEntity.Item.State,
                            StreetAddress = rootEntity.Item.StreetAddress,
                            Type = rootEntity.IsManagement ? (byte)EntityTypeEnum.Management : (byte)EntityTypeEnum.Property
                        };
                    }
                }
            }
        }
    }   
}
