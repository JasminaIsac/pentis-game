namespace PentisGame.Core;

/// <summary>
/// Generator 12-bag: fiecare sac conține cele douăsprezece tipuri exact o dată.
/// </summary>
public sealed class PieceFactory
{
    private readonly Random _random;
    private readonly Queue<PentominoType> _queue = new();

    /// <summary>
    /// Creează fabrica. Testele pot pasa un <see cref="Random"/> cu seed fix.
    /// </summary>
    public PieceFactory(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }

    /// <summary>Golește coada; următoarea extragere începe un sac nou.</summary>
    public void Reset() => _queue.Clear();

    /// <summary>Scoate următorul tip din sac.</summary>
    public PentominoType GetNext()
    {
        Ensure(1);
        return _queue.Dequeue();
    }

    /// <summary>
    /// Arată următoarele <paramref name="count"/> tipuri fără să le scoată.
    /// </summary>
    public IReadOnlyList<PentominoType> Peek(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        Ensure(count);
        return _queue.Take(count).ToArray();
    }

    /// <summary>
    /// Construiește o piesă la rotația 0, Y = 0, X centrat pe lățimea puțului.
    /// </summary>
    public Pentomino Create(PentominoType type)
    {
        var size = PentominoCatalog.GetSize(type);
        var x = (GameConstants.BoardWidth - size) / 2;
        return new Pentomino(type, x, y: 0);
    }

    private void Ensure(int count)
    {
        while (_queue.Count < count)
        {
            foreach (var type in CreateShuffledBag())
            {
                _queue.Enqueue(type);
            }
        }
    }

    private PentominoType[] CreateShuffledBag()
    {
        var bag = Enum.GetValues<PentominoType>();
        for (var i = bag.Length - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }

        return bag;
    }
}
