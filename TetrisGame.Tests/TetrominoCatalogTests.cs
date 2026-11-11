using TetrisGame.Core;

namespace TetrisGame.Tests;

public class TetrominoCatalogTests
{
    public static TheoryData<TetrominoType> AllTypes { get; } =
    [
        TetrominoType.I,
        TetrominoType.O,
        TetrominoType.T,
        TetrominoType.S,
        TetrominoType.Z,
        TetrominoType.J,
        TetrominoType.L,
    ];

    [Fact]
    public void TetrominoType_ContainsExactlySevenValues()
    {
        Assert.Equal(7, Enum.GetValues<TetrominoType>().Length);
    }

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void GetColorId_IsUniqueAndDifferentFromEmptyCell(TetrominoType type)
    {
        var colorId = TetrominoCatalog.GetColorId(type);

        Assert.NotEqual(TetrominoCatalog.EmptyCell, colorId);
        Assert.Equal((int)type + 1, colorId);
    }

    [Fact]
    public void GetColorId_ValuesAreAllDistinct()
    {
        var ids = Enum.GetValues<TetrominoType>()
            .Select(TetrominoCatalog.GetColorId)
            .ToArray();

        Assert.Equal(ids.Length, ids.Distinct().Count());
        Assert.Equal([1, 2, 3, 4, 5, 6, 7], ids.OrderBy(id => id));
    }

    [Theory]
    [InlineData(TetrominoType.I, 4)]
    [InlineData(TetrominoType.O, 2)]
    [InlineData(TetrominoType.T, 3)]
    [InlineData(TetrominoType.S, 3)]
    [InlineData(TetrominoType.Z, 3)]
    [InlineData(TetrominoType.J, 3)]
    [InlineData(TetrominoType.L, 3)]
    public void GetSize_MatchesMatrixSide(TetrominoType type, int expectedSize)
    {
        Assert.Equal(expectedSize, TetrominoCatalog.GetSize(type));

        var shape = TetrominoCatalog.GetShape(type);
        Assert.Equal(expectedSize, shape.GetLength(0));
        Assert.Equal(expectedSize, shape.GetLength(1));
    }

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void GetShape_HasExactlyFourOccupiedCells(TetrominoType type)
    {
        var shape = TetrominoCatalog.GetShape(type);
        var occupied = 0;

        for (var row = 0; row < shape.GetLength(0); row++)
        {
            for (var col = 0; col < shape.GetLength(1); col++)
            {
                if (shape[row, col] != 0)
                {
                    occupied++;
                }
            }
        }

        Assert.Equal(4, occupied);
    }

    [Fact]
    public void GetShape_MatchesRotationZeroLayouts()
    {
        AssertShape(TetrominoType.I,
        [
            [0, 0, 0, 0],
            [1, 1, 1, 1],
            [0, 0, 0, 0],
            [0, 0, 0, 0],
        ]);
        AssertShape(TetrominoType.O,
        [
            [1, 1],
            [1, 1],
        ]);
        AssertShape(TetrominoType.T,
        [
            [0, 1, 0],
            [1, 1, 1],
            [0, 0, 0],
        ]);
        AssertShape(TetrominoType.S,
        [
            [0, 1, 1],
            [1, 1, 0],
            [0, 0, 0],
        ]);
        AssertShape(TetrominoType.Z,
        [
            [1, 1, 0],
            [0, 1, 1],
            [0, 0, 0],
        ]);
        AssertShape(TetrominoType.J,
        [
            [1, 0, 0],
            [1, 1, 1],
            [0, 0, 0],
        ]);
        AssertShape(TetrominoType.L,
        [
            [0, 0, 1],
            [1, 1, 1],
            [0, 0, 0],
        ]);
    }

    [Fact]
    public void GetShape_ReturnsIndependentCopy()
    {
        var first = TetrominoCatalog.GetShape(TetrominoType.T);
        first[0, 1] = 0;

        var second = TetrominoCatalog.GetShape(TetrominoType.T);

        Assert.Equal(1, second[0, 1]);
    }

    [Fact]
    public void Core_DoesNotReferenceDrawingOrWindowsForms()
    {
        var coreAssembly = typeof(TetrominoCatalog).Assembly;
        var referencedNames = coreAssembly.GetReferencedAssemblies().Select(name => name.Name);

        Assert.DoesNotContain("System.Drawing", referencedNames);
        Assert.DoesNotContain("System.Windows.Forms", referencedNames);

        var sourceRoot = FindCoreSourceRoot();
        var sources = Directory.GetFiles(sourceRoot, "*.cs", SearchOption.AllDirectories);
        Assert.All(sources, path =>
        {
            var text = File.ReadAllText(path);
            Assert.DoesNotContain("System.Drawing", text);
            Assert.DoesNotContain("System.Windows.Forms", text);
            Assert.DoesNotContain("Color.", text);
        });
    }

    private static void AssertShape(TetrominoType type, int[][] expected)
    {
        var shape = TetrominoCatalog.GetShape(type);
        Assert.Equal(expected.Length, shape.GetLength(0));
        Assert.Equal(expected[0].Length, shape.GetLength(1));

        for (var row = 0; row < expected.Length; row++)
        {
            for (var col = 0; col < expected[row].Length; col++)
            {
                Assert.Equal(expected[row][col], shape[row, col]);
            }
        }
    }

    private static string FindCoreSourceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "TetrisGame.Core");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Nu am găsit sursele TetrisGame.Core.");
    }
}
