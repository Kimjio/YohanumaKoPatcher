partial class PatchWorks
{
    public void CopyBundles()
    {
        Console.WriteLine("Copying bundles");
        if (!File.Exists(Path.Combine(outputPath, "6abb75f3680e3b06557ab0c4d27bf44d.bundle")))
        {
            File.Copy(
                Path.Combine(patchResourcesPath, "6abb75f3680e3b06557ab0c4d27bf44d.bundle"),
                Path.Combine(outputPath, "6abb75f3680e3b06557ab0c4d27bf44d.bundle"));
        }
        if (!File.Exists(Path.Combine(outputPath, "6b8ef1bef19c8efb79d76be851f882b7.bundle")))
        {
            File.Copy(
                Path.Combine(patchResourcesPath, "6b8ef1bef19c8efb79d76be851f882b7.bundle"),
                Path.Combine(outputPath, "6b8ef1bef19c8efb79d76be851f882b7.bundle"));
        }
        if (!File.Exists(Path.Combine(outputPath, "a1c0ae0d8f7d2f12172abde5734b6aec.bundle")))
        {
            File.Copy(
                Path.Combine(patchResourcesPath, "a1c0ae0d8f7d2f12172abde5734b6aec.bundle"),
                Path.Combine(outputPath, "a1c0ae0d8f7d2f12172abde5734b6aec.bundle"));
        }
    }
}