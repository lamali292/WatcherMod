using SixLabors.ImageSharp;

namespace ImageGen.Pipeline;

/// <summary>Variable-size bin packer using the shelf algorithm (for powers and relics).</summary>
public class ShelfPacker(int width)
{
    private readonly List<Shelf> _shelves = [];
    private int _height;

    public (int Width, int Height) CanvasSize => (width, _height);

    public (int X, int Y) Pack(int w, int h)
    {
        Shelf? best = null;
        var bestWaste = int.MaxValue;

        foreach (var shelf in _shelves)
            if (h <= shelf.Height && shelf.NextX + w <= width)
            {
                var waste = shelf.Height - h;
                if (waste >= bestWaste) continue;
                best = shelf;
                bestWaste = waste;
            }

        if (best is not null)
        {
            var x = best.NextX;
            best.NextX += w + 1;
            return (x, best.Y);
        }

        var newShelf = new Shelf(_height) { NextX = w + 1, Height = h };
        _shelves.Add(newShelf);
        _height += h + 1;
        return (0, newShelf.Y);
    }

    private record Shelf(int Y)
    {
        public int Height;
        public int NextX;
    }
}

/// <summary>Fixed-size slot packer for card atlases.</summary>
public class SlotPacker(int w, int h, int padding = 1)
{
    public const int MaxAtlas = 4096;

    private int Cols => MaxAtlas / (w + padding);
    private int Rows => MaxAtlas / (h + padding);
    public int SlotsPerAtlas => Cols * Rows;

    /// <summary>Returns (atlasIndex, x, y) for a given slot number.</summary>
    public (int Idx, int X, int Y) SlotToPos(int slot)
    {
        var spa = SlotsPerAtlas;
        var idx = slot / spa;
        var loc = slot % spa;
        return (idx, loc % Cols * (w + padding), loc / Cols * (h + padding));
    }
}

/// <summary>Packs a list of (key, image) pairs using ShelfPacker, doubling width until it fits.</summary>
public static class ShelfPackAll
{
    public static (ShelfPacker Packer, List<(int X, int Y)> Placements)
        Pack<T>(IReadOnlyList<(T Key, Image Img)> data)
    {
        var totalArea = data.Sum(d => (d.Img.Width + 1) * (d.Img.Height + 1));
        var estSide = Math.Max(Utils.NextPowerOfTwo((int)(Math.Sqrt(totalArea) * 1.2)), 64);
        var order = Enumerable.Range(0, data.Count).OrderByDescending(i => data[i].Img.Height).ToArray();

        ShelfPacker packer;
        List<(int X, int Y)> placements;

        for (var attempt = 0; attempt < 4; attempt++)
        {
            packer = new ShelfPacker(estSide);
            placements = Enumerable.Repeat((0, 0), data.Count).ToList();
            foreach (var i in order)
                placements[i] = packer.Pack(data[i].Img.Width, data[i].Img.Height);
            if (packer.CanvasSize.Height <= estSide * 2)
                return (packer, placements);
            estSide <<= 1;
        }

        // Fallback — return whatever we have
        packer = new ShelfPacker(estSide);
        placements = Enumerable.Repeat((0, 0), data.Count).ToList();
        foreach (var i in order)
            placements[i] = packer.Pack(data[i].Img.Width, data[i].Img.Height);
        return (packer, placements);
    }
}