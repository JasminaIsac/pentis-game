namespace PentisGame.Core;

/// <summary>
/// Piesă pentomino cu tip, formă, rotație și poziție. Independentă de puț.
/// </summary>
public sealed class Pentomino
{
    /// <summary>
    /// Creează o piesă la rotația 0, cu forma din catalog.
    /// </summary>
    public Pentomino(PentominoType type, int x, int y)
    {
        Type = type;
        X = x;
        Y = y;
        Rotation = 0;
        Shape = PentominoCatalog.GetShape(type);
    }

    private Pentomino(PentominoType type, int x, int y, int rotation, int[,] shape)
    {
        Type = type;
        X = x;
        Y = y;
        Rotation = rotation;
        Shape = shape;
    }

    /// <summary>Tipul piesei; nu se schimbă la rotație.</summary>
    public PentominoType Type { get; }

    /// <summary>Matricea curentă a formei. Nu este matricea catalogului.</summary>
    public int[,] Shape { get; private set; }

    /// <summary>Coloana colțului stânga-sus, în coordonate de puț.</summary>
    public int X { get; private set; }

    /// <summary>Rândul colțului stânga-sus, în coordonate de puț.</summary>
    public int Y { get; private set; }

    /// <summary>Rotația curentă: 0, 1, 2 sau 3.</summary>
    public int Rotation { get; private set; }

    /// <summary>Mută piesa cu deplasarea dată.</summary>
    public void Move(int dx, int dy)
    {
        X += dx;
        Y += dy;
    }

    /// <summary>
    /// Rotire în sens orar: transpunere, apoi inversarea fiecărui rând.
    /// </summary>
    public void RotateClockwise()
    {
        Shape = RotateMatrixClockwise(Shape);
        Rotation = (Rotation + 1) % 4;
    }

    /// <summary>
    /// Rotire în sens antiorar: inversul rotației orare.
    /// </summary>
    public void RotateCounterClockwise()
    {
        Shape = RotateMatrixCounterClockwise(Shape);
        Rotation = (Rotation + 3) % 4;
    }

    /// <summary>Copie cu matrice proprie.</summary>
    public Pentomino Clone()
    {
        var size = Shape.GetLength(0);
        var copy = new int[size, size];
        Array.Copy(Shape, copy, Shape.Length);
        return new Pentomino(Type, X, Y, Rotation, copy);
    }

    /// <summary>
    /// Cele cinci celule ocupate, ca perechi (X + coloană, Y + rând).
    /// </summary>
    public IReadOnlyList<(int X, int Y)> GetCells()
    {
        var cells = new List<(int X, int Y)>(5);
        var size = Shape.GetLength(0);
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                if (Shape[row, col] != 0)
                {
                    cells.Add((X + col, Y + row));
                }
            }
        }

        return cells;
    }

    private static int[,] RotateMatrixClockwise(int[,] source)
    {
        var size = source.GetLength(0);
        var result = new int[size, size];
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                result[col, size - 1 - row] = source[row, col];
            }
        }

        return result;
    }

    private static int[,] RotateMatrixCounterClockwise(int[,] source)
    {
        var size = source.GetLength(0);
        var result = new int[size, size];
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                result[size - 1 - col, row] = source[row, col];
            }
        }

        return result;
    }
}
