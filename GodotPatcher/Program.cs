using ColorHelper;

namespace GodotPatcher;

class Program
{
    static void Main(string[] args)
    {
        Painter.PaintEverything("./");
        Patcher.DoPatch("./");
    }
}