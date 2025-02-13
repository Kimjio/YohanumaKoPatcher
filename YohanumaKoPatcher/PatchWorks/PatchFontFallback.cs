using AssetsTools.NET.Extra;

partial class PatchWorks
{
    public void PatchFontFallback()
    {
        Console.WriteLine("Patching font fallback");

        string fileName = isNX ? "localization-assets-shared_assets_all.bundle" : "46c941530c6f45cd51b8e149487c8160.bundle";

        if (File.Exists(Path.Combine(outputPath, fileName)))
        {
            Console.WriteLine("Already patched. Skipping.");
            return;
        }
        var assets = LoadAssets(Path.Combine(bundlePath, fileName), bundleKey);

        var fonts = GetScriptInfosByClassName(assets, "TMP_FontAsset")
            .ToDictionary(asset => manager.GetBaseField(assets, asset)["m_Name"].AsString, asset => asset);

        var fields = manager.GetBaseField(assets, fonts["FOT-SkipStd-B SDF"]);
        var fallbackArray = fields["m_FallbackFontAssetTable.Array"];

        var fallbackEntry = ValueBuilder.DefaultValueFieldFromArrayTemplate(fallbackArray);
        fallbackEntry["m_FileID"].AsInt = 0;
        fallbackEntry["m_PathID"].AsLong = fonts["NotoSerifHK-Black SDF"].PathId;
        fallbackArray.Children.Add(fallbackEntry);

        fonts["FOT-SkipStd-B SDF"].SetNewData(fields);
        WriteAndCompressBundle(assets, Path.Combine(outputPath, fileName), bundleKey);

        assets.AssetsStream.Close();
    }
}
