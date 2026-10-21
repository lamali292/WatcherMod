using System.Text.Json.Nodes;
using ImageGen.Pipeline;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGen;

public class PackPowers(string scriptDir, bool force)
    : ImagePipeline(scriptDir, ".powers_cache.json", force)
{
    private const int BigSize = 256;
    private const int AtlasSize = 64;
    private const int SpriteSize = 24;
    private const float ContentScale = 0.9f;
    private const string CustomSuffix = "";

    private const int OutlineRadiusBig = 10;
    private const int OutlineRadiusAtlas = 3;
    private const float OutlineSigma = 0.1f;

    private static readonly string[] InputSubdirs = ["powers", "powers_beta"];

    protected override JsonObject ProcessCharacter(string charId, string charProj, JsonObject charCache)
    {
        var outAtlases = Path.Join(Parent, charProj, "images", "atlases");
        var outPowers = Path.Join(Parent, charProj, "images", "powers");
        var outTres = Path.Join(outAtlases, "power_atlas.sprites");
        var outTresSprite = Path.Join(outAtlases, "power_sprite_atlas.sprites");
        var resBase = $"res://{charProj}/images/atlases";
        var atlasResPath = $"{resBase}/power_atlas.png";
        var spriteResPath = $"{resBase}/power_sprite_atlas.png";

        foreach (var d in new[] { outAtlases, outPowers, outTres, outTresSprite })
            Directory.CreateDirectory(d);

        // Collect input files — subdirs in priority order, first-seen filename wins
        var seen = new HashSet<string>();
        var inputFiles = new List<string>();
        foreach (var sub in InputSubdirs)
        {
            var d = Path.Join(ImagesDir, sub);
            if (!Directory.Exists(d)) continue;
            foreach (var file in Directory.EnumerateFiles(d, "*.png").Order())
                if (seen.Add(Path.GetFileName(file)))
                    inputFiles.Add(file);
        }

        if (inputFiles.Count == 0)
        {
            Console.WriteLine("  no images found");
            return charCache;
        }

        // Skip if nothing changed
        var currentHashes = inputFiles.ToDictionary(f => Path.GetFileName(f)!, Utils.FileHash);
        var outputsExist =
            File.Exists(Path.Join(outAtlases, "power_atlas.png")) &&
            File.Exists(Path.Join(outAtlases, "power_sprite_atlas.png"));

        if (!Force && outputsExist && charCache["input_hashes"] is JsonObject cachedHashes &&
            currentHashes.All(kv => cachedHashes[kv.Key]?.GetValue<string>() == kv.Value))
        {
            Console.WriteLine("  nothing changed, skipping");
            return charCache;
        }

        var entries = new List<(string Stem, Image<Rgba32> Big, Image<Rgba32> Small, Image<Rgba32> Sprite)>();
        foreach (var file in inputFiles)
            try
            {
                var stem = Path.GetFileNameWithoutExtension(file);
                using var raw = Image.Load<Rgba32>(file);
                var big = Outline.ApplyOutline(ScaleCentered(raw, BigSize), OutlineRadiusBig);
                var small = Outline.ApplyOutline(ScaleCentered(raw, AtlasSize), OutlineRadiusAtlas);
                var sprite = ScaleCentered(raw, SpriteSize);
                entries.Add((stem, big, small, sprite));
            }
            catch (Exception e)
            {
                Console.WriteLine(file);
                Console.WriteLine(e);
            }

        var atlasData = entries.Select(e => (e.Stem, (Image)e.Small)).ToList();
        var spriteData = entries.Select(e => (e.Stem, (Image)e.Sprite)).ToList();

        var (atlasPacker, atlasPlaces) = ShelfPackAll.Pack(atlasData);
        var (spritePacker, spritePlaces) = ShelfPackAll.Pack(spriteData);

        int aw = Utils.NextPowerOfTwo(atlasPacker.CanvasSize.Width), ah = atlasPacker.CanvasSize.Height;
        int sw = Utils.NextPowerOfTwo(spritePacker.CanvasSize.Width), sh = spritePacker.CanvasSize.Height;

        using var atlas = new Image<Rgba32>(aw, ah);
        using var spriteAtlas = new Image<Rgba32>(sw, sh);

        for (var i = 0; i < entries.Count; i++)
        {
            var (stem, big, small, _) = entries[i];
            var (ax, ay) = atlasPlaces[i];
            var tresName = $"{stem}{CustomSuffix}";

            atlas.Mutate(ctx => ctx.DrawImage(small, new Point(ax, ay), 1f));
            Utils.WriteTres(Path.Join(outTres, $"{tresName}.tres"),
                atlasResPath, ax, ay, small.Width, small.Height, $"{charId}_{tresName}_atlas");

            if (Utils.SaveImageIfChanged(big, Path.Join(outPowers, $"{tresName}.png")))
                Console.WriteLine($"  updated big: {tresName}");
        }

        for (var i = 0; i < entries.Count; i++)
        {
            var (stem, _, _, sprite) = entries[i];
            var (sx, sy) = spritePlaces[i];
            var tresName = $"{stem}{CustomSuffix}";

            spriteAtlas.Mutate(ctx => ctx.DrawImage(sprite, new Point(sx, sy), 1f));
            Utils.WriteTres(Path.Join(outTresSprite, $"{tresName}.tres"),
                spriteResPath, sx, sy, sprite.Width, sprite.Height, $"{charId}_{tresName}_sprite");
        }

        Utils.SaveImageIfChanged(atlas, Path.Join(outAtlases, "power_atlas.png"));
        Utils.SaveImageIfChanged(spriteAtlas, Path.Join(outAtlases, "power_sprite_atlas.png"));
        Console.WriteLine($"  {entries.Count} powers packed");

        foreach (var (_, big, small, sprite) in entries)
        {
            big.Dispose();
            small.Dispose();
            sprite.Dispose();
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