using System.Text.Json;

partial class PatchWorks
{
    public void UpdateMasterDataTable()
    {
        Console.WriteLine("Updating Master DB");
        
        string fileName = isNX ? "master_assets_all.bundle" : "0bd374d9376bad42e740a3c9fe1b6ea6.bundle";

        var mdb = new MasterDBPatcher(Path.Combine(bundlePath, fileName), bundleKey, isNX);

        var glossary = JsonSerializer.Deserialize<List<GlossaryEntry>>(mdb.JsonStrings["Glossary"])!;
        var glossarySource = glossary.Where(entry => entry.Language! == "ja")
            .ToDictionary(e => e.MasterID!, e => e);
        var glossaryLocalized = ReadCsvToTextTable(Path.Combine(patchResourcesPath, "tables", "glossary.csv"));

        glossary.RemoveAll(entry => entry.Language! == "zh_TW");

        foreach (var (k, v) in glossarySource)
        {
            var nameKey = $"{v.MasterID}.Name";
            var textKey = $"{v.MasterID}.Text";
            glossary.Add(new GlossaryEntry
            {
                ID = $"{v.MasterID}_zh_TW",
                MasterID = v.MasterID,
                Language = "zh_TW",
                Name = glossaryLocalized.ContainsKey(nameKey)
                    && glossaryLocalized[nameKey].Localized != ""
                    ? glossaryLocalized[nameKey].Localized
                    : v.Name,
                Text = glossaryLocalized.ContainsKey(textKey)
                    && glossaryLocalized[textKey].Localized != ""
                    ? glossaryLocalized[textKey].Localized
                    : v.Text,
                Icon = v.Icon
            });
        }
        mdb.JsonStrings["Glossary"] = JsonSerializer.Serialize(glossary);



        var tutorial = JsonSerializer.Deserialize<List<TutorialPageEntry>>(mdb.JsonStrings["TutorialPage"])!;
        var tutorialSource = tutorial.Where(entry => entry.Language == "ja")
            .ToDictionary(e => $"{e.TutorialID}/{e.Page}", e => e);
        var tutorialLocalized = ReadCsvToTextTable(Path.Combine(patchResourcesPath, "tables", "tutorialpage.csv"));

        tutorial.RemoveAll(entry => entry.Language == "zh_TW");

        foreach (var (k, v) in tutorialSource)
        {
            tutorial.Add(new TutorialPageEntry
            {
                TutorialID = v.TutorialID,
                Language = "zh_TW",
                Page = v.Page,
                Text = tutorialLocalized.ContainsKey(k)
                    && tutorialLocalized[k].Localized != ""
                    ? tutorialLocalized[k].Localized
                    : v.Text,
                ImageAddress = v.ImageAddress
            });
        }
        mdb.JsonStrings["TutorialPage"] = JsonSerializer.Serialize(tutorial);

        mdb.WriteBundle(Path.Combine(outputPath, fileName), bundleKey);
    }
}
