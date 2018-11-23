using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace AzureSearch.Infrastructure.Models
{
    /*
     {
        "id": 1,
        "first_name": "Deidre",
        "last_name": "Arthur",
        "email": "darthur0@rakuten.co.jp",
        "gender": "Female",
        "ip_address": "9.155.1.239"
     },
    */

    // The SerializePropertyNamesAsCamelCase attribute is defined in the Azure Search .NET SDK.
    // It ensures that Pascal-case property names in the model class are mapped to camel-case
    // field names in the index.
    [SerializePropertyNamesAsCamelCase]
    public partial class People
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        [JsonProperty("id")]
        public string Id { get; set; }

        [IsSearchable]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [IsSearchable, IsFilterable, IsSortable]
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [IsSearchable]
        public string Email { get; set; }

        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Gender { get; set; }

        [IsSearchable]
        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }
    }
}
