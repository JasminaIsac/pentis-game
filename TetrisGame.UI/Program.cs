using TetrisGame.UI.Forms;

namespace TetrisGame.UI;

/// <summary>
/// Punctul de intrare al aplicației.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Pornește meniul principal.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainMenuForm());
    }
}
