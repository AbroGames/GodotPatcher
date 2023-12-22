using ColorHelper;
using ImageMagick;

namespace GodotPatcher.Processors;

public class SvgProcessor : IImageProcessor
{
    /// <summary>
    /// Adjusts image's hue
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <param name="hueAngle">Angle between -180 and 180</param>
    public void ModulateHue(string path, double hueAngle)
    {
        var config = Config.Get();
        string origColor = config.Painter.LatestColor.ToLower(); // #478cbf by default
        string newColor = AdjustHex(origColor, hueAngle);
        var svgContents = File.ReadAllText(path);
        Console.WriteLine($"Replacing {origColor} with {newColor} in {path}");
        File.WriteAllText(path, svgContents.Replace(origColor, newColor));
    }

    private string AdjustHex(string origColor, double hueAngle)
    {
        var color = ColorConverter.HexToHsv(new HEX(origColor));
        color.H += (int)hueAngle;
        return "#" + ColorConverter.HsvToHex(color).Value;
    }
}