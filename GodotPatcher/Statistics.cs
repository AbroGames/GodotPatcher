namespace GodotPatcher;

public static class Statistics
{
    public static int ScannedDirectories { get; private set; } = 0;
    public static int ScannedFiles { get; private set; } = 0;
    public static int PatchedFiles { get; private set; } = 0;
    public static int RenamedFiles { get; private set; } = 0;
    public static int RenamedDirectories { get; private set; } = 0;

    public static void Reset()
    {
        ScannedDirectories = 0;
        ScannedFiles = 0;
        PatchedFiles = 0;
        RenamedFiles = 0;
        RenamedDirectories = 0;
    }
    
    public static int CountScannedDir() => ++ScannedDirectories;
    public static int CountScannedFile() => ++ScannedFiles;
    public static int CountPatchedFile() => ++PatchedFiles;
    public static int CountRenamedFile() => ++RenamedFiles;
    public static int CountRenamedDir() => ++RenamedDirectories;
}