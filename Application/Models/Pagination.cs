using System.Text.Json.Serialization;

namespace SearchEngine.Autocomplete.Application.Models
{
    public abstract class Pagination
    {        
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 25;               
    }
}
