using System.Text;

namespace GodotPatcher.Tools;

public static class Logger
{
    private const string LogPrefix  = "[INFO]:  ";
    private const string WarnPrefix = "[WARN]:  ";
    private const string ErrPrefix  = "[ERROR]: ";
    public static void Log(object message, uint depth = 0)
    {
        var sb = new StringBuilder();
        sb.Append(LogPrefix);
        for (int i = 0; i < depth; i++)
            sb.Append("  ");

        sb.Append(message);
        Console.WriteLine(sb.ToString());
    }

    public static void Log(object message)
    {
        var sb = new StringBuilder();
        sb.Append(LogPrefix);
        sb.Append(message);
        Console.WriteLine(sb.ToString());
    }

    public static void Warn(object message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        var sb = new StringBuilder();
        sb.Append(WarnPrefix);
        sb.Append(message);
        Console.WriteLine(sb.ToString());
        Console.ForegroundColor = color;
    }

    public static void Error(object message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        var sb = new StringBuilder();
        sb.Append(ErrPrefix);
        sb.Append(message);
        Console.WriteLine(sb.ToString());
        Console.ForegroundColor = color;
    }
}