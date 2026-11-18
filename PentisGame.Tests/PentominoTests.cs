using PentisGame.Core;

namespace PentisGame.Tests;

public class PentominoTests
{
    public static TheoryData<PentominoType> AllTypes { get; } =
    [
        PentominoType.F, PentominoType.I, PentominoType.L, PentominoType.N,
        PentominoType.P, PentominoType.T, PentominoType.U, PentominoType.V,
        PentominoType.W, PentominoType.X, PentominoType.Y, PentominoType.Z,
    ];

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void FourClockwiseRotations_RestoreInitialShape(PentominoType type)
    {
        var piece = new Pentomino(type, 0, 0);
        var initial = CopyShape(piece.Shape);
        for (var i = 0; i < 4; i++)
        {
            piece.RotateClockwise();
        }

        Assert.Equal(0, piece.Rotation);
        AssertShapesEqual(initial, piece.Shape);
        Assert.Equal(type, piece.Type);
    }

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void FourCounterClockwiseRotations_RestoreInitialShape(PentominoType type)
    {
        var piece = new Pentomino(type, 0, 0);
        var initial = CopyShape(piece.Shape);
        for (var i = 0; i < 4; i++)
        {
            piece.RotateCounterClockwise();
        }

        Assert.Equal(0, piece.Rotation);
        AssertShapesEqual(initial, piece.Shape);
    }

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void GetCells_AlwaysHasExactlyFiveCells(PentominoType type)
    {
        var piece = new Pentomino(type, 2, 3);
        for (var rotation = 0; rotation < 4; rotation++)
        {
            Assert.Equal(5, piece.GetCells().Count);
            piece.RotateClockwise();
        }
    }

    [Fact]
    public void Clone_IsIndependentOfOriginal()
    {
        var original = new Pentomino(PentominoType.T, 3, 1);
        var clone = original.Clone();
        clone.Move(1, 2);
        clone.RotateClockwise();
        clone.Shape[0, 0] = 9;

        Assert.Equal(3, original.X);
        Assert.Equal(1, original.Y);
        Assert.Equal(0, original.Rotation);
        Assert.Equal(1, original.Shape[0, 0]);
    }

    [Fact]
    public void Type_RemainsUnchangedAfterRotation()
    {
        var piece = new Pentomino(PentominoType.L, 0, 0);
        piece.RotateClockwise();
        piece.RotateCounterClockwise();
        Assert.Equal(PentominoType.L, piece.Type);
    }

    [Fact]
    public void IHorizontal_OccupiesSingleMatrixRow()
    {
        var piece = new Pentomino(PentominoType.I, 0, 0);
        var occupiedRows = new HashSet<int>();
        for (var row = 0; row < piece.Shape.GetLength(0); row++)
        {
            for (var col = 0; col < piece.Shape.GetLength(1); col++)
            {
                if (piece.Shape[row, col] != 0)
                {
                    occupiedRows.Add(row);
                }
            }
        }

        Assert.Single(occupiedRows);
        Assert.Equal(1, occupiedRows.First());
    }

    private static int[,] CopyShape(int[,] shape)
    {
        var copy = new int[shape.GetLength(0), shape.GetLength(1)];
        Array.Copy(shape, copy, shape.Length);
        return copy;
    }

    private static void AssertShapesEqual(int[,] expected, int[,] actual)
    {
        Assert.Equal(expected.GetLength(0), actual.GetLength(0));
        Assert.Equal(expected.GetLength(1), actual.GetLength(1));
        for (var row = 0; row < expected.GetLength(0); row++)
        {
            for (var col = 0; col < expected.GetLength(1); col++)
            {
                Assert.Equal(expected[row, col], actual[row, col]);
            }
        }
    }
}
