namespace PentisGame.Core;

/// <summary>
/// Stările posibile ale partidei.
/// </summary>
public enum GameState
{
    /// <summary>Înainte de pornirea unei partide.</summary>
    Menu,

    /// <summary>Partida rulează.</summary>
    Playing,

    /// <summary>Partida este pe pauză.</summary>
    Paused,

    /// <summary>Partida s-a încheiat.</summary>
    GameOver,
}
