namespace AbjjadAssignment.services;

public static class Phone
{
    public const string PHONE = "phone";
    public static readonly int Width = 480;
    public static readonly int Height = 854;
}

public static class Tablet
{
    public const string TABLET = "tablet";
    public static readonly int Width = 768;
    public static readonly int Height = 1024;
}

public static class Desktop
{
    public const string DESKTOP = "desktop";
    public static readonly int Width = 1920;
    public static readonly int Height = 1080;
}

public static class ImageConstants
{
    public static readonly string ImagesFolderPath = "ImageStorage";

    public static readonly long MaxFileSize = 2 * 1024 * 1024; // 2MB
    public static readonly string[] AllowedExtensions = { ".jpg", ".png", ".webp" };

    public static readonly Dictionary<string, (int Width, int Height)> SizePresets = new()
    {
        [Phone.PHONE] = (Phone.Width, Phone.Height),
        [Tablet.TABLET] = (Tablet.Width, Tablet.Height),
        [Desktop.DESKTOP] = (Desktop.Width, Desktop.Height),
    };
}
