using PentisGame.Core;

namespace PentisGame.Tests;

public class GameEngineTests
{
    private static int Bottom => GameConstants.BoardHeight + GameConstants.HiddenRows - 1;

    [Fact]
    public void StartNewGame_ResetsBoardScoreAndSpawnsPiece()
    {
        var engine = new GameEngine(new Random(1));
        engine.StartNewGame(5);

        Assert.Equal(GameState.Playing, engine.State);
        Assert.Equal(0, engine.Score.Score);
        Assert.Equal(5, engine.Score.Level);
        Assert.NotNull(engine.CurrentPiece);
        Assert.Equal(0, engine.CurrentPiece!.Y);
    }

    [Fact]
    public void Update_AfterDropInterval_MovesPieceDownOneCell()
    {
        var engine = new GameEngine(new Random(2));
        engine.StartNewGame(1);
        var y = engine.CurrentPiece!.Y;
        engine.Update(engine.Score.DropIntervalMs);
        Assert.Equal(y + 1, engine.CurrentPiece.Y);
    }

    [Fact]
    public void MoveLeftRight_OnlyWhenValid()
    {
        var engine = new GameEngine(new Random(3));
        engine.StartNewGame(1);
        var startX = engine.CurrentPiece!.X;
        Assert.True(engine.MoveRight());
        Assert.Equal(startX + 1, engine.CurrentPiece!.X);

        while (engine.MoveLeft())
        {
        }

        var blockedX = engine.CurrentPiece!.X;
        Assert.False(engine.MoveLeft());
        Assert.Equal(blockedX, engine.CurrentPiece.X);
    }

    [Fact]
    public void Rotate_ChangesShapeWhenSpaceAllows()
    {
        var engine = new GameEngine(new Random(4));
        engine.StartNewGame(1);
        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.T, 3, 8));
        var before = CopyShape(engine.CurrentPiece!.Shape);
        Assert.True(engine.RotateClockwise());
        Assert.False(ShapesEqual(before, engine.CurrentPiece.Shape));
        Assert.True(engine.Board.IsValidPosition(engine.CurrentPiece));
    }

    [Fact]
    public void RotateAgainstWallWithoutKickRoom_LeavesPieceUnchanged()
    {
        var engine = new GameEngine(new Random(5));
        engine.StartNewGame(1);
        ReplaceCurrentPiece(engine, CreatePieceAt(PentominoType.I, 0, 8, rotations: 1));

        for (var y = 0; y < engine.Board.TotalRows; y++)
        {
            engine.Board.SetCell(1, y, 1);
            engine.Board.SetCell(2, y, 1);
        }

        var current = engine.CurrentPiece!;
        var x = current.X;
        var yPos = current.Y;
        var rotation = current.Rotation;
        var shape = CopyShape(current.Shape);

        Assert.False(engine.RotateClockwise());
        Assert.Equal(x, engine.CurrentPiece!.X);
        Assert.Equal(yPos, engine.CurrentPiece.Y);
        Assert.Equal(rotation, engine.CurrentPiece.Rotation);
        Assert.True(ShapesEqual(shape, engine.CurrentPiece.Shape));
    }

    [Fact]
    public void WallKick_AllowsRotationNearLeftWall()
    {
        var engine = new GameEngine(new Random(6));
        engine.StartNewGame(1);
        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.T, 0, 10));
        Assert.True(engine.RotateCounterClockwise());
        Assert.True(engine.Board.IsValidPosition(engine.CurrentPiece!));
        Assert.All(engine.CurrentPiece!.GetCells(), c => Assert.True(c.X >= 0));
    }

    [Fact]
    public void WallKick_WorksForIWithTwoCellOffset()
    {
        var engine = new GameEngine(new Random(7));
        engine.StartNewGame(1);
        ReplaceCurrentPiece(engine, CreatePieceAt(PentominoType.I, 6, 10, rotations: 0));
        Assert.True(engine.RotateClockwise());
        Assert.True(engine.Board.IsValidPosition(engine.CurrentPiece!));
    }

    [Fact]
    public void FailedDownwardMove_IsSignaledByLockOnUpdate()
    {
        var engine = new GameEngine(new Random(8));
        engine.StartNewGame(1);
        // X pe 3×3: la Y=Bottom-2 ocupă Bottom-2..Bottom
        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 4, Bottom - 2));

        var locked = false;
        engine.PieceLocked += () => locked = true;
        engine.Update(engine.Score.DropIntervalMs);

        Assert.True(locked);
        Assert.NotEqual(0, engine.Board.GetCell(5, Bottom - 1));
    }

    [Fact]
    public void Locking_ClearsLinesAndAwardsPoints()
    {
        var engine = new GameEngine(new Random(9));
        engine.StartNewGame(1);

        // I orizontal acoperă coloanele 2–6 pe rândul Bottom; umplem restul
        for (var x = 0; x < 10; x++)
        {
            if (x is < 2 or > 6)
            {
                engine.Board.SetCell(x, Bottom, 1);
            }
        }

        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.I, 2, Bottom - 1));
        engine.HardDrop();

        Assert.Equal(0, engine.Board.GetCell(0, Bottom));
        Assert.Equal(100, engine.Score.Score);
        Assert.Equal(1, engine.Score.TotalLines);
    }

    [Fact]
    public void SpawnOnOccupiedCells_EndsGameWithoutOverwriting()
    {
        var engine = new GameEngine(new Random(10));
        engine.StartNewGame(1);

        for (var y = 0; y < 6; y++)
        {
            for (var x = 0; x < 9; x++)
            {
                engine.Board.SetCell(x, y, 2);
            }
        }

        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 4, Bottom - 2));
        var gameOver = false;
        engine.GameOver += () => gameOver = true;
        engine.HardDrop();

        Assert.True(gameOver);
        Assert.Equal(GameState.GameOver, engine.State);
        Assert.Equal(PentominoCatalog.GetColorId(PentominoType.X), engine.Board.GetCell(5, Bottom - 1));
        Assert.Equal(2, engine.Board.GetCell(0, 0));
    }

    [Fact]
    public void PauseAndGameOver_FreezeGravity()
    {
        var engine = new GameEngine(new Random(11));
        engine.StartNewGame(1);
        var y = engine.CurrentPiece!.Y;

        engine.Pause();
        engine.Update(engine.Score.DropIntervalMs * 3);
        Assert.Equal(y, engine.CurrentPiece.Y);

        engine.Resume();
        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 4, Bottom - 2));
        engine.HardDrop();
        Assert.Equal(GameState.Playing, engine.State);

        for (var row = 0; row < 6; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                engine.Board.SetCell(col, row, 1);
            }
        }

        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 0, Bottom - 2));
        engine.HardDrop();
        Assert.Equal(GameState.GameOver, engine.State);

        var pieceY = engine.CurrentPiece!.Y;
        engine.Update(5000);
        Assert.Equal(pieceY, engine.CurrentPiece.Y);
    }

    [Fact]
    public void SoftDrop_AddsPointResetsGravityAndLocksWhenBlocked()
    {
        var engine = new GameEngine(new Random(12));
        engine.StartNewGame(1);
        var y = engine.CurrentPiece!.Y;
        engine.SoftDrop();
        Assert.Equal(y + 1, engine.CurrentPiece.Y);
        Assert.Equal(1, engine.Score.Score);

        engine.Update(engine.Score.DropIntervalMs - 1);
        Assert.Equal(y + 1, engine.CurrentPiece.Y);
        engine.Update(1);
        Assert.Equal(y + 2, engine.CurrentPiece.Y);

        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 4, Bottom - 2));
        var locked = false;
        engine.PieceLocked += () => locked = true;
        engine.SoftDrop();
        Assert.True(locked);
    }

    [Fact]
    public void HardDrop_MatchesGetDropPositionAndScoresTwoPerCell()
    {
        var engine = new GameEngine(new Random(13));
        engine.StartNewGame(1);
        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 4, 5));
        var landing = engine.GetDropPosition(engine.CurrentPiece!);
        var cells = landing.Y - engine.CurrentPiece!.Y;
        var scoreBefore = engine.Score.Score;

        engine.HardDrop();

        Assert.Equal(Bottom - 2, landing.Y);
        Assert.Equal(scoreBefore + cells * 2, engine.Score.Score);
        Assert.Equal(PentominoCatalog.GetColorId(PentominoType.X), engine.Board.GetCell(5, Bottom - 1));
    }

    [Fact]
    public void HardDrop_AlreadyLanded_LocksWithZeroPoints()
    {
        var engine = new GameEngine(new Random(14));
        engine.StartNewGame(1);
        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 4, Bottom - 2));
        engine.HardDrop();
        Assert.Equal(0, engine.Score.Score);
        Assert.Equal(PentominoCatalog.GetColorId(PentominoType.X), engine.Board.GetCell(5, Bottom - 1));
    }

    [Fact]
    public void Hold_SwapsOnceUntilLock()
    {
        var engine = new GameEngine(new Random(15));
        engine.StartNewGame(1);
        var first = engine.CurrentPiece!.Type;
        var upcoming = engine.PeekNext(1)[0];

        engine.Hold();
        Assert.Equal(first, engine.HeldPiece);
        Assert.Equal(upcoming, engine.CurrentPiece!.Type);
        Assert.False(engine.CanHold);

        var held = engine.HeldPiece;
        var current = engine.CurrentPiece.Type;
        engine.Hold();
        Assert.Equal(held, engine.HeldPiece);
        Assert.Equal(current, engine.CurrentPiece.Type);

        engine.HardDrop();
        Assert.True(engine.CanHold);

        var beforeSwap = engine.CurrentPiece!.Type;
        engine.Hold();
        Assert.Equal(beforeSwap, engine.HeldPiece);
        Assert.Equal(held, engine.CurrentPiece!.Type);
        Assert.Equal(0, engine.CurrentPiece.Rotation);
        Assert.Equal(0, engine.CurrentPiece.Y);
    }

    [Fact]
    public void Hold_IntoOccupiedSpawn_DoesNothing()
    {
        var engine = new GameEngine(new Random(16));
        engine.StartNewGame(1);
        engine.Hold();
        Assert.NotNull(engine.HeldPiece);

        for (var y = 0; y < 6; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                engine.Board.SetCell(x, y, 1);
            }
        }

        ReplaceCurrentPiece(engine, new Pentomino(PentominoType.X, 0, Bottom - 4));
        SetCanHold(engine, true);

        var held = engine.HeldPiece;
        var currentType = engine.CurrentPiece!.Type;
        var xPos = engine.CurrentPiece.X;
        var yPos = engine.CurrentPiece.Y;

        engine.Hold();

        Assert.Equal(held, engine.HeldPiece);
        Assert.Equal(currentType, engine.CurrentPiece.Type);
        Assert.Equal(xPos, engine.CurrentPiece.X);
        Assert.Equal(yPos, engine.CurrentPiece.Y);
    }

    [Fact]
    public void Commands_DoNothingWhenPaused()
    {
        var engine = new GameEngine(new Random(17));
        engine.StartNewGame(1);
        engine.Pause();
        var x = engine.CurrentPiece!.X;
        Assert.False(engine.MoveLeft());
        Assert.False(engine.RotateClockwise());
        engine.SoftDrop();
        Assert.Equal(x, engine.CurrentPiece.X);
        Assert.Equal(0, engine.Score.Score);
    }

    private static void ReplaceCurrentPiece(GameEngine engine, Pentomino piece)
    {
        var field = typeof(GameEngine).GetField("_currentPiece", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field!.SetValue(engine, piece);
    }

    private static void SetCanHold(GameEngine engine, bool value)
    {
        typeof(GameEngine).GetProperty(nameof(GameEngine.CanHold))!.SetValue(engine, value);
    }

    private static Pentomino CreatePieceAt(PentominoType type, int x, int y, int rotations)
    {
        var piece = new Pentomino(type, x, y);
        for (var i = 0; i < rotations; i++)
        {
            piece.RotateClockwise();
        }

        return piece;
    }

    private static int[,] CopyShape(int[,] shape)
    {
        var copy = new int[shape.GetLength(0), shape.GetLength(1)];
        Array.Copy(shape, copy, shape.Length);
        return copy;
    }

    private static bool ShapesEqual(int[,] a, int[,] b)
    {
        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
        {
            return false;
        }

        for (var row = 0; row < a.GetLength(0); row++)
        {
            for (var col = 0; col < a.GetLength(1); col++)
            {
                if (a[row, col] != b[row, col])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
