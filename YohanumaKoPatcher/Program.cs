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
    WaitInput();
    return;
}

var resourcePath = Path.Join(AppContext.BaseDirectory, "ko-patch-assets");
if (!Directory.Exists(resourcePath))
{
    Console.WriteLine("Cannot find patch data");
    WaitInput();
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
    WaitInput();
    return;
}
try
{
    var patcher = new PatchWorks(gamePath, resourcePath, versionMap[hash]);
    patcher.PatchBinary();
    patcher.PatchLanguageDropdown();
    patcher.PatchFontFallback();
    patcher.UpdateUnityLocaleTable();
    patcher.UpdateMasterDataTable();
    patcher.PatchCatalog();
    patcher.PatchCatalogSettings();
    patcher.CopyBundles();
}
catch (Exception e)
{
    Console.WriteLine($"Patch failed: {e}");
    WaitInput();
    return;
}

Console.WriteLine("Done!");
WaitInput();

static void WaitInput()
{
#if !DEBUG
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey(true);
#endif
}