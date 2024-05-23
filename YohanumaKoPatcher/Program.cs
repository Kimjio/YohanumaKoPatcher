if (args.Length != 1)
{
    Console.WriteLine("Usage: YohanumaKoPatcher <GAMEPATH>");
    return;
}

var gamePath = Path.GetFullPath(args[0]);
if (!File.Exists(Path.Join(gamePath, "GameAssembly.dll")))
{
    Console.WriteLine("Cannot find game data");
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