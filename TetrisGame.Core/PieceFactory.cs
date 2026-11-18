namespace TetrisGame.Core;

/// <summary>
/// Generator 7-bag: fiecare sac conține cele șapte tipuri exact o dată.
/// </summary>
public sealed class PieceFactory
{
    private readonly Random _random;
    private readonly Queue<TetrominoType> _queue = new();

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
    public TetrominoType GetNext()
    {
        Ensure(1);
        return _queue.Dequeue();
    }

    /// <summary>
    /// Arată următoarele <paramref name="count"/> tipuri fără să le scoată.
    /// </summary>
    public IReadOnlyList<TetrominoType> Peek(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        Ensure(count);
        return _queue.Take(count).ToArray();
    }

    /// <summary>
    /// Construiește o piesă la rotația 0, Y = 0, X centrat pe lățimea puțului.
    /// </summary>
    public Tetromino Create(TetrominoType type)
    {
        var size = TetrominoCatalog.GetSize(type);
        var x = (GameConstants.BoardWidth - size) / 2;
        return new Tetromino(type, x, y: 0);
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

    private TetrominoType[] CreateShuffledBag()
    {
        var bag = Enum.GetValues<TetrominoType>();
        for (var i = bag.Length - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }

        return bag;
    }
}
