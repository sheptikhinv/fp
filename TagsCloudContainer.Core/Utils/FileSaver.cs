using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudContainer.Core.Utils;

public static class FileSaver
{
    public static Result<string> SaveFile(Bitmap bitmap, string path)
    {
        var extension = Path.GetExtension(path).ToLower();
        var imageFormat = extension.GetImageFormat();
        Result<string> result;
        if (imageFormat.IsSuccess)
        {
            result = Result.Ok(path);
            bitmap.Save(path, imageFormat.Value);
        }
        else
        {
            result = Result.Fail<string>(imageFormat.Error);
        }

        bitmap.Dispose();
        return result;
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

    public static Result<string> GetImageExtension(this ImageFormat format)
    {
        if (format.Equals(ImageFormat.Jpeg))
            return ".jpg".AsResult();
        if (format.Equals(ImageFormat.Png))
            return ".png".AsResult();
        if (format.Equals(ImageFormat.Bmp))
            return ".bmp".AsResult();
        if (format.Equals(ImageFormat.Gif))
            return ".gif".AsResult();
        if (format.Equals(ImageFormat.Tiff))
            return ".tiff".AsResult();

        return Result.Fail<string>($"Unknown image format {format}");
    }
}