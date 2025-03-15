﻿using AbjjadAssignment.services.abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;

namespace AbjjadAssignment.services.implementations;

internal sealed class ImageProcessorService : IImageProcessor
{
    public async Task<(byte[] webpData, ImageMetadata metadata)> ProcessImageAsync(IFormFile image)
    {
        using var stream = image.OpenReadStream();
        using var img = await Image.LoadAsync(stream);

        var metaData = new ImageMetadata();
        if (img.Metadata.ExifProfile is not null)
        {
            IExifValue<Rational[]>? exifGeolocationLat = null;
            IExifValue<Rational[]>? exifGeolocationLong = null;
            IExifValue<string>? cameraMake = null;
            IExifValue<string>? model = null;

            var goeLocationLatOK =
                img.Metadata.ExifProfile?.TryGetValue(ExifTag.GPSLatitude, out exifGeolocationLat)
                ?? false;
            var geoLocationLongOK =
                img.Metadata.ExifProfile?.TryGetValue(ExifTag.GPSLongitude, out exifGeolocationLong)
                ?? false;

            string geoLocation = string.Empty;

            if (
                goeLocationLatOK
                && geoLocationLongOK
                && exifGeolocationLat is not null
                && exifGeolocationLong is not null
            )
            {
                var latitude = ConvertToDecimalDegrees(exifGeolocationLat.Value);
                var longitude = ConvertToDecimalDegrees(exifGeolocationLong.Value);

                geoLocation =
                    $"{latitude}° {(latitude >= 0 ? "N" : "S")}, {longitude}° {(longitude >= 0 ? "E" : "W")}";
            }

            var cameraMakeOK =
                img.Metadata.ExifProfile?.TryGetValue(ExifTag.Make, out cameraMake) ?? false;
            var modelOK = img.Metadata.ExifProfile?.TryGetValue(ExifTag.Model, out model) ?? false;

            metaData.GeoLocation = geoLocation;
            metaData.CameraMake =
                cameraMakeOK && cameraMake is not null ? cameraMake.Value! : string.Empty;
            metaData.CameraModel = modelOK && model is not null ? model.Value! : string.Empty;
        }

        using var ms = new MemoryStream();
        await img.SaveAsWebpAsync(ms);
        return (ms.ToArray(), metaData);
    }

    static double ConvertToDecimalDegrees(Rational[] coordinates)
    {
        if (coordinates.Length < 3)
            return 0;

        double degrees = (double)coordinates[0].Numerator / coordinates[0].Denominator;
        double minutes = (double)coordinates[1].Numerator / coordinates[1].Denominator;
        double seconds = (double)coordinates[2].Numerator / coordinates[2].Denominator;

        return degrees + (minutes / 60.0) + (seconds / 3600.0);
    }

    public async Task<byte[]> ResizeImageAsync(byte[] imageData, string size)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("Image data cannot be null or empty.");

        try
        {
            using var msInput = new MemoryStream(imageData);
            using var img = await Image.LoadAsync(msInput); // No 'out format' needed

            var (width, height) = size switch
            {
                Phone.PHONE => (Phone.Width, Phone.Height),
                Tablet.TABLET => (Tablet.Width, Tablet.Height),
                Desktop.DESKTOP => (Desktop.Width, Desktop.Height),
                _ => throw new ArgumentException("Invalid size"),
            };

            img.Mutate(x =>
                x.Resize(
                    new ResizeOptions { Size = new Size(width, height), Mode = ResizeMode.Max }
                )
            );

            using var msOutput = new MemoryStream();
            await img.SaveAsWebpAsync(msOutput);
            return msOutput.ToArray();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "An error occurred while processing the image.",
                ex
            );
        }
    }
}
