using TetrisGame.Core;

namespace TetrisGame.UI.Forms;

/// <summary>
/// Fereastra de joc. În această etapă este goală și are dimensiunea fixă a tablei.
/// </summary>
public sealed class GameForm : Form
{
    /// <summary>
    /// Creează fereastra de joc, centrată și fără maximizare.
    /// </summary>
    public GameForm()
    {
        DoubleBuffered = true;
        AutoScaleMode = AutoScaleMode.None;
        Text = "Tetris — Joc";
        Font = new Font("Segoe UI", 12F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(GameConstants.GameFormWidth, GameConstants.GameFormHeight);
        BackColor = Color.FromArgb(24, 24, 28);

        Controls.Add(new Label
        {
            Text = "Zona de joc",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
        });
    }
}
