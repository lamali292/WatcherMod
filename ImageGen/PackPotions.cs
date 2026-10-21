using System.Text.Json.Nodes;
using ImageGen.Pipeline;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGen;

public class PackPotions(string scriptDir, bool force)
    : ImagePipeline(scriptDir, ".potions_cache.json", force)
{
    private const int AtlasSize = 64;
    private const float ContentScale = 0.9f;
    private const string CustomSuffix = "";

    private const int OutlineRadiusAtlas = 3;
    private const float OutlineSigma = 0.1f;

    private static readonly string[] InputSubdirs = ["potions", "potions_beta"];

    protected override JsonObject ProcessCharacter(string charId, string charProj, JsonObject charCache)
    {
        var outAtlases = Path.Join(Parent, charProj, "images", "atlases");
        var outTres = Path.Join(outAtlases, "potion_atlas.sprites");
        var resBase = $"res://{charProj}/images/atlases";
        var atlasResPath = $"{resBase}/potion_atlas.png";
        var outlineResPath = $"{resBase}/potion_outline_atlas.png";

        foreach (var d in new[] { outAtlases, outTres })
            Directory.CreateDirectory(d);

        var seen = new HashSet<string>();
        var inputFiles = (from sub in InputSubdirs
            select Path.Join(ImagesDir, sub)
            into d
            where Directory.Exists(d)
            from file in Directory.EnumerateFiles(d, "*.png").Order()
            where seen.Add(Path.GetFileName(file))
            select file).ToList();

        if (inputFiles.Count == 0)
        {
            Console.WriteLine("  no images found");
            return charCache;
        }

        var currentHashes = inputFiles.ToDictionary(f => Path.GetFileName(f)!, Utils.FileHash);
        var outputsExist =
            File.Exists(Path.Join(outAtlases, "potion_atlas.png")) &&
            File.Exists(Path.Join(outAtlases, "potion_outline_atlas.png"));

        if (!Force && outputsExist && charCache["input_hashes"] is JsonObject cachedHashes &&
            currentHashes.All(kv => cachedHashes[kv.Key]?.GetValue<string>() == kv.Value))
        {
            Console.WriteLine("  nothing changed, skipping");
            return charCache;
        }

        var entries = new List<(string Stem, Image<Rgba32> Small, Image<Rgba32> Outline)>();
        foreach (var file in inputFiles)
        {
            var stem = Path.GetFileNameWithoutExtension(file);
            using var raw = Image.Load<Rgba32>(file);

            var small = ScaleCentered(raw, AtlasSize);

            var outlineFull = Outline.WhiteOutlineImage(small, OutlineRadiusAtlas);
            var outline = outlineFull.Clone(ctx => ctx.Resize(AtlasSize, AtlasSize, KnownResamplers.Lanczos3));
            outlineFull.Dispose();

            entries.Add((stem, small, outline));
        }

        var atlasData = entries.Select(e => (e.Stem, (Image)e.Small)).ToList();
        var outlineData = entries.Select(e => (e.Stem, (Image)e.Outline)).ToList();

        var (atlasPacker, atlasPlaces) = ShelfPackAll.Pack(atlasData);
        var (outlinePacker, outlinePlaces) = ShelfPackAll.Pack(outlineData);

        int aw = Utils.NextPowerOfTwo(atlasPacker.CanvasSize.Width), ah = atlasPacker.CanvasSize.Height;
        int ow = Utils.NextPowerOfTwo(outlinePacker.CanvasSize.Width), oh = outlinePacker.CanvasSize.Height;

        using var atlas = new Image<Rgba32>(aw, ah);
        using var outlineAtlas = new Image<Rgba32>(ow, oh);

        for (var i = 0; i < entries.Count; i++)
        {
            var (stem, small, outline) = entries[i];
            var (ax, ay) = atlasPlaces[i];
            var (ox, oy) = outlinePlaces[i];
            var tresName = $"{stem}{CustomSuffix}";

            atlas.Mutate(ctx => ctx.DrawImage(small, new Point(ax, ay), 1f));
            outlineAtlas.Mutate(ctx => ctx.DrawImage(outline, new Point(ox, oy), 1f));

            Utils.WriteTres(Path.Join(outTres, $"{tresName}.tres"),
                atlasResPath, ax, ay, small.Width, small.Height, $"{charId}_{tresName}_atlas");
            Utils.WriteTres(Path.Join(outTres, $"{tresName}_outline.tres"),
                outlineResPath, ox, oy, outline.Width, outline.Height, $"{charId}_{tresName}_outline");
        }

        Utils.SaveImageIfChanged(atlas, Path.Join(outAtlases, "potion_atlas.png"));
        Utils.SaveImageIfChanged(outlineAtlas, Path.Join(outAtlases, "potion_outline_atlas.png"));
        Console.WriteLine($"  {entries.Count} potions packed");

        foreach (var (_, small, outline) in entries)
        {
            small.Dispose();
            outline.Dispose();
        }

        charCache["input_hashes"] = new JsonObject(
            currentHashes.Select(kv => KeyValuePair.Create<string, JsonNode?>(kv.Key, kv.Value)));

        return charCache;
    }

    private static Image<Rgba32> ScaleCentered(Image<Rgba32> src, int full)
    {
        var inner = Math.Max(1, (int)MathF.Round(full * ContentScale));
        return src.Clone(ctx => ctx
            .Resize(inner, inner, KnownResamplers.Lanczos3)
            .Resize(new ResizeOptions
            {
                Size = new Size(full, full),
                Mode = ResizeMode.BoxPad,
                Position = AnchorPositionMode.Center
            }));
    }
}