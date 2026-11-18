namespace TetrisGame.Core;

/// <summary>
/// Scor, nivel, linii eliminate și intervalul de cădere.
/// </summary>
public sealed class ScoreManager
{
    private int _startLevel = GameConstants.StartLevelMin;

    /// <summary>Scorul curent.</summary>
    public int Score { get; private set; }

    /// <summary>Nivelul curent.</summary>
    public int Level { get; private set; } = GameConstants.StartLevelMin;

    /// <summary>Numărul total de linii eliminate în partidă.</summary>
    public int TotalLines { get; private set; }

    /// <summary>Intervalul de cădere pentru nivelul curent, în milisecunde.</summary>
    public int DropIntervalMs { get; private set; } = GameConstants.InitialDropIntervalMs;

    /// <summary>Ridicat doar când scorul se schimbă.</summary>
    public event Action? ScoreChanged;

    /// <summary>Ridicat doar când nivelul se schimbă.</summary>
    public event Action? LevelChanged;

    /// <summary>
    /// Resetează scorul la 0 și pornește de la nivelul cerut (1–10).
    /// </summary>
    public void Reset(int startLevel)
    {
        _startLevel = Math.Clamp(startLevel, GameConstants.StartLevelMin, GameConstants.StartLevelMax);
        Score = 0;
        Level = _startLevel;
        TotalLines = 0;
        DropIntervalMs = ComputeDropInterval(Level);
    }

    /// <summary>
    /// Adaugă punctele pentru 1–4 linii, apoi actualizează nivelul și viteza.
    /// </summary>
    public void AddLineClear(int lines)
    {
        if (lines is < 1 or > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(lines), lines, "Numărul de linii trebuie să fie între 1 și 4.");
        }

        var previousScore = Score;
        var previousLevel = Level;

        Score += GameConstants.LineClearScores[lines] * Level;
        TotalLines += lines;
        Level = _startLevel + TotalLines / GameConstants.LinesPerLevel;
        DropIntervalMs = ComputeDropInterval(Level);

        if (Score != previousScore)
        {
            ScoreChanged?.Invoke();
        }

        if (Level != previousLevel)
        {
            LevelChanged?.Invoke();
        }
    }

    /// <summary>Adaugă punctele de soft drop.</summary>
    public void AddSoftDrop(int cells)
    {
        if (cells <= 0)
        {
            return;
        }

        Score += cells * GameConstants.SoftDropPointsPerCell;
        ScoreChanged?.Invoke();
    }

    /// <summary>Adaugă punctele de hard drop.</summary>
    public void AddHardDrop(int cells)
    {
        if (cells <= 0)
        {
            return;
        }

        Score += cells * GameConstants.HardDropPointsPerCell;
        ScoreChanged?.Invoke();
    }

    private static int ComputeDropInterval(int level)
    {
        var interval = (int)(GameConstants.InitialDropIntervalMs * Math.Pow(0.9, level - 1));
        return Math.Max(GameConstants.MinDropIntervalMs, interval);
    }
}
