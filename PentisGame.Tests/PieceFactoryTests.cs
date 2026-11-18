using PentisGame.Core;

namespace PentisGame.Tests;

public class PieceFactoryTests
{
    [Fact]
    public void EachBagOfTwelve_ContainsEveryTypeOnce()
    {
        var factory = new PieceFactory(new Random(42));
        for (var bag = 0; bag < 5; bag++)
        {
            var types = Enumerable.Range(0, 12).Select(_ => factory.GetNext()).ToArray();
            Assert.Equal(12, types.Distinct().Count());
            Assert.Equal(Enum.GetValues<PentominoType>().OrderBy(t => t), types.OrderBy(t => t));
        }
    }

    [Fact]
    public void SameSeed_ProducesSameSequence()
    {
        var a = new PieceFactory(new Random(123));
        var b = new PieceFactory(new Random(123));
        var seqA = Enumerable.Range(0, 36).Select(_ => a.GetNext()).ToArray();
        var seqB = Enumerable.Range(0, 36).Select(_ => b.GetNext()).ToArray();
        Assert.Equal(seqA, seqB);
    }

    [Fact]
    public void Peek_MatchesSubsequentGetNext()
    {
        var factory = new PieceFactory(new Random(7));
        var peeked = factory.Peek(3);
        var drawn = new[] { factory.GetNext(), factory.GetNext(), factory.GetNext() };
        Assert.Equal(peeked, drawn);
    }

    [Theory]
    [InlineData(PentominoType.I, 2)]
    [InlineData(PentominoType.X, 3)]
    [InlineData(PentominoType.L, 3)]
    public void Create_SpawnsCenteredAtTopWithRotationZero(PentominoType type, int expectedX)
    {
        var factory = new PieceFactory(new Random(1));
        var piece = factory.Create(type);
        Assert.Equal(0, piece.Y);
        Assert.Equal(0, piece.Rotation);
        Assert.Equal(expectedX, piece.X);
        Assert.Equal(type, piece.Type);
    }
}
