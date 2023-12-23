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
        
        Painter.PaintEverything("./");
        Patcher.DoPatch("./");
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