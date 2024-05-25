using System.Security.Cryptography;
using System.Text.Json;

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

Dictionary<string, GameVersionInfo> versionMap;
using (var f = File.OpenRead(Path.Join(resourcePath, "versioninfo.json")))
{
    versionMap = JsonSerializer.Deserialize<Dictionary<string, GameVersionInfo>>(f)!;
}

string hash;
using (var f = File.OpenRead(Path.Join(gamePath, "GameAssembly.dll")))
using (var sha1 = SHA1.Create())
{
    hash = Convert.ToHexString(sha1.ComputeHash(f)).ToLower();
}

if (!versionMap.ContainsKey(hash))
{
    Console.WriteLine("Unknown game version");
    return;
}

var patcher = new PatchWorks(gamePath, resourcePath, versionMap[hash]);
patcher.PatchBinary();
patcher.PatchLanguageDropdown();
patcher.PatchFontFallback();
patcher.UpdateUnityLocaleTable();
patcher.UpdateMasterDataTable();
patcher.PatchCatalog();
patcher.PatchCatalogSettings();
patcher.CopyBundles();
Console.WriteLine("Done!");