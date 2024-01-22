using System.Text.RegularExpressions;
using GodotPatcher.Tools;

namespace GodotPatcher;

public static class Patcher
{
    private static List<string> replacementTargets;
    static List<string> targetExtensions;
    private static Dictionary<string, string> replacementPairs = new();

    static Patcher()
    {
        Configure();
    }
    
    public static void Configure()
    {
        var config = Config.Get();
        replacementTargets = config.Patcher.ReplacementTargets ?? replacementTargets;
        targetExtensions = config.Patcher.TargetExtensions ?? targetExtensions;
        
        foreach (var replacementTarget in replacementTargets)
        {
            replacementPairs[replacementTarget] = replacementTarget.Replace("Godot", config.Patcher.GodotRename ?? "Godot");
        }
        foreach (var (key, value) in config.Patcher.SpecificReplacements)
        {
            replacementPairs[key] = value;
        }
    }
    
    public static void DoUnpatch(string path)
    {
        Console.WriteLine("Attempt to rollback patches...");
        DoPatch(path, true);
    }
    
    
    public static void DoPatch(string path, bool inverted = false)
    {
        Statistics.Reset();
        Console.ForegroundColor = ConsoleColor.Gray;
        UpdateVersion(Config.Get());
        ProcessDirectory(path, 0, inverted);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Scanned {Statistics.ScannedDirectories} dirs and {Statistics.ScannedFiles} files");
        Console.WriteLine($"Renamed {Statistics.RenamedDirectories} dirs and {Statistics.RenamedFiles} files");
        Console.WriteLine($"Patched {Statistics.PatchedFiles} files");
        Console.ResetColor();
    }

    private static void UpdateVersion(Config config)
    {
        var contents = File.ReadAllText("version.py");

        contents = UpdateVersionData(contents, "short_name", config.Patcher.ShortName);
        contents = UpdateVersionData(contents, "name", config.Patcher.Name);
        contents = UpdateVersionData(contents, "status", config.Patcher.Status);
        contents = UpdateVersionData(contents, "module_config", config.Patcher.ModuleConfig);
        contents = UpdateVersionData(contents, "website", config.Patcher.Website);
        contents = UpdateVersionData(contents, "docs", config.Patcher.Docs);
        
        contents = UpdateVersionData(contents, "major", config.Patcher.Major);
        contents = UpdateVersionData(contents, "minor", config.Patcher.Minor);
        contents = UpdateVersionData(contents, "patch", config.Patcher.Patch);
        contents = UpdateVersionData(contents, "year", config.Patcher.Year);
        
        File.WriteAllText("version.py", contents);
    }
    
    private static string UpdateVersionData(string fileContents, string field, string? replacement)
    {
        if (replacement is not null)
        {
            var regex = new Regex($"(^{field} = )(.*)($)", RegexOptions.Multiline);
            var replaced = regex.Replace(fileContents, $"${{1}}\"{replacement}\"${{3}}");
            return replaced;
        }

        return fileContents;
    }

    private static string UpdateVersionData(string fileContents, string field, int replacement = -1)
    {
        if (replacement >= 0)
        {
            var regex = new Regex($"(^{field} = )(.*)($)", RegexOptions.Multiline);
            var replaced = regex.Replace(fileContents, $"${{1}}{replacement}${{3}}");
            return replaced;
        }
        if (replacement == -2 && field.Equals("year"))
        {
            var regex = new Regex($"(^{field} = )(.*)($)", RegexOptions.Multiline);
            var replaced = regex.Replace(fileContents, $"${{1}}{DateTime.Now.Year}${{3}}");
            return replaced;
        }

        return fileContents;
    }
    
    public static void ProcessDirectory(string dirPath, uint depth  = 0, bool inverted = false)
    {
        Logger.Log($"#{Statistics.CountScannedDir()} Processing directory {dirPath}:", depth);
        var dirs = Directory.GetDirectories(dirPath);
        var files = Directory.GetFiles(dirPath);
        
        foreach (var dir in dirs)
        {
            ProcessDirectory(dir, depth + 1, inverted);
        }
        
        foreach (var file in files)
        {
            TryReplaceFileContents(file, depth + 1, inverted);
            TryRenameByPath(file, depth + 1, inverted);
        }
        
        TryRenameByPath(dirPath, depth, inverted);
    }

    private static void TryReplaceFileContents(string file, uint depth, bool inverted = false)
    {
        foreach (var targetExtension in targetExtensions)
        {
            if(!file.EndsWith(targetExtension))
                continue;
            Statistics.CountScannedFile();
            var contents = File.ReadAllText(file);
            var newContents = contents;
            foreach (var (key, value) in replacementPairs)
            {
                newContents = !inverted 
                    ? newContents.Replace(key, value)
                    : newContents.Replace(value, key);
            }

            if (!newContents.Equals(contents))
            {
                Statistics.CountPatchedFile();File.WriteAllText(file, newContents);
                Logger.Log($"#{Statistics.PatchedFiles} Patched contents of {Path.GetFileName(file)}", depth);
            }
            
            break;
        }
    }

    public static void TryRenameByPath(string path, uint depth = 0, bool inverted = false)
    {
        var name = Path.GetFileName(path);
        var baseName = name;
        var parentDir = Path.GetDirectoryName(path);
        
        foreach (var (key, value) in replacementPairs)
        {
            name = !inverted 
                ? name.Replace(key, value)
                : name.Replace(value, key);
        }

        if (name.Equals(baseName))
            return;

        var newPath = Path.Combine(parentDir, name);
        
        FileAttributes attr = File.GetAttributes(path);
        if (attr.HasFlag(FileAttributes.Directory))
        {
            Directory.Move(path, newPath);
            Logger.Log($"#{Statistics.CountRenamedDir()} Directory {baseName} was renamed to {name}", depth);
        }
        else
        {
            File.Move(path, newPath);
            Logger.Log($"#{Statistics.CountRenamedFile()} File {baseName} was renamed to {name}", depth);
        }
    }
}