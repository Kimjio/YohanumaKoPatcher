using XdeltaDecoder = PleOps.XdeltaSharp.Decoder;

partial class PatchWorks
{
    public void PatchBinary()
    {
        Console.WriteLine("Patching game binary");
        if (isPatched)
        {
            Console.WriteLine("Already patched. Skipping.");
            return;
        }
        var gameAssemblySource = Path.Join(gamePath, "GameAssembly.dll");
        var gameAssemblyTarget = Path.Join(gamePath, "GameAssembly_patched.dll");
        using (var input = File.OpenRead(gameAssemblySource))
        using (var patch = File.OpenRead(Path.Join(patchResourcesPath, "binarypatch", gameVersion, "GameAssembly.vcdiff")))
        using (var output = File.Create(gameAssemblyTarget))
        using (var decoder = new XdeltaDecoder.Decoder(input, patch, output))
        {
            decoder.Run();
        }
        File.Replace(gameAssemblyTarget, gameAssemblySource, null);

        var globalMetadataSource = Path.Join(gameResourcePath, "il2cpp_data", "Metadata", "global-metadata.dat");
        var globalMetadataTarget = Path.Join(gameResourcePath, "il2cpp_data", "Metadata", "global-metadata_patched.dat");
        using (var input = File.OpenRead(globalMetadataSource))
        using (var patch = File.OpenRead(Path.Join(patchResourcesPath, "binarypatch", gameVersion, "global-metadata.vcdiff")))
        using (var output = File.Create(globalMetadataTarget))
        using (var decoder = new XdeltaDecoder.Decoder(input, patch, output))
        {
            decoder.Run();
        }
        File.Replace(globalMetadataTarget, globalMetadataSource, null);

    }
}