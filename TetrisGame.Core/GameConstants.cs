namespace TetrisGame.Core;

/// <summary>
/// Valorile fixe ale jocului. Restul proiectului le folosește de aici.
/// </summary>
public static class GameConstants
{
    /// <summary>Numărul de coloane ale puțului.</summary>
    public const int BoardWidth = 10;

    /// <summary>Numărul de rânduri vizibile ale puțului.</summary>
    public const int BoardHeight = 20;

    /// <summary>Rânduri ascunse deasupra zonei vizibile, pentru apariția piesei.</summary>
    public const int HiddenRows = 2;

    /// <summary>Latura unei celule, în pixeli.</summary>
    public const int CellSize = 30;

    /// <summary>Lățimea ferestrei de joc, în pixeli.</summary>
    public const int GameFormWidth = 600;

    /// <summary>Înălțimea ferestrei de joc, în pixeli.</summary>
    public const int GameFormHeight = 720;

    /// <summary>Intervalul de cădere la nivel 1, în milisecunde.</summary>
    public const int InitialDropIntervalMs = 800;

    /// <summary>Intervalul minim de cădere, în milisecunde.</summary>
    public const int MinDropIntervalMs = 100;

    /// <summary>Linii eliminate necesare pentru a crește nivelul.</summary>
    public const int LinesPerLevel = 10;

    /// <summary>Puncte acordate pentru fiecare celulă parcursă cu soft drop.</summary>
    public const int SoftDropPointsPerCell = 1;

    /// <summary>Puncte acordate pentru fiecare celulă parcursă cu hard drop.</summary>
    public const int HardDropPointsPerCell = 2;

    /// <summary>Lungimea maximă a numelui din clasament.</summary>
    public const int MaxNameLength = 12;

    /// <summary>Numărul maxim de scoruri păstrate în clasament.</summary>
    public const int MaxHighScores = 10;

    /// <summary>Nivelul de start minim.</summary>
    public const int StartLevelMin = 1;

    /// <summary>Nivelul de start maxim.</summary>
    public const int StartLevelMax = 10;

    /// <summary>Întârzierea până la repetarea unei săgeți ținute apăsată, în milisecunde.</summary>
    public const int DasDelayMs = 170;

    /// <summary>Intervalul de repetare a unei săgeți ținute apăsată, în milisecunde.</summary>
    public const int DasRepeatMs = 50;

    /// <summary>Durata evidențierii liniilor eliminate, în milisecunde.</summary>
    public const int LineClearFlashMs = 200;

    /// <summary>
    /// Punctele pentru liniile eliminate. Indexul este numărul de linii (0–4).
    /// </summary>
    public static IReadOnlyList<int> LineClearScores { get; } = [0, 100, 300, 500, 800];
}
