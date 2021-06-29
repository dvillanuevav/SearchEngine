using MediatR;
using Newtonsoft.Json;
using SearchEngine.Autocomplete.Application.Extensions;
using SearchEngine.Autocomplete.Application.Interfaces;
using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Domain;
using SearchEngine.Autocomplete.Domain.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Application.Commands
{
    public class IndexRealEstateEntitiesCommandHandler : IRequestHandler<IndexRealEstateEntitiesCommand, bool>
    {
        private const string FilesPath = "Data";

        private const int MaxBatch = 1000;

        private readonly IRealEstateEntityService _realEstateEntityService;

        public IndexRealEstateEntitiesCommandHandler(IRealEstateEntityService realEstateEntityService)
        {
            _realEstateEntityService = realEstateEntityService;
        }

        public async Task<bool> Handle(IndexRealEstateEntitiesCommand request, CancellationToken cancellationToken)
        {
            bool response = await _realEstateEntityService.CreateIndexAsync();

            if (response)
            {
                var files = Directory.GetFiles($"{FilesPath}").ToList();

                List<Task> bulkIndexTasks = new List<Task>();

                foreach (string filePath in files)
                {
                    foreach (var batches in LoadDataFromFile(filePath).Take(request.MaxItems).Batch(MaxBatch))
                    {
                        bulkIndexTasks.Add(_realEstateEntityService.BulkIndexAsync(batches));
                    }
                };

                await Task.WhenAll(bulkIndexTasks);
            }

            return response;
        }

        private IEnumerable<RealEstateEntity> LoadDataFromFile(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
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