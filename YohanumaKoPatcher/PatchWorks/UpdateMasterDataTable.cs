using System.Text.Json;

partial class PatchWorks
{
    public void UpdateMasterDataTable()
    {
        Console.WriteLine("Updating Master DB");
        var mdb = new MasterDBPatcher(Path.Combine(bundlePath, "0bd374d9376bad42e740a3c9fe1b6ea6.bundle"), bundleKey);

        var glossary = JsonSerializer.Deserialize<List<GlossaryEntry>>(mdb.JsonStrings["Glossary"])!;
        var glossarySource = glossary.Where(entry => entry.Language! == "ja")
            .ToDictionary(e => e.MasterID!, e => e);
        var glossaryLocalized = ReadCsvToTextTable(Path.Combine(patchResourcesPath, "tables", "glossary.csv"));
        if (isDemo)
        {
            var demo = ReadCsvToTextTable(Path.Combine(patchResourcesPath, "tables", "glossary_demo.csv"));
            foreach (var (k, v) in demo) glossaryLocalized[k] = v;
        }

        foreach (var (k, v) in glossarySource)
        {
            var nameKey = $"{v.MasterID}.Name";
            var textKey = $"{v.MasterID}.Text";
            glossary.Add(new GlossaryEntry
            {
                ID = $"{v.MasterID}_ko",
                MasterID = v.MasterID,
                Language = "ko",
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
        foreach (var (k, v) in tutorialSource)
        {
            tutorial.Add(new TutorialPageEntry
            {
                TutorialID = v.TutorialID,
                Language = "ko",
                Page = v.Page,
                Text = tutorialLocalized.ContainsKey(k)
                    && tutorialLocalized[k].Localized != ""
                    ? tutorialLocalized[k].Localized
                    : v.Text,
                ImageAddress = v.ImageAddress
            });
        }
        mdb.JsonStrings["TutorialPage"] = JsonSerializer.Serialize(tutorial);

        mdb.WriteBundle(Path.Combine(outputPath, "0bd374d9376bad42e740a3c9fe1b6ea6.bundle"));
    }
}