using ImageMagick;


namespace GodotPatcher.Processors;

public class PngProcessor : IImageProcessor
{
    public void ModulateHue(string path, double hueAngle)
    {
        var image = new MagickImage(path);
        Percentage huePercentage = (Percentage)IImageProcessor.FromAngle(hueAngle);
        
        image.Modulate((Percentage)100, (Percentage)100, huePercentage);
        image.Write(path);
    }
}