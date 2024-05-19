using AddressablesTools.Catalog;
using AssetsTools.NET;

class CatalogPatcher
{
    private ContentCatalogData catalogData;
    private string sourcePath;
    private Dictionary<string, ResourceLocation> locationsMap;

    public CatalogPatcher(string catalogPath)
    {
        catalogData = AddressablesTools.AddressablesJsonParser.FromBundle(catalogPath);
        sourcePath = catalogPath;
        locationsMap = new();

        // PrimaryKey is not a unique identifier. But it works anyway.
        foreach (var loc in catalogData.Locations)
        {
            locationsMap[$"{loc.PrimaryKey}<{loc.Type.ClassName}>"] = loc;
        }
    }

    public void AddLocation(ResourceLocation location)
    {
        var key = $"{location.PrimaryKey}<{location.Type.ClassName}>";
        if (!locationsMap.TryAdd(key, location))
        {
            // change value while keeping object reference
            var oldLocation = locationsMap[key];
            oldLocation.InternalId = location.InternalId;
            oldLocation.ProviderId = location.ProviderId;
            oldLocation.Dependency = location.Dependency;
            oldLocation.DependencyHashCode = location.DependencyHashCode;
            oldLocation.PrimaryKey = location.PrimaryKey;
            oldLocation.Type = location.Type;
            oldLocation.Data = location.Data;
        }
        else
        {
            catalogData.Locations.Add(location);
        }
    }

    public void AddBucket(object key, List<string> locations)
    {
        catalogData.Resources.TryAdd(key, new List<ResourceLocation>());
        foreach (var location in locations)
        {
            catalogData.Resources[key].Add(locationsMap[location]);
        }
    }

    public void WriteCatalog(string catalogPath)
    {
        var memory = new MemoryStream();
        AddressablesTools.AddressablesJsonParser.ToBundle(
            catalogData, File.OpenRead(sourcePath), memory);
        var uncompressed = new AssetBundleFile();
        uncompressed.Read(new AssetsFileReader(memory));
        using (AssetsFileWriter fileWriter = new(catalogPath))
        {
            uncompressed.Pack(fileWriter, AssetBundleCompressionType.LZ4);
        }
        uncompressed.Close();
    }
}
