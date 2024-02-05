using Newtonsoft.Json;

namespace GodotPatcher;

public class Config
{
    private const string ConfigFileName = "GodotPatcher.cfg";

    public PainterConfig Painter = new PainterConfig();
    public PatcherConfig Patcher = new PatcherConfig();
    
    private static Config? _instance = null;
    
    public static Config Get()
    {
        if(_instance is not null) return _instance;
        
        _instance = new Config();
        try
        {
            if (File.Exists(ConfigFileName))
            {
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFileName));
                _instance = config;
            }
            else
            {
                Save();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Unable to read config file: " + e.Message);
            Console.WriteLine("Default config will be loaded");
        }
        
        return _instance;
    }

    public static void Set(Config config)
    {
        _instance = config;
    }

    public static void Save()
    {
        string data = JsonConvert.SerializeObject(_instance, Formatting.Indented);
        File.WriteAllText(ConfigFileName, data);
    }
}

public class PainterConfig
{
    public const string DefaultColor = "#478cbf";
    public string BaseColor = DefaultColor;
    public double HueAdjust = 65;

    public string[] Files = new[]
    {
        "logo_outlined.svgpng",
        "logo.svgpng",
        "icon.svgpng",
        "icon_outlined.svgpng",
        "misc/dist/project_icon.svg",
        "misc/dist/icon_console.svg",
        "main/app_icon.png",
        "main/splash.png",
        "platform/windows/godot.ico",
        "platform/windows/godot_console.ico",
        "editor/icons/DefaultProjectIcon.svg",
    };

}

public class PatcherConfig
{
    public string? GodotRename = "Abrodot";
    public List<string>? TargetExtensions = new() { ".sln", ".csproj", ".props", ".targets", ".py", ".sh", ".h", ".cpp", ".xml", ".cs", ".yml", ".xml" };
    public List<string>? ReplacementTargets = new() { "GodotSharp", "Godot.NET.Sdk", "Godot.SourceGenerators", "GodotPlugins", "GodotTools" };
    public Dictionary<string, string>? SpecificReplacements = new()
    {
        { "project.godot", "project.abd" },
        { "ends_with(\".godot\")", "ends_with(\".abd\")" },
        { "<GodotFloat64 Condition=\" '$(GodotFloat64)' == '' \">false</GodotFloat64>", "<GodotFloat64 Condition=\" '$(GodotFloat64)' == '' \">true</GodotFloat64>" }
    };
    public bool IgnoreExtensions = true;
    
// version.py
    public string? ShortName = "abrodot";
    public string? Name = "Abrodot Engine";
    public int Major = -1;
    public int Minor = -1;
    public int Patch = 1;
    public string? Status = "abro-stable";
    public string? ModuleConfig = null;
    public int Year = -2; // Use -1 to not change and -2 to replace with current year.
    public string? Website = null;
    public string? Docs = null;
}