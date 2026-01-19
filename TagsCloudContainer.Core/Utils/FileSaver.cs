using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudContainer.Core.Utils;

public static class FileSaver
{
    public static Result<string> SaveFile(Bitmap bitmap, string path)
    {
        var extension = Path.GetExtension(path).ToLower();
        var imageFormat = extension.GetImageFormat();
        if (!imageFormat.IsSuccess)
            return Result.Fail<string>(imageFormat.Error);
        bitmap.Save(path, imageFormat.Value);
        bitmap.Dispose();
        return Result.Ok(path);
    }

    public static Result<ImageFormat> GetImageFormat(this string extension)
    {
        return extension switch
        {
            ".jpg" => ImageFormat.Jpeg.AsResult(),
            ".png" => ImageFormat.Png.AsResult(),
            ".bmp" => ImageFormat.Bmp.AsResult(),
            ".gif" => ImageFormat.Gif.AsResult(),
            ".tiff" => ImageFormat.Tiff.AsResult(),
            _ => Result.Fail<ImageFormat>($"Unknown image format {extension}")
        };
    }
}