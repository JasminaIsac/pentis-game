using System.Diagnostics;
using TetrisGame.Core;
using TetrisGame.UI.Rendering;

namespace TetrisGame.UI.Forms;

/// <summary>
/// Fereastra de joc: bucla de timp, taste, desenarea puțului și panoul lateral.
/// </summary>
public sealed class GameForm : Form
{
    private static readonly Rectangle BoardArea = new(20, 60, 300, 600);
    private static readonly Rectangle PanelArea = new(340, 60, 240, 600);

    private readonly GameEngine _engine;
    private readonly BoardRenderer _renderer;
    private readonly Theme _theme;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly Font _titleFont;
    private readonly Font _labelFont;
    private readonly Font _valueFont;
    private readonly Font _overlayFont;
    private readonly SolidBrush _textBrush;
    private readonly SolidBrush _panelBrush;
    private readonly SolidBrush _overlayBrush;
    private readonly Pen _panelBorderPen;

    private long _lastTimestamp;
    private bool _leftHeld;
    private bool _rightHeld;
    private bool _softDropHeld;
    private double _leftRepeatTimer;
    private double _rightRepeatTimer;
    private double _softDropRepeatTimer;
    private bool _leftInitialDelayDone;
    private bool _rightInitialDelayDone;

    private int _displayScore;
    private int _displayLevel = 1;
    private int _displayLines;
    private readonly HashSet<Keys> _oneShotKeysDown = [];

    /// <summary>
    /// Creează fereastra de joc, centrată și fără maximizare.
    /// </summary>
    public GameForm()
    {
        DoubleBuffered = true;
        AutoScaleMode = AutoScaleMode.None;
        KeyPreview = true;
        Text = "Tetris — Joc";
        Font = new Font("Segoe UI", 12F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(GameConstants.GameFormWidth, GameConstants.GameFormHeight);
        BackColor = Color.FromArgb(24, 24, 28);

        _theme = Theme.Dark;
        _renderer = new BoardRenderer(_theme);
        _engine = new GameEngine();
        _titleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
        _labelFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        _valueFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        _overlayFont = new Font("Segoe UI", 28F, FontStyle.Bold);
        _textBrush = new SolidBrush(_theme.Text);
        _panelBrush = new SolidBrush(Color.FromArgb(32, 32, 40));
        _overlayBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
        _panelBorderPen = new Pen(Color.FromArgb(70, 70, 80));

        _timer = new System.Windows.Forms.Timer { Interval = 16 };
        _timer.Tick += OnTimerTick;

        _engine.ScoreChanged += OnScoreChanged;
        _engine.LevelChanged += OnLevelChanged;
        _engine.LinesCleared += OnLinesCleared;
        _engine.PieceLocked += RequestRedraw;
        _engine.StateChanged += RequestRedraw;
        _engine.GameOver += RequestRedraw;

        Load += (_, _) =>
        {
            _engine.StartNewGame();
            SyncHudFromEngine();
            _lastTimestamp = Stopwatch.GetTimestamp();
            _timer.Start();
        };

        FormClosed += (_, _) =>
        {
            _timer.Stop();
            _timer.Dispose();
            _renderer.Dispose();
            _titleFont.Dispose();
            _labelFont.Dispose();
            _valueFont.Dispose();
            _overlayFont.Dispose();
            _textBrush.Dispose();
            _panelBrush.Dispose();
            _overlayBrush.Dispose();
            _panelBorderPen.Dispose();
        };
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.Clear(_theme.WindowBackground);

        _renderer.Draw(g, _engine, BoardArea);
        DrawSidePanel(g);
        DrawOverlays(g);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled)
        {
            return;
        }

        if (IsOneShotKey(e.KeyCode) && !_oneShotKeysDown.Add(e.KeyCode))
        {
            e.Handled = true;
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Left:
                if (!_leftHeld)
                {
                    _engine.MoveLeft();
                    Invalidate();
                    _leftHeld = true;
                    _leftInitialDelayDone = false;
                    _leftRepeatTimer = 0;
                }

                e.Handled = true;
                break;

            case Keys.Right:
                if (!_rightHeld)
                {
                    _engine.MoveRight();
                    Invalidate();
                    _rightHeld = true;
                    _rightInitialDelayDone = false;
                    _rightRepeatTimer = 0;
                }

                e.Handled = true;
                break;

            case Keys.Down:
                if (!_softDropHeld)
                {
                    _engine.SoftDrop();
                    SyncHudFromEngine();
                    Invalidate();
                    _softDropHeld = true;
                    _softDropRepeatTimer = 0;
                }

                e.Handled = true;
                break;

            case Keys.Up:
            case Keys.X:
                _engine.RotateClockwise();
                Invalidate();
                e.Handled = true;
                break;

            case Keys.Z:
                _engine.RotateCounterClockwise();
                Invalidate();
                e.Handled = true;
                break;

            case Keys.Space:
                _engine.HardDrop();
                SyncHudFromEngine();
                Invalidate();
                e.Handled = true;
                break;

            case Keys.C:
            case Keys.ShiftKey:
            case Keys.LShiftKey:
            case Keys.RShiftKey:
                _engine.Hold();
                Invalidate();
                e.Handled = true;
                break;

            case Keys.P:
            case Keys.Escape:
                _engine.TogglePause();
                Invalidate();
                e.Handled = true;
                break;

            case Keys.R:
                RequestRestart();
                e.Handled = true;
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        _oneShotKeysDown.Remove(e.KeyCode);
        switch (e.KeyCode)
        {
            case Keys.Left:
                _leftHeld = false;
                break;
            case Keys.Right:
                _rightHeld = false;
                break;
            case Keys.Down:
                _softDropHeld = false;
                break;
        }
    }

    private static bool IsOneShotKey(Keys key) => key is
        Keys.Up or Keys.X or Keys.Z or Keys.Space or Keys.C or
        Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey or
        Keys.P or Keys.Escape or Keys.R;

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var now = Stopwatch.GetTimestamp();
        var elapsedMs = Stopwatch.GetElapsedTime(_lastTimestamp, now).TotalMilliseconds;
        _lastTimestamp = now;

        HandleHeldKeys(elapsedMs);
        _engine.Update(elapsedMs);
        Invalidate();
    }

    private void HandleHeldKeys(double elapsedMs)
    {
        if (_engine.State != GameState.Playing)
        {
            return;
        }

        if (_leftHeld)
        {
            _leftRepeatTimer += elapsedMs;
            var threshold = _leftInitialDelayDone ? GameConstants.DasRepeatMs : GameConstants.DasDelayMs;
            if (_leftRepeatTimer >= threshold)
            {
                _engine.MoveLeft();
                _leftRepeatTimer = 0;
                _leftInitialDelayDone = true;
            }
        }

        if (_rightHeld)
        {
            _rightRepeatTimer += elapsedMs;
            var threshold = _rightInitialDelayDone ? GameConstants.DasRepeatMs : GameConstants.DasDelayMs;
            if (_rightRepeatTimer >= threshold)
            {
                _engine.MoveRight();
                _rightRepeatTimer = 0;
                _rightInitialDelayDone = true;
            }
        }

        if (_softDropHeld)
        {
            _softDropRepeatTimer += elapsedMs;
            if (_softDropRepeatTimer >= GameConstants.DasRepeatMs)
            {
                _engine.SoftDrop();
                SyncHudFromEngine();
                _softDropRepeatTimer = 0;
            }
        }
    }

    private void DrawSidePanel(Graphics g)
    {
        g.FillRectangle(_panelBrush, PanelArea);
        g.DrawRectangle(_panelBorderPen, PanelArea);

        var x = PanelArea.Left + 16;
        var width = PanelArea.Width - 32;
        var y = PanelArea.Top + 12;

        g.DrawString("TETRIS", _titleFont, _textBrush, new RectangleF(x, y, width, 36), Centered());
        y += 48;

        g.DrawString("NEXT", _labelFont, _textBrush, x, y);
        y += 24;
        var nextBox = new Rectangle(x, y, width, 72);
        g.DrawRectangle(_panelBorderPen, nextBox);
        var next = _engine.PeekNext(1);
        if (next.Count > 0)
        {
            _renderer.DrawPreview(g, nextBox, next[0]);
        }

        y += 88;
        g.DrawString("HOLD", _labelFont, _textBrush, x, y);
        y += 24;
        var holdBox = new Rectangle(x, y, width, 72);
        g.DrawRectangle(_panelBorderPen, holdBox);
        _renderer.DrawPreview(g, holdBox, _engine.HeldPiece, dimmed: !_engine.CanHold);

        y += 96;
        g.DrawString("SCOR", _labelFont, _textBrush, x, y);
        y += 24;
        g.DrawString(_displayScore.ToString("D6"), _valueFont, _textBrush, x, y);

        y += 40;
        g.DrawString("NIVEL", _labelFont, _textBrush, x, y);
        y += 24;
        g.DrawString(_displayLevel.ToString(), _valueFont, _textBrush, x, y);

        y += 40;
        g.DrawString("LINII", _labelFont, _textBrush, x, y);
        y += 24;
        g.DrawString(_displayLines.ToString(), _valueFont, _textBrush, x, y);
    }

    private void DrawOverlays(Graphics g)
    {
        if (_engine.State == GameState.Paused)
        {
            g.FillRectangle(_overlayBrush, BoardArea);
            DrawCenteredOverlay(g, "PAUZĂ");
        }
        else if (_engine.State == GameState.GameOver)
        {
            g.FillRectangle(_overlayBrush, BoardArea);
            DrawCenteredOverlay(g, "GAME OVER");
            var scoreText = $"Scor: {_displayScore}   Nivel: {_displayLevel}";
            var size = g.MeasureString(scoreText, _labelFont);
            g.DrawString(
                scoreText,
                _labelFont,
                _textBrush,
                BoardArea.Left + (BoardArea.Width - size.Width) / 2,
                BoardArea.Top + BoardArea.Height / 2f + 28);
        }
    }

    private void DrawCenteredOverlay(Graphics g, string text)
    {
        var size = g.MeasureString(text, _overlayFont);
        g.DrawString(
            text,
            _overlayFont,
            _textBrush,
            BoardArea.Left + (BoardArea.Width - size.Width) / 2,
            BoardArea.Top + (BoardArea.Height - size.Height) / 2);
    }

    private void RequestRestart()
    {
        var result = MessageBox.Show(
            this,
            "Repornești partida curentă?",
            "Repornire",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        _engine.StartNewGame();
        SyncHudFromEngine();
        Invalidate();
    }

    private void OnScoreChanged()
    {
        _displayScore = _engine.Score.Score;
        RequestRedraw();
    }

    private void OnLevelChanged()
    {
        _displayLevel = _engine.Score.Level;
        RequestRedraw();
    }

    private void OnLinesCleared(int _)
    {
        _displayLines = _engine.Score.TotalLines;
        _displayScore = _engine.Score.Score;
        _displayLevel = _engine.Score.Level;
        RequestRedraw();
    }

    private void SyncHudFromEngine()
    {
        _displayScore = _engine.Score.Score;
        _displayLevel = _engine.Score.Level;
        _displayLines = _engine.Score.TotalLines;
    }

    private void RequestRedraw()
    {
        if (!IsHandleCreated || IsDisposed)
        {
            return;
        }

        BeginInvoke(Invalidate);
    }

    private static StringFormat Centered() => new()
    {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center,
    };
}
