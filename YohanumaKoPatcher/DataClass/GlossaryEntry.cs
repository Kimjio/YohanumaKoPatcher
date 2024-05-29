using System.Text.Json.Serialization;

class GlossaryEntry
{
    public string? ID { get; set; }
    public string? MasterID { get; set; }

    [JsonPropertyName("Laungage")]
    public string? Language { get; set; }
    public string? Name { get; set; }
    public string? Text { get; set; }
    public string? Icon { get; set; }

}
