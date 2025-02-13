using CsvHelper.Configuration.Attributes;

class TextTable
{
    [Name("location")]
    public string Location { get; set; } = "";
    [Name("target")]
    public string Localized { get; set; } = "";
}
