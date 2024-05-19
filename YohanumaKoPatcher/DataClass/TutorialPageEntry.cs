using System.Text.Json.Serialization;

class TutorialPageEntry
{
    public string? TutorialID { get; set; }
    public int Page { get; set; }

    [JsonPropertyName("Laungage")]
    public string? Language { get; set; }
    public string? Text { get; set; }
    public string? ImageAddress { get; set; }

}