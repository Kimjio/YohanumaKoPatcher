using AssetsTools.NET;
using AssetsTools.NET.Extra;

partial class PatchWorks
{
    const string BUNDLEKEY = "Yohane the Parhelion - NUMAZU in the MIRAGE -";
    private string addressablePath;
    private string bundlePath;
    private string outputPath;
    private string patchResourcesPath;
    private string bundleKey;
    private AssetsManager manager;
    private bool isDemo = false;
    private Dictionary<string, AssetsFileInstance> assetsLookup;
    public PatchWorks(string gameResourcePath, string patchResourcesPath)
    {
        bundleKey = isDemo ? BUNDLEKEY + " Demo" : BUNDLEKEY;
        this.patchResourcesPath = patchResourcesPath;
        addressablePath = Path.Join(gameResourcePath, "StreamingAssets", "aa");
        bundlePath = Path.Combine(addressablePath, "StandaloneWindows64");
        outputPath = Path.Combine(addressablePath, "KoreanLocaleMod");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        manager = new AssetsManager();
        assetsLookup = new();
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
        if (assetsLookup.ContainsKey(bundlePath)) {
            return assetsLookup[bundlePath];
        }
        var fileStream = new FileStream(bundlePath, FileMode.Open);
        BundleFileInstance bundle;
        if (cryptoKey != null)
        {
            var cryptoStream = new BundleDecryptStream(
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
    public static void WriteAndCompressBundle(AssetsFileInstance assets, string outfile)
    {
        var bundle = assets.parentBundle;
        bundle.file.BlockAndDirInfo.DirectoryInfos[0].SetNewData(assets.file);
        var memory = new MemoryStream();
        AssetsFileWriter memWriter = new(memory);
        bundle.file.Write(memWriter);
        memory.Seek(0, SeekOrigin.Begin);

        var uncompressed = new AssetBundleFile();
        uncompressed.Read(new AssetsFileReader(memory));
        using (AssetsFileWriter fileWriter = new(outfile))
        {
            uncompressed.Pack(fileWriter, AssetBundleCompressionType.LZ4);
        }
        uncompressed.Close();
    }
}