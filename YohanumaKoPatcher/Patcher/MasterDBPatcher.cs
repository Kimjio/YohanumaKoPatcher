using AssetsTools.NET;
using AssetsTools.NET.Extra;
using MessagePack;

class MasterDBPatcher
{
    private AssetsManager manager;
    private AssetsFileInstance assets;
    private AssetFileInfo dbInfo;
    private AssetTypeValueField dbBase;
    private MessagePackSerializerOptions lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
    public Dictionary<string, string> JsonStrings { get; set; } = new();

    public MasterDBPatcher(string mdbPath, string bundleKey)
    {
        var fileStream = new FileStream(mdbPath, FileMode.Open);
        manager = new AssetsManager();
        var cryptoStream = new BundleDecryptStream(
            fileStream, bundleKey, Path.GetFileNameWithoutExtension(mdbPath));
        var bundle = manager.LoadBundleFile(cryptoStream, fileStream.Name);
        assets = manager.LoadAssetsFileFromBundle(bundle, 0);

        dbInfo = assets.file.GetAssetsOfType(AssetClassID.TextAsset).First();
        dbBase = manager.GetBaseField(assets, dbInfo);
        var masterDB = new MemoryStream(dbBase["m_Script"].AsByteArray);

        var header = MessagePackSerializer.Deserialize<Dictionary<string, (int, int)>>(masterDB);
        var offset = masterDB.Position;

        foreach (var kv in header)
        {
            var pack = new byte[kv.Value.Item2];
            masterDB.Seek(offset + kv.Value.Item1, SeekOrigin.Begin);
            masterDB.Read(pack, 0, pack.Length);
            JsonStrings[kv.Key] = MessagePackSerializer.ConvertToJson(pack, lz4Options);
        }
    }

    public void WriteBundle(string path)
    {
        var packs = new MemoryStream();
        var header = new Dictionary<string, (int, int)>();
        var offset = 0;
        foreach (var kv in JsonStrings)
        {
            var pack = MessagePackSerializer.ConvertFromJson(kv.Value, lz4Options);
            header[kv.Key] = (offset, pack.Length);
            offset += pack.Length;
            packs.Write(pack);
        }

        var masterDB = new MemoryStream();
        MessagePackSerializer.Serialize(masterDB, header);
        packs.WriteTo(masterDB);

        dbBase["m_Script"].AsByteArray = masterDB.ToArray();
        dbInfo.SetNewData(dbBase);
        PatchWorks.WriteAndCompressBundle(assets, path);
    }
}
