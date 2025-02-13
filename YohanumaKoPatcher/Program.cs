using System.Security.Cryptography;
using System.Text.Json;

if (args.Length < 1 || args.Length > 2)
{
    Console.WriteLine("Usage: YohanumaKoPatcher [--export] <GAMEPATH>");
    return;
}

var isExport = args.Length == 2 && args[0] == "--export";

string gamePath;
if (isExport)
{
    gamePath = Path.GetFullPath(args[1]);

}
else
{
    gamePath = Path.GetFullPath(args[0]);
}

if (!Directory.Exists(gamePath))
{
    Console.WriteLine("Cannot find game directory");
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

try
{
    var patcher = new PatchWorks(gamePath, resourcePath);

    if (isExport)
    {
        patcher.DumpMasterDataTable();
        patcher.DumpUnityLocaleTable();
        return;
    }

    patcher.PatchLanguageDropdown();
    patcher.PatchFontFallback();
    patcher.UpdateUnityLocaleTable();
    patcher.UpdateMasterDataTable();
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
