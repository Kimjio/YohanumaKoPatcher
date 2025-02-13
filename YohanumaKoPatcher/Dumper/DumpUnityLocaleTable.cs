using AssetsTools.NET.Extra;

partial class PatchWorks
{
    public void DumpUnityLocaleTable()
    {
        Console.WriteLine("Dumping Unity locale table");

        string fileName = isNX ? "localization-assets-shared_assets_all.bundle" : "46c941530c6f45cd51b8e149487c8160.bundle";
        string fileName2 = isNX ? "localization-string-tables-korean(ko)_assets_all.bundle" : "f38d2768ca886a5dbbe339e1d5a46132.bundle";

        var sharedAssets = LoadAssets(Path.Combine(bundlePath, fileName), bundleKey);

        var keyMap = GetScriptInfosByClassName(sharedAssets, "SharedTableData")
            .SelectMany(t => manager.GetBaseField(sharedAssets, t)["m_Entries.Array"].Children)
            .ToDictionary(i => i["m_Id"].AsLong, i => i["m_Key"].AsString);

        var assets = LoadAssets(Path.Combine(bundlePath, fileName2), bundleKey);
        var tables = GetScriptInfosByClassName(assets, "StringTable");

        var fileMap = new Dictionary<string, string>
        {
            {"UI_TextTable_ko", "ui.csv"},
            {"Story_TextTable_ko", "story.csv"}
        };

        foreach (var table in tables)
        {
            var fields = manager.GetBaseField(assets, table);
            var tableArray = fields["m_TableData.Array"];

            var textTable = tableArray.Children.Select(entry => new TextTable
            {
                Location = keyMap[entry["m_Id"].AsLong],
                Localized = entry["m_Localized"].AsString
            }).ToList();

            WriteTextTableToCsv(Path.Combine(patchResourcesPath, "tables", fileMap[fields["m_Name"].AsString]), textTable);
        }

        assets.AssetsStream.Close();
        sharedAssets.AssetsStream.Close();
    }
}
