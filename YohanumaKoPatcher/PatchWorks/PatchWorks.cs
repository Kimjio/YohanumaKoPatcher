using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Globalization;
using CsvHelper;
using System.IO;

partial class PatchWorks
{
    const string BUNDLEKEY = "Yohane the Parhelion - NUMAZU in the MIRAGE -";
    private string gamePath;
    private string gameResourcePath;
    private string addressablePath;
    private string bundlePath;
    private string outputPath;
    private string patchResourcesPath;
    private string? bundleKey = null;
    private AssetsManager manager;
    private bool isNX;
    private bool isSTOVE;
    private Dictionary<string, AssetsFileInstance> assetsLookup;
    public PatchWorks(string gamePath, string patchResourcesPath)
    {
        this.gamePath = gamePath;
        this.patchResourcesPath = patchResourcesPath;

        gameResourcePath = Path.Join(gamePath, "Data");
        isNX = Directory.Exists(gameResourcePath);

        if (!isNX)
        {
            gameResourcePath = Path.Join(gamePath, $"{BUNDLEKEY}_Data");
            isSTOVE = Directory.Exists(gameResourcePath);

            if (!isSTOVE)
            {
                gameResourcePath = Path.Join(gamePath, "yohanuma_Data");
            }
        }

        if (!Directory.Exists(gameResourcePath))
        {
            throw new DirectoryNotFoundException($"Cannot find game data");
        }

        addressablePath = Path.Join(gameResourcePath, "StreamingAssets", "aa");
        bundlePath = Path.Combine(addressablePath, isNX ? "Switch" : "StandaloneWindows64");
        outputPath = Path.Combine(addressablePath, "KoreanLocaleMod");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        manager = new AssetsManager();
        assetsLookup = new();
        Console.WriteLine($"Version {(isNX ? "Switch" : "StandaloneWindows64")}");

        if (!isNX)
        {
            bundleKey = BUNDLEKEY;
        }
    }

    public IEnumerable<AssetFileInfo> GetScriptInfosByClassName(AssetsFileInstance assets, string className)
    {
        var classIdx = AssetHelper.GetAssetsFileScriptInfos(manager, assets)
            .First(kv => kv.Value.ClassName == className).Key;
        return assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            .Where(asset => assets.file.GetScriptIndex(asset) == classIdx);
    }

    public AssetsFileInstance LoadAssets(string bundlePath, string? cryptoKey = null)
    {
        if (assetsLookup.ContainsKey(bundlePath))
        {
            return assetsLookup[bundlePath];
        }
        var fileStream = new FileStream(bundlePath, FileMode.Open);
        BundleFileInstance bundle;
        if (cryptoKey != null)
        {
           var cryptoStream = new BundleStream(
                fileStream, cryptoKey, Path.GetFileNameWithoutExtension(bundlePath));
            bundle = manager.LoadBundleFile(cryptoStream, fileStream.Name);
        }
        else
        {
            bundle = manager.LoadBundleFile(fileStream, fileStream.Name);
        }
        var assets = manager.LoadAssetsFileFromBundle(bundle, 0);
        assetsLookup[bundlePath] = assets;
        return assets;
    }
    public static void WriteAndCompressBundle(AssetsFileInstance assets, string outfile, string? cryptoKey = null)
    {
        var bundle = assets.parentBundle;
        bundle.file.BlockAndDirInfo.DirectoryInfos[0].SetNewData(assets.file);

        using var memory = new MemoryStream();
        AssetsFileWriter memWriter = new(memory);
        bundle.file.Write(memWriter);
        memory.Seek(0, SeekOrigin.Begin);

        var uncompressed = new AssetBundleFile();
        uncompressed.Read(new AssetsFileReader(memory));

        if (cryptoKey != null)
        {
            using var cryptoStream = new BundleStream(
                 new FileStream(outfile, FileMode.Create, FileAccess.Write), cryptoKey, Path.GetFileNameWithoutExtension(outfile));
            using AssetsFileWriter fileWriter = new(cryptoStream);
            uncompressed.Pack(fileWriter, AssetBundleCompressionType.LZMA);
        }
        else
        {
            using AssetsFileWriter fileWriter = new(outfile);
            uncompressed.Pack(fileWriter, AssetBundleCompressionType.LZMA);
        }
        uncompressed.Close();
    }

    static private Dictionary<string, TextTable> ReadCsvToTextTable(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<TextTable>().ToDictionary(e => e.Location, e => e);
    }

    static private void WriteTextTableToCsv(string path, List<TextTable> textTable)
    {
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(textTable);
    }
}
