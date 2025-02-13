using AssetsTools.NET.Extra;

partial class PatchWorks
{
    public void UpdateUnityLocaleTable()
    {
        Console.WriteLine("Updating Unity locale table");

        string fileName = isNX ? "localization-assets-shared_assets_all.bundle" : "46c941530c6f45cd51b8e149487c8160.bundle";
        string fileName2 = isNX ? "localization-string-tables-chinese(traditional)(zh-tw)_assets_all.bundle" : "bc693a3197ea49742faa00a88c5633b2.bundle";

        var sharedAssets = LoadAssets(Path.Combine(bundlePath, fileName), bundleKey);

        var keyMap = GetScriptInfosByClassName(sharedAssets, "SharedTableData")
            .SelectMany(t => manager.GetBaseField(sharedAssets, t)["m_Entries.Array"].Children)
            .ToDictionary(i => i["m_Key"].AsString, i => i["m_Id"].AsLong);

        var assets = LoadAssets(Path.Combine(patchResourcesPath, $"localetable_template{(isNX ? "_nx" : "")}.bundle"));
        var tables = GetScriptInfosByClassName(assets, "StringTable");

        var fileMap = new Dictionary<string, string>
        {
            {"UI_TextTable_zh-TW", "ui.csv"},
            {"Story_TextTable_zh-TW", "story.csv"}
        };

        foreach (var table in tables)
        {
            var fields = manager.GetBaseField(assets, table);
            var tableArray = fields["m_TableData.Array"];
            tableArray.Children.Clear();
            var records = ReadCsvToTextTable(Path.Combine(patchResourcesPath, "tables", fileMap[fields["m_Name"].AsString]));
            foreach (var record in records.Values)
            {
                if (keyMap.ContainsKey(record.Location))
                {
                    var tableEntry = ValueBuilder.DefaultValueFieldFromArrayTemplate(tableArray);
                    tableEntry["m_Id"].AsLong = keyMap[record.Location];
                    tableEntry["m_Localized"].AsString = record.Localized;
                    tableArray.Children.Add(tableEntry);
                }
            }
            table.SetNewData(fields);
        }
        WriteAndCompressBundle(assets, Path.Combine(outputPath, fileName2), bundleKey);

        assets.AssetsStream.Close();
        sharedAssets.AssetsStream.Close();
    }
}
