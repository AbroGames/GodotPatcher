namespace GodotPatcher.Processors;

public class SvgPngProcessor : IImageProcessor
{
    public void ModulateHue(string path, double hueAngle)
    {
        var withoutExtension = path.Replace(".svgpng", "");
        var svg = ImageProcessors.Processors["svg"];
        var png = ImageProcessors.Processors["png"];
        
        svg.ModulateHue(withoutExtension+".svg", hueAngle);
        png.ModulateHue(withoutExtension+".png", hueAngle);
    }
}