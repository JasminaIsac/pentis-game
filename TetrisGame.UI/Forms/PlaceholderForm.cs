namespace TetrisGame.UI.Forms;

/// <summary>
/// Aspectul comun al ferestrelor care încă nu au conținut.
/// </summary>
internal static class PlaceholderForm
{
    /// <summary>
    /// Setează titlul, dimensiunea fixă și un text centrat.
    /// </summary>
    public static void Apply(Form form, string title)
    {
        form.Text = title;
        form.Font = new Font("Segoe UI", 12F);
        form.FormBorderStyle = FormBorderStyle.FixedSingle;
        form.MaximizeBox = false;
        form.StartPosition = FormStartPosition.CenterParent;
        form.ClientSize = new Size(420, 240);
        form.BackColor = Color.FromArgb(24, 24, 28);

        form.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
        });
    }
}
