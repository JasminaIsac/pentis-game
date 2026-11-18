using PentisGame.Core;

namespace PentisGame.UI.Rendering;

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
    /// Întoarce culoarea GDI+ pentru identificatorul din grilă (1–12).
    /// </summary>
    public Color GetPieceColor(int colorId) => colorId switch
    {
        1 => Color.DeepPink,       // F
        2 => Color.Cyan,           // I
        3 => Color.Orange,         // L
        4 => Color.LimeGreen,      // N
        5 => Color.HotPink,        // P
        6 => Color.MediumOrchid,   // T
        7 => Color.DodgerBlue,     // U
        8 => Color.Gold,           // V
        9 => Color.Coral,          // W
        10 => Color.WhiteSmoke,    // X
        11 => Color.YellowGreen,   // Y
        12 => Color.Tomato,        // Z
        _ => BoardBackground,
    };

    /// <summary>Culoarea unei piese după tip.</summary>
    public Color GetPieceColor(PentominoType type) =>
        GetPieceColor(PentominoCatalog.GetColorId(type));
}
