using SearchEngine.Autocomplete.Application.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SearchEngine.Autocomplete.Api.Models.v1
{
    public class SearchByMarketModel : Pagination
    {
        public SearchByMarketModel()
        {
            this.Markets = new string[] { };
        }

        [JsonPropertyName("keyword")]
        [Required]
        public string Keyword { get; set; }

        [JsonPropertyName("markets")]
        public string[] Markets { get; set; }
    }
}