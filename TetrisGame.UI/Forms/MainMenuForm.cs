namespace TetrisGame.UI.Forms;

/// <summary>
/// Meniul principal. Din el se deschid ferestrele aplicației.
/// </summary>
public sealed class MainMenuForm : Form
{
    /// <summary>
    /// Creează meniul cu acțiunile de navigare.
    /// </summary>
    public MainMenuForm()
    {
        Text = "Tetris";
        Font = new Font("Segoe UI", 12F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(420, 460);
        BackColor = Color.FromArgb(24, 24, 28);

        var title = new Label
        {
            Text = "Tetris",
            Bounds = new Rectangle(0, 36, ClientSize.Width, 48),
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Font.FontFamily, 22F, FontStyle.Bold),
            ForeColor = Color.White,
        };
        Controls.Add(title);

        var top = 120;
        Controls.Add(CreateButton("Joc nou", top, (_, _) => ShowChild(new GameForm())));
        Controls.Add(CreateButton("Clasament", top += 56, (_, _) => ShowChild(new HighScoresForm())));
        Controls.Add(CreateButton("Setări", top += 56, (_, _) => ShowChild(new SettingsForm())));
        Controls.Add(CreateButton("Despre", top += 56, (_, _) => ShowChild(new AboutForm())));
        Controls.Add(CreateButton("Ieșire", top += 56, (_, _) => Close()));
    }

    private Button CreateButton(string text, int top, EventHandler onClick)
    {
        var button = new Button
        {
            Text = text,
            Size = new Size(240, 44),
            Location = new Point((ClientSize.Width - 240) / 2, top),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(48, 48, 56),
            ForeColor = Color.White,
            UseVisualStyleBackColor = false,
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 100);
        button.Click += onClick;
        return button;
    }

    private void ShowChild(Form form)
    {
        using (form)
        {
            form.ShowDialog(this);
        }
    }
}
