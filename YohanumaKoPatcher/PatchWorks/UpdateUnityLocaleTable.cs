using AssetsTools.NET.Extra;

partial class PatchWorks
{
    public void UpdateUnityLocaleTable()
    {
        Console.WriteLine("Updating Unity locale table");
        var sharedAssets = LoadAssets(Path.Combine(bundlePath, "46c941530c6f45cd51b8e149487c8160.bundle"), bundleKey);

        var keyMap = GetScriptInfosByClassName(sharedAssets, "SharedTableData")
            .SelectMany(t => manager.GetBaseField(sharedAssets, t)["m_Entries.Array"].Children)
            .ToDictionary(i => i["m_Key"].AsString, i => i["m_Id"].AsLong);

        var assets = LoadAssets(Path.Combine(patchResourcesPath, "localetable_template.bundle"));
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
            var records = ReadCsvToTextTable(Path.Combine(patchResourcesPath, "tables", fileMap[fields["m_Name"].AsString]));
            foreach (var record in records.Values)
            {
                var tableEntry = ValueBuilder.DefaultValueFieldFromArrayTemplate(tableArray);
                tableEntry["m_Id"].AsLong = keyMap[record.Location];
                tableEntry["m_Localized"].AsString = record.Localized == "" ? record.Source : record.Localized;
                tableArray.Children.Add(tableEntry);
            }
            table.SetNewData(fields);
        }
        WriteAndCompressBundle(assets, Path.Combine(outputPath, "f38d2768ca886a5dbbe339e1d5a46132.bundle"));
    }
}