namespace TetrisGame.UI.Forms;

/// <summary>
/// Fereastra Despre. Instrucțiunile de joc se adaugă ulterior.
/// </summary>
public sealed class AboutForm : Form
{
    /// <summary>
    /// Creează fereastra goală Despre.
    /// </summary>
    public AboutForm()
    {
        PlaceholderForm.Apply(this, "Despre");
    }
}
