partial class PatchWorks
{
    public void PatchCatalogSettings()
    {
        Console.WriteLine("Patching Addressables catalog settings");
        string settings;
        using (StreamReader reader = new(Path.Combine(addressablePath, "settings.json")))
        {
            settings = reader.ReadToEnd();
        }
        
        if (!settings.Contains("catalog_ko.bundle"))
        {
            using StreamWriter writer = new(Path.Combine(addressablePath, "settings.json"));
            writer.Write(settings.Replace("catalog.bundle", "catalog_ko.bundle"));
        }
    }
}
