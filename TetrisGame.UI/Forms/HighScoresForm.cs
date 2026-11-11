namespace TetrisGame.UI.Forms;

/// <summary>
/// Fereastra de clasament. Conținutul se adaugă odată cu salvarea scorurilor.
/// </summary>
public sealed class HighScoresForm : Form
{
    /// <summary>
    /// Creează fereastra goală de clasament.
    /// </summary>
    public HighScoresForm()
    {
        PlaceholderForm.Apply(this, "Clasament");
    }
}
