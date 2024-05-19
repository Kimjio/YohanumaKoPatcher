using System.Text.Json.Serialization;
using AddressablesTools.Catalog;

class CatalogPatchData
{
    [JsonPropertyName("buckets_append")]
    public Dictionary<string, List<string>>? BucketsAppend { get; set; }
    
    [JsonPropertyName("entries_replace")]
    public List<ResourceLocation>? EntriesReplace { get; set; }
}
