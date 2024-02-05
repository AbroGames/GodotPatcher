using ColorHelper;
using GodotPatcher.Tools;

namespace GodotPatcher;

class Program
{
    static void Main(string[] args)
    {
        ConsoleHelper.OutputReceived += (o, s) => { Logger.Log(s); };
        if (args.Contains("--reset"))
        {
            Reset();
            return;
        }
        
        if (args.Contains("--normalize"))
        {
            Normalize();
            return;
        }
        
        Painter.PaintEverything("./");
        Patcher.DoPatch("./");
    }

    private static void Normalize()
    {
        var config = Config.Get().Patcher;
        Dictionary<string, string> renamings = new()
        {
            {"godot.windows.editor.double.x86_64.mono", config.GodotRename},
            {"godot.windows.template_debug.double.x86_64.mono", "windows_debug_x86_64"},
            {"godot.windows.template_release.double.x86_64.mono", "windows_release_x86_64"}
        };
        
        string basePath = "./bin/";
        string buildName = $"{config.Major}.{config.Minor}.{config.Patch}.{config.Status}.mono";
        File.Create(Path.Combine(basePath, "_sc_")).Dispose();
        string exportPath = Path.Combine(basePath, @"editor_data\export_templates\"+buildName);
        
        Directory.CreateDirectory(exportPath);
        Directory.Delete(exportPath, true);
        Directory.CreateDirectory(exportPath);

        var files = Directory.GetFiles(basePath);
        
        foreach (var (key, value) in renamings)
        {
            foreach (var file in files)
            {
                var newFile = file.Replace(key, value);
                if (file == newFile) continue;
                
                if(File.Exists(newFile))
                {
                    File.Delete(newFile);
                }

                var fileName = Path.GetFileName(newFile);
                if (fileName.Contains("windows_debug_x86_64") || fileName.Contains("windows_release_x86_64"))
                {
                    File.Move(file, Path.Combine(exportPath, fileName));
                }
                else
                {
                    File.Move(file, newFile);
                }
            }
        }
        
        
    }
    
    private static void Reset()
    {
        try
        {
            Directory.Delete("modules/mono", true);
            ConsoleHelper.ExecuteCommand("git reset --hard");
        }
        catch (Exception e)
        {
            Logger.Error("Unable to reset repo: " + e.Message);
        }
    }
}