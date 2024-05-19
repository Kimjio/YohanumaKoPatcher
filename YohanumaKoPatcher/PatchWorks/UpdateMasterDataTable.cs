using System.Globalization;
using System.Text.Json;
using CsvHelper;

partial class PatchWorks
{
    public void UpdateMasterDataTable()
    {
        Console.WriteLine("Updating Master DB");
        var mdb = new MasterDBPatcher(Path.Combine(bundlePath, "0bd374d9376bad42e740a3c9fe1b6ea6.bundle"), bundleKey);

        using (var reader = new StreamReader(Path.Combine(patchResourcesPath, "tables", "glossary.csv")))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var glossaryEntries = JsonSerializer.Deserialize<List<GlossaryEntry>>(mdb.JsonStrings["Glossary"])!;
            var sources = glossaryEntries.Where(entry => entry.Language! == "ja")
                .ToDictionary(e => e.MasterID!, e => e);
            var updateDict = new Dictionary<string, GlossaryEntry>();
            foreach (var record in csv.GetRecords<TextTable>())
            {
                var split = record.Location.Split('.');
                var id = split[0];
                var type = split[1];
                if (!updateDict.ContainsKey(id) && sources.ContainsKey(id))
                {
                    var src = sources[id];
                    updateDict[id] = new GlossaryEntry
                    {
                        ID = $"{id}_ko",
                        MasterID = src.MasterID,
                        Language = "ko",
                        Name = src.Name,
                        Text = src.Text,
                        Icon = src.Icon
                    };
                }
                if (record.Localized != "")
                {
                    if (type == "Name") updateDict[id].Name = record.Localized;
                    else updateDict[id].Text = record.Localized;
                }
            }
            glossaryEntries.AddRange(updateDict.Values);
            mdb.JsonStrings["Glossary"] = JsonSerializer.Serialize(glossaryEntries);
        }


        using (var reader = new StreamReader(Path.Combine(patchResourcesPath, "tables", "tutorialpage.csv")))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var tutorialEntries = JsonSerializer.Deserialize<List<TutorialPageEntry>>(mdb.JsonStrings["TutorialPage"])!;
            var sources = tutorialEntries.Where(entry => entry.Language == "ja")
                .ToDictionary(e => $"{e.TutorialID}/{e.Page}", e => e);
            var updateDict = new Dictionary<string, TutorialPageEntry>();
            foreach (var record in csv.GetRecords<TextTable>())
            {
                if (!sources.ContainsKey(record.Location)) continue;
                var src = sources[record.Location];
                updateDict[record.Location] = new TutorialPageEntry
                {
                    TutorialID = src.TutorialID,
                    Language = "ko",
                    Page = src.Page,
                    Text = string.IsNullOrEmpty(record.Localized) ? src.Text : record.Localized,
                    ImageAddress = src.ImageAddress
                };
            }
            tutorialEntries.AddRange(updateDict.Values);
            mdb.JsonStrings["TutorialPage"] = JsonSerializer.Serialize(tutorialEntries);
        }
        mdb.WriteBundle(Path.Combine(outputPath, "0bd374d9376bad42e740a3c9fe1b6ea6.bundle"));
    }
}