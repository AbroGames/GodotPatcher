using ImageMagick;

namespace GodotPatcher.Processors;

public interface IImageProcessor
{
    /// <summary>
    /// Adjusts image's hue
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <param name="hueAngle">Angle between -180 and 180</param>
    void ModulateHue(string path, double hueAngle);

    static double FromAngle(double hueAngle)
    {
        return (100 + (hueAngle / 180.0) * 100);
    }
}