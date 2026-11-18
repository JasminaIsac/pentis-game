using PentisGame.Core;

namespace PentisGame.Tests;

public class PentominoCatalogTests
{
    public static TheoryData<PentominoType> AllTypes { get; } =
    [
        PentominoType.F, PentominoType.I, PentominoType.L, PentominoType.N,
        PentominoType.P, PentominoType.T, PentominoType.U, PentominoType.V,
        PentominoType.W, PentominoType.X, PentominoType.Y, PentominoType.Z,
    ];

    [Fact]
    public void PentominoType_ContainsExactlyTwelveValues()
    {
        Assert.Equal(12, Enum.GetValues<PentominoType>().Length);
    }

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void GetColorId_IsUniqueAndDifferentFromEmptyCell(PentominoType type)
    {
        var colorId = PentominoCatalog.GetColorId(type);
        Assert.NotEqual(PentominoCatalog.EmptyCell, colorId);
        Assert.Equal((int)type + 1, colorId);
    }

    [Fact]
    public void GetColorId_ValuesAreAllDistinct()
    {
        var ids = Enum.GetValues<PentominoType>()
            .Select(PentominoCatalog.GetColorId)
            .ToArray();

        Assert.Equal(ids.Length, ids.Distinct().Count());
        Assert.Equal(Enumerable.Range(1, 12), ids.OrderBy(id => id));
    }

    [Theory]
    [MemberData(nameof(AllTypes))]
    public void GetShape_HasExactlyFiveOccupiedCells(PentominoType type)
    {
        var shape = PentominoCatalog.GetShape(type);
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

        Assert.Equal(5, occupied);
        Assert.Equal(shape.GetLength(0), PentominoCatalog.GetSize(type));
    }

    [Fact]
    public void GetShape_ReturnsIndependentCopy()
    {
        var first = PentominoCatalog.GetShape(PentominoType.T);
        first[0, 0] = 0;
        var second = PentominoCatalog.GetShape(PentominoType.T);
        Assert.Equal(1, second[0, 0]);
    }

    [Fact]
    public void Core_DoesNotReferenceDrawingOrWindowsForms()
    {
        var coreAssembly = typeof(PentominoCatalog).Assembly;
        var referencedNames = coreAssembly.GetReferencedAssemblies().Select(name => name.Name);
        Assert.DoesNotContain("System.Drawing", referencedNames);
        Assert.DoesNotContain("System.Windows.Forms", referencedNames);
    }
}
