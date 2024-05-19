using System.Text.Json;
using AddressablesTools.Catalog;

partial class PatchWorks
{
    public void PatchCatalog()
    {
        Console.WriteLine("Patching Addressables catalog");
        if (File.Exists(Path.Combine(addressablePath, "catalog_ko.bundle")))
        {
            Console.WriteLine("Already patched. Skipping.");
            return;
        }
        var catalogPatcher = new CatalogPatcher(Path.Combine(addressablePath, "catalog.bundle"));

        using StreamReader f = new(Path.Combine(patchResourcesPath, "catalogpatch.json"));
        var patchData = JsonSerializer.Deserialize<CatalogPatchData>(f.ReadToEnd())!;

        // patch locations
        foreach (var newLocation in patchData.EntriesReplace!)
        {
            if (newLocation.Data != null)
                newLocation.Data = JsonSerializer.Deserialize<ClassJsonObject>((JsonElement)newLocation.Data);
            if (newLocation.Dependency != null)
            {
                JsonElement dependency = (JsonElement)newLocation.Dependency;
                newLocation.Dependency = dependency.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.Number => dependency.GetInt32(),
                    JsonValueKind.String => dependency.GetString(),
                    _ => throw new NotImplementedException(),
                };
            }
            catalogPatcher.AddLocation(newLocation);
        }

        // patch buckets
        foreach (var (k, v) in patchData.BucketsAppend!)
        {
            object key = k.StartsWith('#') ? int.Parse(k[1..]) : k;
            catalogPatcher.AddBucket(key, v);
        }

        catalogPatcher.WriteCatalog(Path.Combine(addressablePath, "catalog_ko.bundle"));
    }
}