using System.Security.Cryptography;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace ImageGen.Pipeline;

public static class Utils
{
    public static string FileHash(string path)
    {
        using var md5 = MD5.Create();
        using var fs = File.OpenRead(path);
        return Convert.ToHexString(md5.ComputeHash(fs)).ToLower();
    }

    public static string DeterministicUid(string name, int length = 7)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(name));
        var h = bytes.Aggregate<byte, long>(0, (current, b) => current * 256 + b);
        if (h < 0) h = -h;
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            sb.Append(chars[(int)(h % chars.Length)]);
            h /= chars.Length;
        }

        return sb.ToString();
    }

    public static bool WriteIfChanged(string path, byte[] data)
    {
        if (File.Exists(path) && File.ReadAllBytes(path).SequenceEqual(data)) return false;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, data);
        return true;
    }

    public static bool SaveImageIfChanged(Image img, string path)
    {
        using var buf = new MemoryStream();
        img.Save(buf, new PngEncoder());
        return WriteIfChanged(path, buf.ToArray());
    }

    public static int NextPowerOfTwo(int n)
    {
        var p = 1;
        while (p < n) p <<= 1;
        return p;
    }

    public static void WriteTres(string path, string atlasResPath, int x, int y, int w, int h,
        string name, int ml = 0, int mt = 0, int mr = 0, int mb = 0)
    {
        var marginLine = (ml | mt | mr | mb) != 0 ? $"margin = Rect2({ml}, {mt}, {mr}, {mb})\n" : "";
        var content =
            $"[gd_resource type=\"AtlasTexture\" load_steps=2 format=3 uid=\"uid://{DeterministicUid(name)}\"]\n" +
            $"[ext_resource type=\"Texture2D\" path=\"{atlasResPath}\" id=\"1\"]\n" +
            $"[resource]\natlas = ExtResource(\"1\")\nregion = Rect2({x}, {y}, {w}, {h})\n" +
            marginLine;
        WriteIfChanged(path, Encoding.UTF8.GetBytes(content));
    }
}