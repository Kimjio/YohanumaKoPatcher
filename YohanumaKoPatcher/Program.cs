if (args.Length != 1)
{
    Console.WriteLine("Usage: YohanumaKoPatcher <GAMEPATH>");
    return;
}

var gamePath = args[0];
if (!Directory.Exists(Path.Join(gamePath, "yohanuma_Data")))
{
    Console.WriteLine("Cannot find data folder");
    return;
}

var resourcePath = Path.Join(AppContext.BaseDirectory, "ko-patch-assets");
if (!Directory.Exists(resourcePath))
{
    Console.WriteLine("Cannot find patch data");
    return;
}

var patcher = new PatchWorks(gamePath, resourcePath);
patcher.PatchBinary();
patcher.PatchLanguageDropdown();
patcher.PatchFontFallback();
patcher.UpdateUnityLocaleTable();
patcher.UpdateMasterDataTable();
patcher.PatchCatalog();
patcher.PatchCatalogSettings();
patcher.CopyBundles();
Console.WriteLine("Done!");