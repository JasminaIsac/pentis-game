namespace TetrisGame.Core;

/// <summary>
/// Formele de rotație 0 și identificatorii de culoare pentru cele șapte piese.
/// Culoarea desenată se alege în interfață pe baza identificatorului.
/// </summary>
public static class TetrominoCatalog
{
    /// <summary>Valoarea unei celule libere în puț.</summary>
    public const int EmptyCell = 0;

    /// <summary>
    /// Întoarce identificatorul numeric de culoare al tipului.
    /// Diferit de <see cref="EmptyCell"/> și unic pentru fiecare tip.
    /// </summary>
    public static int GetColorId(TetrominoType type) => (int)type + 1;

    /// <summary>
    /// Întoarce o copie a matricei de rotație 0 pentru tipul dat.
    /// Celulele ocupate au valoarea 1; cele libere au valoarea 0.
    /// </summary>
    public static int[,] GetShape(TetrominoType type)
    {
        var source = GetShapeSource(type);
        var size = source.GetLength(0);
        var copy = new int[size, size];
        Array.Copy(source, copy, source.Length);
        return copy;
    }

    /// <summary>
    /// Întoarce latura matricei de rotație 0 (2 pentru O, 4 pentru I, 3 pentru restul).
    /// </summary>
    public static int GetSize(TetrominoType type) => type switch
    {
        TetrominoType.I => 4,
        TetrominoType.O => 2,
        _ => 3,
    };

    private static int[,] GetShapeSource(TetrominoType type) => type switch
    {
        TetrominoType.I => Shapes.I,
        TetrominoType.O => Shapes.O,
        TetrominoType.T => Shapes.T,
        TetrominoType.S => Shapes.S,
        TetrominoType.Z => Shapes.Z,
        TetrominoType.J => Shapes.J,
        TetrominoType.L => Shapes.L,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Tip de piesă necunoscut."),
    };

    private static class Shapes
    {
        // I pe 4×4: rândul 1 complet ocupat
        public static readonly int[,] I =
        {
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
        };

        // O pe 2×2
        public static readonly int[,] O =
        {
            { 1, 1 },
            { 1, 1 },
        };

        // T pe 3×3
        public static readonly int[,] T =
        {
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        };

        // S pe 3×3
        public static readonly int[,] S =
        {
            { 0, 1, 1 },
            { 1, 1, 0 },
            { 0, 0, 0 },
        };

        // Z pe 3×3
        public static readonly int[,] Z =
        {
            { 1, 1, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 },
        };

        // J pe 3×3
        public static readonly int[,] J =
        {
            { 1, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        };

        // L pe 3×3
        public static readonly int[,] L =
        {
            { 0, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        };
    }
}
