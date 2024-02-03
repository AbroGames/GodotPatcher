using System.Globalization;
using System.Text.RegularExpressions;
using ColorHelper;
using GodotPatcher.Processors;
using GodotPatcher.Tools;

namespace GodotPatcher;

public static class Painter
{
    public static void PaintEverything(string path)
    {
        var images = Config.Get().Painter.Files;
        foreach (var image in images)
        {
            var format = Path.GetExtension(image).Substring(1);
            Logger.Log($"Painting {image} as {format}");
            ImageProcessors.Processors[format].ModulateHue(Path.GetFullPath(Path.Combine(path,image)), Config.Get().Painter.HueAdjust);
        }
        
        // Disabled for now
        //PatchThemes(path);
    }

    private static void PatchThemes(string root)
    {
        var path = Path.Combine(root, "editor/editor_themes.cpp");
        var contents = File.ReadAllText(path);
        var newContents = contents;
        var colorRegex = new Regex(@"(^\s*preset_(accent|base)_color\s*=\s*Color\s*\()(\s*\d\.\d*\s*),(\s*\d\.\d*\s*),(\s*\d\.\d*\s*)(\).*$)", RegexOptions.Multiline);
        var matches = colorRegex.Matches(contents);
        foreach (Match match in matches)
        {
            // Groups:
            // 0: full match
            // 1: beginning
            // 2: color type
            // 3: r
            // 4: g
            // 5: b
            // 6: ending
            var r = Double.Parse(match.Groups[3].ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
            var g = Double.Parse(match.Groups[4].ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
            var b = Double.Parse(match.Groups[5].ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
            var origColor = ColorConverter.RgbToHsv(new RGB((byte)(r*255), (byte)(g*255), (byte)(b*255)));
            
            origColor.H += (int)Config.Get().Painter.HueAdjust;
            
            var newColor = ColorConverter.HsvToRgb(origColor);
            var newR = newColor.R / 255.0;
            var newG = newColor.G / 255.0;
            var newB = newColor.B / 255.0;
            
            var lineReplacement = $"{match.Groups[1]}" +
                                  $"{newR.ToString("N2", CultureInfo.InvariantCulture)}, " +
                                  $"{newG.ToString("N2", CultureInfo.InvariantCulture)}, " +
                                  $"{newB.ToString("N2", CultureInfo.InvariantCulture)}" +
                                  $"{match.Groups[6]}";
            Logger.Log($"Replacing {match.Groups[0]} with {lineReplacement}");
            newContents = newContents.Replace(match.Groups[0].ToString(), lineReplacement);
        }
        
        File.WriteAllText(path, newContents);
    }
    
    private static IImageProcessor GetProcessor(string format)
    {
        return ImageProcessors.Processors[format.ToLower()];
    }

    private static void ProcessDirectory(string path)
    {
        
    }
}