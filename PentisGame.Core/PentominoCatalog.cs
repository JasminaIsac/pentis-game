namespace PentisGame.Core;

/// <summary>
/// Formele de rotație 0 și identificatorii de culoare pentru cele douăsprezece pentomino.
/// Culoarea desenată se alege în interfață pe baza identificatorului.
/// </summary>
public static class PentominoCatalog
{
    /// <summary>Valoarea unei celule libere în puț.</summary>
    public const int EmptyCell = 0;

    /// <summary>
    /// Întoarce identificatorul numeric de culoare al tipului.
    /// Diferit de <see cref="EmptyCell"/> și unic pentru fiecare tip.
    /// </summary>
    public static int GetColorId(PentominoType type) => (int)type + 1;

    /// <summary>
    /// Întoarce o copie a matricei de rotație 0 pentru tipul dat.
    /// Celulele ocupate au valoarea 1; cele libere au valoarea 0.
    /// </summary>
    public static int[,] GetShape(PentominoType type)
    {
        var source = GetShapeSource(type);
        var size = source.GetLength(0);
        var copy = new int[size, size];
        Array.Copy(source, copy, source.Length);
        return copy;
    }

    /// <summary>
    /// Întoarce latura matricei de rotație 0.
    /// </summary>
    public static int GetSize(PentominoType type) => GetShapeSource(type).GetLength(0);

    private static int[,] GetShapeSource(PentominoType type) => type switch
    {
        PentominoType.F => Shapes.F,
        PentominoType.I => Shapes.I,
        PentominoType.L => Shapes.L,
        PentominoType.N => Shapes.N,
        PentominoType.P => Shapes.P,
        PentominoType.T => Shapes.T,
        PentominoType.U => Shapes.U,
        PentominoType.V => Shapes.V,
        PentominoType.W => Shapes.W,
        PentominoType.X => Shapes.X,
        PentominoType.Y => Shapes.Y,
        PentominoType.Z => Shapes.Z,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Tip de piesă necunoscut."),
    };

    private static class Shapes
    {
        public static readonly int[,] F =
        {
            { 0, 1, 1 },
            { 1, 1, 0 },
            { 0, 1, 0 },
        };

        public static readonly int[,] I =
        {
            { 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };

        public static readonly int[,] L =
        {
            { 1, 0, 0, 0 },
            { 1, 0, 0, 0 },
            { 1, 0, 0, 0 },
            { 1, 1, 0, 0 },
        };

        public static readonly int[,] N =
        {
            { 0, 1, 0, 0 },
            { 0, 1, 0, 0 },
            { 1, 1, 0, 0 },
            { 1, 0, 0, 0 },
        };

        public static readonly int[,] P =
        {
            { 1, 1, 0 },
            { 1, 1, 0 },
            { 1, 0, 0 },
        };

        public static readonly int[,] T =
        {
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
        };

        public static readonly int[,] U =
        {
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        };

        public static readonly int[,] V =
        {
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static readonly int[,] W =
        {
            { 1, 0, 0 },
            { 1, 1, 0 },
            { 0, 1, 1 },
        };

        public static readonly int[,] X =
        {
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 },
        };

        public static readonly int[,] Y =
        {
            { 0, 1, 0, 0 },
            { 1, 1, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 1, 0, 0 },
        };

        public static readonly int[,] Z =
        {
            { 1, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 1 },
        };
    }
}
