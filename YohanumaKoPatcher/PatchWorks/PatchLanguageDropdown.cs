partial class PatchWorks
{
    public void PatchLanguageDropdown()
    {
        Console.WriteLine("Patching language selector dropdown");

        string fileName = isNX ? "setting_assets_all.bundle" : "d52fd8377a93be75ed1c29ef4869754d.bundle";

        if (File.Exists(Path.Combine(outputPath, fileName)))
        {
            Console.WriteLine("Already patched. Skipping.");
            return;
        }
        var assets = LoadAssets(Path.Combine(bundlePath, fileName), bundleKey);
        foreach (var dropdown in GetScriptInfosByClassName(assets, "CustomTMPDropdown"))
        {
            var fields = manager.GetBaseField(assets, dropdown);
            var optionsArray = fields["m_Options.m_Options.Array"];
            optionsArray.Children[2]["m_Text"].AsString = "한국어";
            dropdown.SetNewData(fields);
        }
        WriteAndCompressBundle(assets, Path.Combine(outputPath, fileName), bundleKey);

        assets.AssetsStream.Close();
    }
}
