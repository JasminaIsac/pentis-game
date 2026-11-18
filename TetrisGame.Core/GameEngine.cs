namespace TetrisGame.Core;

/// <summary>
/// Motorul partidei: stări, gravitație, comenzi și evenimente. Fără dependențe UI.
/// </summary>
public sealed class GameEngine
{
    private static readonly (int Dx, int Dy)[] WallKicks =
    [
        (-1, 0),
        (1, 0),
        (-2, 0),
        (2, 0),
        (0, -1),
    ];

    private readonly PieceFactory _factory;
    private double _gravityAccumulator;
    private Tetromino? _currentPiece;

    /// <summary>
    /// Creează motorul. Testele pot injecta un <see cref="Random"/> cu seed fix.
    /// </summary>
    public GameEngine(Random? random = null)
    {
        Board = new Board();
        Score = new ScoreManager();
        _factory = new PieceFactory(random);
        State = GameState.Menu;
    }

    /// <summary>Puțul partidei.</summary>
    public Board Board { get; }

    /// <summary>Scorul, nivelul și viteza.</summary>
    public ScoreManager Score { get; }

    /// <summary>Starea curentă a partidei.</summary>
    public GameState State { get; private set; }

    /// <summary>Piesa activă; null înainte de StartNewGame.</summary>
    public Tetromino? CurrentPiece => _currentPiece;

    /// <summary>Tipul piesei din Hold, dacă există.</summary>
    public TetrominoType? HeldPiece { get; private set; }

    /// <summary>True cât timp Hold este permis pentru piesa curentă.</summary>
    public bool CanHold { get; private set; } = true;

    /// <summary>Ridicat după fixarea unei piese în puț.</summary>
    public event Action? PieceLocked;

    /// <summary>Ridicat când se elimină linii.</summary>
    public event Action<int>? LinesCleared;

    /// <summary>Ridicat când scorul se schimbă.</summary>
    public event Action? ScoreChanged
    {
        add => Score.ScoreChanged += value;
        remove => Score.ScoreChanged -= value;
    }

    /// <summary>Ridicat când nivelul se schimbă.</summary>
    public event Action? LevelChanged
    {
        add => Score.LevelChanged += value;
        remove => Score.LevelChanged -= value;
    }

    /// <summary>Ridicat la sfârșitul partidei.</summary>
    public event Action? GameOver;

    /// <summary>Ridicat la orice schimbare de stare.</summary>
    public event Action? StateChanged;

    /// <summary>Următoarele tipuri din sac, fără extragere.</summary>
    public IReadOnlyList<TetrominoType> PeekNext(int count = 1) => _factory.Peek(count);

    /// <summary>
    /// Pornește o partidă nouă de la nivelul dat.
    /// </summary>
    public void StartNewGame(int startLevel = GameConstants.StartLevelMin)
    {
        Board.Clear();
        Score.Reset(startLevel);
        _factory.Reset();
        HeldPiece = null;
        CanHold = true;
        _gravityAccumulator = 0;
        SetState(GameState.Playing);
        SpawnNextPiece();
    }

    /// <summary>Pune partida pe pauză.</summary>
    public void Pause()
    {
        if (State == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
    }

    /// <summary>Reia partida din pauză.</summary>
    public void Resume()
    {
        if (State == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }

    /// <summary>Comută între Playing și Paused.</summary>
    public void TogglePause()
    {
        if (State == GameState.Playing)
        {
            Pause();
        }
        else if (State == GameState.Paused)
        {
            Resume();
        }
    }

    /// <summary>
    /// Avansul timpului. Mută gravitația doar în starea Playing.
    /// </summary>
    public void Update(double elapsedMs)
    {
        if (State != GameState.Playing || _currentPiece is null)
        {
            return;
        }

        _gravityAccumulator += elapsedMs;
        while (State == GameState.Playing && _gravityAccumulator >= Score.DropIntervalMs)
        {
            _gravityAccumulator -= Score.DropIntervalMs;
            if (!TryMove(0, 1))
            {
                LockCurrentPiece();
                break;
            }
        }
    }

    /// <summary>Mută piesa o celulă la stânga, dacă este posibil.</summary>
    public bool MoveLeft() => IsPlaying && TryMove(-1, 0);

    /// <summary>Mută piesa o celulă la dreapta, dacă este posibil.</summary>
    public bool MoveRight() => IsPlaying && TryMove(1, 0);

    /// <summary>Rotește în sens orar, cu wall kick dacă e nevoie.</summary>
    public bool RotateClockwise() => IsPlaying && TryRotate(clockwise: true);

    /// <summary>Rotește în sens antiorar, cu wall kick dacă e nevoie.</summary>
    public bool RotateCounterClockwise() => IsPlaying && TryRotate(clockwise: false);

    /// <summary>
    /// Soft drop: o celulă în jos (+1 punct) sau fixare dacă nu mai e loc.
    /// </summary>
    public void SoftDrop()
    {
        if (!IsPlaying || _currentPiece is null)
        {
            return;
        }

        if (TryMove(0, 1))
        {
            Score.AddSoftDrop(1);
            _gravityAccumulator = 0;
        }
        else
        {
            LockCurrentPiece();
        }
    }

    /// <summary>
    /// Hard drop: coboară până la aterizare, adaugă 2 puncte pe celulă, fixează.
    /// </summary>
    public void HardDrop()
    {
        if (!IsPlaying || _currentPiece is null)
        {
            return;
        }

        var landing = GetDropPosition(_currentPiece);
        var cells = landing.Y - _currentPiece.Y;
        _currentPiece.Move(0, cells);
        if (cells > 0)
        {
            Score.AddHardDrop(cells);
        }

        LockCurrentPiece();
    }

    /// <summary>
    /// Hold: reține piesa curentă și aduce piesa reținută sau următoarea din sac.
    /// </summary>
    public void Hold()
    {
        if (!IsPlaying || _currentPiece is null || !CanHold)
        {
            return;
        }

        var currentType = _currentPiece.Type;
        var candidate = HeldPiece is null
            ? _factory.Create(_factory.GetNext())
            : _factory.Create(HeldPiece.Value);

        if (!Board.IsValidPosition(candidate))
        {
            return;
        }

        HeldPiece = currentType;
        _currentPiece = candidate;
        CanHold = false;
        _gravityAccumulator = 0;
    }

    /// <summary>
    /// Poziția de aterizare a piesei; nu modifică piesa și nu scrie în puț.
    /// </summary>
    public Tetromino GetDropPosition(Tetromino piece)
    {
        var clone = piece.Clone();
        while (true)
        {
            clone.Move(0, 1);
            if (!Board.IsValidPosition(clone))
            {
                clone.Move(0, -1);
                return clone;
            }
        }
    }

    private bool IsPlaying => State == GameState.Playing && _currentPiece is not null;

    private bool TryMove(int dx, int dy)
    {
        if (_currentPiece is null)
        {
            return false;
        }

        var clone = _currentPiece.Clone();
        clone.Move(dx, dy);
        if (!Board.IsValidPosition(clone))
        {
            return false;
        }

        _currentPiece.Move(dx, dy);
        return true;
    }

    private bool TryRotate(bool clockwise)
    {
        if (_currentPiece is null)
        {
            return false;
        }

        var clone = _currentPiece.Clone();
        if (clockwise)
        {
            clone.RotateClockwise();
        }
        else
        {
            clone.RotateCounterClockwise();
        }

        if (Board.IsValidPosition(clone))
        {
            ApplyRotation(clockwise, 0, 0);
            return true;
        }

        foreach (var (dx, dy) in WallKicks)
        {
            var kicked = clone.Clone();
            kicked.Move(dx, dy);
            if (Board.IsValidPosition(kicked))
            {
                ApplyRotation(clockwise, dx, dy);
                return true;
            }
        }

        return false;
    }

    private void ApplyRotation(bool clockwise, int dx, int dy)
    {
        if (_currentPiece is null)
        {
            return;
        }

        if (clockwise)
        {
            _currentPiece.RotateClockwise();
        }
        else
        {
            _currentPiece.RotateCounterClockwise();
        }

        if (dx != 0 || dy != 0)
        {
            _currentPiece.Move(dx, dy);
        }
    }

    private void LockCurrentPiece()
    {
        if (_currentPiece is null)
        {
            return;
        }

        Board.LockPiece(_currentPiece);
        PieceLocked?.Invoke();

        var lines = Board.ClearFullLines();
        if (lines > 0)
        {
            var scoredLines = Math.Min(lines, 4);
            Score.AddLineClear(scoredLines);
            LinesCleared?.Invoke(scoredLines);
        }

        CanHold = true;
        SpawnNextPiece();
    }

    private void SpawnNextPiece()
    {
        var next = _factory.Create(_factory.GetNext());
        if (!Board.IsValidPosition(next))
        {
            _currentPiece = next;
            SetState(GameState.GameOver);
            GameOver?.Invoke();
            return;
        }

        _currentPiece = next;
        _gravityAccumulator = 0;
    }

    private void SetState(GameState state)
    {
        if (State == state)
        {
            return;
        }

        State = state;
        StateChanged?.Invoke();
    }
}
