
namespace GodotPatcher.Processors;

public static class ImageProcessors
{
    private static Dictionary<string, IImageProcessor> _processors = new();
    public static IReadOnlyDictionary<string, IImageProcessor> Processors => _processors.AsReadOnly();

    static ImageProcessors()
    {
        _processors.Add("png", new PngProcessor());
        _processors.Add("ico", new IcoProcessor());
        _processors.Add("svg", new SvgProcessor());
        _processors.Add("svgpng", new SvgPngProcessor());
    }
}