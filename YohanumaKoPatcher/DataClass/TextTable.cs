using CsvHelper.Configuration.Attributes;

internal class TextTable
{
    [Name("location")]
    public string Location { get; set; } = "";
    [Name("source")]
    public string Source { get; set; } = "";
    [Name("target")]
    public string Localized { get; set; } = "";
}