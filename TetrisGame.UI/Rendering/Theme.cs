using TetrisGame.Core;

namespace TetrisGame.UI.Rendering;

/// <summary>
/// Paleta de culori a temei întunecate (livrare minimă).
/// </summary>
public sealed class Theme
{
    /// <summary>Tema întunecată implicită.</summary>
    public static Theme Dark { get; } = new();

    /// <summary>Fundalul ferestrei și al panoului.</summary>
    public Color WindowBackground { get; } = Color.FromArgb(24, 24, 28);

    /// <summary>Fundalul puțului.</summary>
    public Color BoardBackground { get; } = Color.FromArgb(12, 12, 16);

    /// <summary>Culoarea grilei.</summary>
    public Color Grid { get; } = Color.FromArgb(40, 40, 48);

    /// <summary>Culoarea textului.</summary>
    public Color Text { get; } = Color.White;

    /// <summary>Conturul unei celule ocupate.</summary>
    public Color CellBorder { get; } = Color.FromArgb(20, 20, 24);

    /// <summary>
    /// Întoarce culoarea GDI+ pentru identificatorul din grilă (1–7).
    /// </summary>
    public Color GetPieceColor(int colorId) => colorId switch
    {
        1 => Color.Cyan,
        2 => Color.Gold,
        3 => Color.MediumOrchid,
        4 => Color.LimeGreen,
        5 => Color.Tomato,
        6 => Color.DodgerBlue,
        7 => Color.Orange,
        _ => BoardBackground,
    };

    /// <summary>Culoarea unei piese după tip.</summary>
    public Color GetPieceColor(TetrominoType type) =>
        GetPieceColor(TetrominoCatalog.GetColorId(type));
}
