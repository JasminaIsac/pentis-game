using TetrisGame.Core;

namespace TetrisGame.Tests;

public class TetrominoTests
{
    public static TheoryData<TetrominoType> AllTypes { get; } =
    [
        TetrominoType.I, TetrominoType.O, TetrominoType.T,
        TetrominoType.S, TetrominoType.Z, TetrominoType.J, TetrominoType.L,
    ];

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void FourClockwiseRotations_RestoreInitialShape(TetrominoType type)
    {
        var piece = new Tetromino(type, 0, 0);
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
    public void FourCounterClockwiseRotations_RestoreInitialShape(TetrominoType type)
    {
        var piece = new Tetromino(type, 0, 0);
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
    public void GetCells_AlwaysHasExactlyFourCells(TetrominoType type)
    {
        var piece = new Tetromino(type, 2, 3);
        for (var rotation = 0; rotation < 4; rotation++)
        {
            Assert.Equal(4, piece.GetCells().Count);
            piece.RotateClockwise();
        }
    }

    [Fact]
    public void Clone_IsIndependentOfOriginal()
    {
        var original = new Tetromino(TetrominoType.T, 3, 1);
        var clone = original.Clone();

        clone.Move(1, 2);
        clone.RotateClockwise();
        clone.Shape[0, 0] = 9;

        Assert.Equal(3, original.X);
        Assert.Equal(1, original.Y);
        Assert.Equal(0, original.Rotation);
        Assert.Equal(0, original.Shape[0, 0]);
        Assert.Equal(1, original.Shape[0, 1]);
    }

    [Fact]
    public void Type_RemainsUnchangedAfterRotation()
    {
        var piece = new Tetromino(TetrominoType.J, 0, 0);
        piece.RotateClockwise();
        piece.RotateCounterClockwise();
        Assert.Equal(TetrominoType.J, piece.Type);
    }

    [Fact]
    public void IHorizontal_OccupiesSingleMatrixRow()
    {
        var piece = new Tetromino(TetrominoType.I, 0, 0);
        var occupiedRows = new HashSet<int>();
        for (var row = 0; row < 4; row++)
        {
            for (var col = 0; col < 4; col++)
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

    [Fact]
    public void O_RemainsUnchangedAfterRotation()
    {
        var piece = new Tetromino(TetrominoType.O, 0, 0);
        var before = CopyShape(piece.Shape);
        piece.RotateClockwise();
        AssertShapesEqual(before, piece.Shape);
    }

    [Fact]
    public void GetCells_ReflectPosition()
    {
        var piece = new Tetromino(TetrominoType.O, 4, 5);
        var cells = piece.GetCells().OrderBy(c => c.Y).ThenBy(c => c.X).ToArray();
        Assert.Equal([(4, 5), (5, 5), (4, 6), (5, 6)], cells);
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
