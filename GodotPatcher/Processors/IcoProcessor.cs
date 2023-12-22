using ImageMagick;

namespace GodotPatcher.Processors;

public class IcoProcessor : IImageProcessor
{
    public void ModulateHue(string path, double hueAngle)
    {
        var origImages = new MagickImageCollection(path);
        var newImages = new MagickImageCollection();
        
        foreach (var image in origImages)
        {
            var huePercentage = (Percentage)IImageProcessor.FromAngle(hueAngle);
            image.Modulate((Percentage)100, (Percentage)100, huePercentage);
            newImages.Add(image);
        }
        
        newImages.Write(path);
    }
}