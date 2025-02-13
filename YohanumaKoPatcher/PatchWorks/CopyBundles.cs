partial class PatchWorks
{
    public void CopyBundles()
    {
        Console.WriteLine("Copying bundles");

        if (isNX)
        {
            if (!File.Exists(Path.Combine(outputPath, "localization-assets-chinese(traditional)(zh-tw)_assets_all.bundle")))
            {
                File.Copy(
                    Path.Combine(patchResourcesPath, "localization-assets-chinese(traditional)(zh-tw)_assets_all.bundle"),
                    Path.Combine(outputPath, "localization-assets-chinese(traditional)(zh-tw)_assets_all.bundle"));
            }
        }
        else
        {
            if (!File.Exists(Path.Combine(outputPath, "a06fcfada455adc1aa6476976bbc5dbf.bundle")))
            {
                File.Copy(
                    Path.Combine(patchResourcesPath, "a06fcfada455adc1aa6476976bbc5dbf.bundle"),
                    Path.Combine(outputPath, "a06fcfada455adc1aa6476976bbc5dbf.bundle"));
            }
        }
    }
}
