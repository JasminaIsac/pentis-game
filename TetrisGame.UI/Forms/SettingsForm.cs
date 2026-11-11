namespace TetrisGame.UI.Forms;

/// <summary>
/// Fereastra de setări. Controalele se adaugă odată cu salvarea setărilor.
/// </summary>
public sealed class SettingsForm : Form
{
    /// <summary>
    /// Creează fereastra goală de setări.
    /// </summary>
    public SettingsForm()
    {
        PlaceholderForm.Apply(this, "Setări");
    }
}
