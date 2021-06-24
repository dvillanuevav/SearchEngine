using Newtonsoft.Json;

namespace SearchEngine.Autocomplete.Application.Models
{
    public class RootModel
    {
        [JsonProperty("mgmt")]
        public ItemModel Management { get; set; }

        [JsonProperty("property")]
        public ItemModel Property { get; set; }

        [JsonIgnore]
        public ItemModel Item
        {
            get
            {
                return Management == null ? Property : Management;
            }
        }

        [JsonIgnore]
        public bool IsManagement
        {
            get
            {
                return Management == null ? false : true;
            }
        }
    }

    public class ItemModel
    {
        [JsonProperty("mgmtID")]
        public int ManagementId { get; set; }

        [JsonProperty("propertyID")]
        public int PropertyId { get; set; }

        [JsonIgnore]
        public int Id
        {
            get
            {
                return ManagementId == default(int) ? PropertyId : ManagementId;
            }
        }

        public string Name { get; set; }

        public string FormerName { get; set; }

        public string Market { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string StreetAddress { get; set; }

        [JsonIgnore]
        public byte Type { get; set; }
    }
}