using System.Text;
using System.Text.Json.Nodes;
using ImageGen.Pipeline;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGen;

public class PackCards(string scriptDir, bool force)
    : ImagePipeline(scriptDir, ".cards_cache.json", force)
{
    private static readonly (int W, int H) NormalSize = (500, 380);
    private static readonly (int W, int H) AncientSize = (500, 702);

    private static readonly string[] InputSubdirs = ["cards", "cards_beta"];

    protected override JsonObject ProcessCharacter(string charId, string charProj, JsonObject charCache)
    {
        var outDir = Path.Join(Parent, charProj, "images", "atlases");
        var resBase = $"res://{charProj}/images/atlases";
        Directory.CreateDirectory(outDir);

        var seen = new HashSet<string>();
        var normalEntries = new List<(string Stem, Image<Rgb24> Img, string Hash)>();
        var ancientEntries = new List<(string Stem, Image<Rgb24> Img, string Hash)>();

        foreach (var sub in InputSubdirs)
        {
            var d = Path.Join(ImagesDir, sub);
            if (!Directory.Exists(d)) continue;

            foreach (var file in Directory.EnumerateFiles(d, "*.png").Order())
            {
                if (!seen.Add(Path.GetFileName(file))) continue;

                var stem = Path.GetFileNameWithoutExtension(file);
                var hash = Utils.FileHash(file);

                using var raw = Image.Load<Rgba32>(file);
                if (raw.Height > raw.Width)
                    ancientEntries.Add((stem, FlattenAlpha(raw, AncientSize), hash));
                else
                    normalEntries.Add((stem, FlattenAlpha(raw, NormalSize), hash));
            }
        }

        charCache["normal"] = PackGroup($"{charId}_normal", normalEntries, outDir, resBase, "card_atlas", NormalSize,
            charCache["normal"]?.AsObject() ?? new JsonObject());
        charCache["ancient"] = PackGroup($"{charId}_ancient", ancientEntries, outDir, resBase, "card_ancient_atlas",
            AncientSize, charCache["ancient"]?.AsObject() ?? new JsonObject());

        foreach (var list in new[] { normalEntries, ancientEntries })
            foreach (var (_, img, _) in list)
                img.Dispose();

        return charCache;
    }

    private JsonObject PackGroup(
        string groupId,
        List<(string Stem, Image<Rgb24> Img, string Hash)> entries,
        string outDir, string resBase, string atlasBase,
        (int W, int H) cardSize, JsonObject groupCache)
    {
        var (w, h) = cardSize;
        var packer = new SlotPacker(w, h);

        if (Force) groupCache = new JsonObject();

        var entryMap = entries.ToDictionary(e => e.Stem, e => (e.Img, e.Hash));
        var nextSlot = groupCache.Count == 0
            ? 0
            : groupCache.Select(kv => kv.Value!["slot"]!.GetValue<int>()).Max() + 1;

        var slotMap = new Dictionary<string, int>();
        var dirty = new HashSet<string>();

        foreach (var (stem, cached) in groupCache)
        {
            if (!entryMap.ContainsKey(stem)) continue;
            slotMap[stem] = cached!["slot"]!.GetValue<int>();
            if (cached["hash"]!.GetValue<string>() != entryMap[stem].Hash) dirty.Add(stem);
        }

        foreach (var stem in entryMap.Keys)
            if (!slotMap.ContainsKey(stem))
            {
                slotMap[stem] = nextSlot++;
                dirty.Add(stem);
            }

        var removed = groupCache.Select(kv => kv.Key).Except(entryMap.Keys).ToHashSet();

        if (slotMap.Count == 0)
        {
            foreach (var stem in removed)
            {
                var p = Path.Join(outDir, "card_atlas.sprites", $"{stem}.tres");
                if (!File.Exists(p)) continue;
                File.Delete(p);
                Console.WriteLine($"  removed: {stem}.tres");
            }

            Console.WriteLine($"  {groupId}: 0 cards, 0 page(s), 0 .tres updated");
            return new JsonObject();
        }

        if (!dirty.Any() && !removed.Any() && !Force)
        {
            Console.WriteLine($"  {groupId}: no changes");
            return groupCache;
        }

        var atlasCount = slotMap.Values.DefaultIfEmpty(0).Max() / packer.SlotsPerAtlas + 1;
        var canvases = Enumerable.Range(0, atlasCount)
            .Select(_ => new Image<Rgb24>(SlotPacker.MaxAtlas, SlotPacker.MaxAtlas, new Rgb24(0, 0, 0)))
            .ToList();

        foreach (var (stem, slot) in slotMap)
        {
            var (idx, x, y) = packer.SlotToPos(slot);
            canvases[idx].Mutate(ctx => ctx.DrawImage(entryMap[stem].Img, new Point(x, y), 1f));
        }

        var atlasResPaths = new List<string>();
        for (var i = 0; i < canvases.Count; i++)
        {
            var positions = slotMap.Values.Select(packer.SlotToPos).Where(p => p.Idx == i).ToList();
            var usedW = positions.Max(p => p.X + w);
            var usedH = positions.Max(p => p.Y + h);

            using var cropped = canvases[i].Clone(ctx => ctx.Crop(new Rectangle(0, 0, usedW, usedH)));
            var fname = $"{atlasBase}_{i}.png";
            atlasResPaths.Add($"{resBase}/{fname}");
            if (Utils.SaveImageIfChanged(cropped, Path.Join(outDir, fname)))
                Console.WriteLine($"  wrote: {fname}");
        }

        foreach (var c in canvases) c.Dispose();

        var tresDir = Path.Join(outDir, "card_atlas.sprites");
        Directory.CreateDirectory(tresDir);
        var tresWritten = 0;

        foreach (var (stem, slot) in slotMap)
        {
            var (idx, x, y) = packer.SlotToPos(slot);
            var cached = groupCache[stem];
            if (!dirty.Contains(stem) && cached?["atlas_idx"]?.GetValue<int>() == idx) continue;

            var uid = Utils.DeterministicUid($"{groupId}_{stem}");
            var content =
                $"[gd_resource type=\"AtlasTexture\" load_steps=2 format=3 uid=\"uid://{uid}\"]\n" +
                $"[ext_resource type=\"Texture2D\" path=\"{atlasResPaths[idx]}\" id=\"1\"]\n" +
                $"[resource]\natlas = ExtResource(\"1\")\nregion = Rect2({x}, {y}, {w}, {h})\n";
            Utils.WriteIfChanged(Path.Join(tresDir, $"{stem}.tres"), Encoding.UTF8.GetBytes(content));
            tresWritten++;
        }

        foreach (var stem in removed)
        {
            var p = Path.Join(tresDir, $"{stem}.tres");
            if (!File.Exists(p)) continue;
            File.Delete(p);
            Console.WriteLine($"  removed: {stem}.tres");
        }

        Console.WriteLine($"  {groupId}: {entryMap.Count} cards, {atlasCount} page(s), {tresWritten} .tres updated");

        return new JsonObject(slotMap.Select(kv =>
        {
            var (idx, _, _) = packer.SlotToPos(kv.Value);
            return KeyValuePair.Create<string, JsonNode?>(kv.Key, new JsonObject
            {
                ["hash"] = entryMap[kv.Key].Hash,
                ["slot"] = kv.Value,
                ["atlas_idx"] = idx
            });
        }));
    }

    private static Image<Rgb24> FlattenAlpha(Image<Rgba32> src, (int W, int H) size)
    {
        var dst = new Image<Rgb24>(size.W, size.H, new Rgb24(0, 0, 0));
        using var resized = src.Clone(ctx => ctx.Resize(size.W, size.H, KnownResamplers.Lanczos3));
        dst.Mutate(ctx => ctx.DrawImage(resized, new Point(0, 0), 1f));
        return dst;
    }
}