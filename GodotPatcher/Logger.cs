using System.Text;

namespace GodotPatcher;

public static class Logger
{
    public static void Log(object message, uint depth = 0)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < depth; i++) 
            sb.Append("  ");

        sb.Append(message);
        Console.WriteLine(sb.ToString());
    }
    
    public static void Warn(object message, uint depth = 0)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log(message, depth);
        Console.ForegroundColor = color;
    }
}