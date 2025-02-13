using System.Text.Json;

partial class PatchWorks
{
    public void DumpMasterDataTable()
    {
        Console.WriteLine("Dumping Master DB");

        string fileName = isNX ? "master_assets_all.bundle" : "0bd374d9376bad42e740a3c9fe1b6ea6.bundle";

        var mdb = new MasterDBPatcher(Path.Combine(bundlePath, fileName), bundleKey, isNX);

        var glossary = JsonSerializer.Deserialize<List<GlossaryEntry>>(mdb.JsonStrings["Glossary"])!;

        var glossaryTextTable = glossary.Where(entry => entry.Language! == "ko")
            .SelectMany(entry => new TextTable[]
            {
                new() { Location = $"{entry.MasterID}.Name", Localized = entry.Name! },
                new() { Location = $"{entry.MasterID}.Text", Localized = entry.Text! }
            }
            )
            .ToList();

        WriteTextTableToCsv(Path.Combine(patchResourcesPath, "tables", "glossary.csv"), glossaryTextTable);



        var tutorial = JsonSerializer.Deserialize<List<TutorialPageEntry>>(mdb.JsonStrings["TutorialPage"])!;

        var tutorialTextTable = tutorial.Where(entry => entry.Language! == "ko")
            .Select(entry => new TextTable { Location = $"{entry.TutorialID}/{entry.Page}", Localized = entry.Text! })
            .ToList();

        WriteTextTableToCsv(Path.Combine(patchResourcesPath, "tables", "tutorialpage.csv"), tutorialTextTable);
    }
}
