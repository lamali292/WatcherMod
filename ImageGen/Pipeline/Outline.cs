using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGen.Pipeline;

public static class Outline
{
    /// <summary>Composites a black shadow outline beneath <paramref name="src" />.</summary>
    public static Image<Rgba32> ApplyOutline(Image<Rgba32> src, int radius, float sigma = 0.1f)
    {
        var blurred = BlurredOutlineMask(src, radius, sigma);

        var shadow = new Image<Rgba32>(src.Width, src.Height);
        shadow.ProcessPixelRows(acc =>
        {
            for (var y = 0; y < src.Height; y++)
            {
                var row = acc.GetRowSpan(y);
                for (var x = 0; x < src.Width; x++)
                    row[x] = new Rgba32(0, 0, 0, (byte)(blurred[y * src.Width + x] * 0.5f));
            }
        });

        shadow.Mutate(ctx => ctx.DrawImage(src, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcOver, 1f));
        return shadow;
    }

    /// <summary>Returns a solid-white image whose alpha is the outline mask (used by the relic outline atlas).</summary>
    public static Image<Rgba32> WhiteOutlineImage(Image<Rgba32> src, int radius, float sigma = 0.1f)
    {
        var blurred = BlurredOutlineMask(src, radius, sigma);

        var result = new Image<Rgba32>(src.Width, src.Height);
        result.ProcessPixelRows(acc =>
        {
            for (var y = 0; y < src.Height; y++)
            {
                var row = acc.GetRowSpan(y);
                for (var x = 0; x < src.Width; x++)
                    row[x] = new Rgba32(255, 255, 255, blurred[y * src.Width + x]);
            }
        });
        return result;
    }

    // ── Internals ─────────────────────────────────────────────────────────────

    private static byte[] BlurredOutlineMask(Image<Rgba32> src, int radius, float sigma)
    {
        var alpha = ExtractAlpha(src);
        var dilated = Dilate(alpha, src.Width, src.Height, radius);
        return GaussianBlur(dilated, src.Width, src.Height, sigma);
    }

    private static byte[] ExtractAlpha(Image<Rgba32> img)
    {
        var alpha = new byte[img.Width * img.Height];
        img.ProcessPixelRows(acc =>
        {
            for (var y = 0; y < img.Height; y++)
            {
                var row = acc.GetRowSpan(y);
                for (var x = 0; x < img.Width; x++)
                    alpha[y * img.Width + x] = row[x].A;
            }
        });
        return alpha;
    }

    private static byte[] Dilate(byte[] alpha, int width, int height, int radius)
    {
        var size = radius * 2 + 1;
        var kernel = new bool[size, size];
        for (var dy = 0; dy < size; dy++)
            for (var dx = 0; dx < size; dx++)
            {
                int ky = dy - radius, kx = dx - radius;
                kernel[dy, dx] = kx * kx + ky * ky <= radius * radius;
            }

        byte Clamp(int y, int x)
        {
            return alpha[Math.Clamp(y, 0, height - 1) * width + Math.Clamp(x, 0, width - 1)];
        }

        var dilated = new byte[width * height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                byte max = 0;
                for (var dy = 0; dy < size; dy++)
                    for (var dx = 0; dx < size; dx++)
                        if (kernel[dy, dx])
                            max = Math.Max(max, Clamp(y + dy - radius, x + dx - radius));
                dilated[y * width + x] = max;
            }

        return dilated;
    }

    private static byte[] GaussianBlur(byte[] data, int width, int height, float sigma)
    {
        var radius = Math.Max(1, (int)MathF.Ceiling(sigma * 3));
        var kernel = BuildKernel(sigma, radius);
        var tmp = new float[data.Length];
        var result = new float[data.Length];

        // Horizontal pass
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                float sum = 0;
                for (var k = -radius; k <= radius; k++)
                    sum += data[y * width + Math.Clamp(x + k, 0, width - 1)] * kernel[k + radius];
                tmp[y * width + x] = sum;
            }

        // Vertical pass
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                float sum = 0;
                for (var k = -radius; k <= radius; k++)
                    sum += tmp[Math.Clamp(y + k, 0, height - 1) * width + x] * kernel[k + radius];
                result[y * width + x] = sum;
            }

        return result.Select(v => (byte)Math.Clamp(v, 0f, 255f)).ToArray();
    }

    private static float[] BuildKernel(float sigma, int radius)
    {
        var k = new float[2 * radius + 1];
        float sum = 0;
        for (var i = -radius; i <= radius; i++)
        {
            k[i + radius] = MathF.Exp(-i * i / (2 * sigma * sigma));
            sum += k[i + radius];
        }

        for (var i = 0; i < k.Length; i++) k[i] /= sum;
        return k;
    }
}