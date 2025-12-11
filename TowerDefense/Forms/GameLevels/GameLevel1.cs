using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Entities.Towers;
using TowerDefense.Data;

namespace TowerDefense.Forms.GameLevels
{
    // 'partial' is the key keyword to link with Input.cs and Render.cs files
    public partial class GameLevel1 : Form
    {
        // =========================================================
        // 1. VARIABLE DECLARATION (SHARED STATE)
        // =========================================================

        // Timer for game loop
        private Timer _gameTimer;
        private bool _isPaused = false;

        // Variables for Visuals (Used in Render.cs)
        private int _lastLives;
        private int _hurtTimer = 0;

        // Variables for Upgrade UI (Used in Input.cs)
        private Panel _pnlTowerActions;
        private Label _lblTowerInfo;
        private Button _btnUpgrade;
        private Button _btnSell;
        private Tower _selectedTower;

        // =========================================================
        // 2. CONSTRUCTOR
        // =========================================================
        public GameLevel1(int levelId)
        {
            // Form Configuration
            this.Text = $"Tower Defense - Level {levelId}";
            this.ClientSize = new Size(800, 600);
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Lock border
            this.MaximizeBox = false; // Disable Maximize button

            InitializeComponent();
            InitializeDynamicControls();
            InitializeTowerSelector(); // <--- This line is crucial now

            // C. Init Game Logic
            SoundManager.LoadSounds();
            GameManager.Instance.StartGame(levelId);
            _lastLives = GameManager.Instance.PlayerLives;

            // Default tower selection (This function is in Input.cs)
            // Note: Ensure InitializeTowerSelector() is called if you implemented the scrollable list
            // otherwise use SelectTower(0);
            if (this.Controls.ContainsKey("_flowTowerPanel"))
            {
                // If using the scrollable panel logic from previous steps
                InitializeTowerSelector();
                SelectTower(0);
            }
            else
            {
                SelectTower(0);
            }

            // D. Init Game Loop
            _gameTimer = new Timer();
            _gameTimer.Interval = 16; // ~60 FPS
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // E. Register Input Event (This function is in Input.cs)
            this.MouseClick += OnBoardClick;
        }

        // Default Constructor (to avoid Designer errors)
        public GameLevel1() : this(1) { }

        // =========================================================
        // 3. GAME LOOP (HEART OF THE GAME)
        // =========================================================
        private void GameLoop(object sender, EventArgs e)
        {
            if (_isPaused) return;

            // A. Update Game Logic
            // GameManager handles multiplication with GameSpeed (x2 Speed)
            GameManager.Instance.Update(0.016f);

            // B. Red Screen Effect Logic (When losing lives)
            if (GameManager.Instance.PlayerLives < _lastLives)
            {
                _hurtTimer = 10;
                _lastLives = GameManager.Instance.PlayerLives;
                SoundManager.Play("lose");
            }
            if (_hurtTimer > 0) _hurtTimer--;

            // C. Check Victory Condition
            if (GameManager.Instance.IsVictory)
            {
                _gameTimer.Stop();
                // Show Victory Form
                // (Assuming MaxLives = 20 for star calculation)
                using (var vicForm = new VictoryForm(
                    GameManager.Instance.LevelMgr.CurrentLevelId,
                    GameManager.Instance.PlayerLives, 20))
                {
                    vicForm.ShowDialog();
                }
                this.Close(); // Return to menu after closing victory board
                return;
            }

            // D. Update Start/Auto Wave Button State
            UpdateWaveButtonState();

            // E. Redraw screen (Calls OnPaint in Render.cs)
            this.Invalidate();
        }

        // Logic for controlling Start Wave Button (Off/On/Auto)
        private void UpdateWaveButtonState()
        {
            var waveMgr = GameManager.Instance.WaveMgr;

            // Condition: Wave has stopped spawning AND all enemies on board are dead
            bool isWaveClear = !waveMgr.IsWaveRunning && GameManager.Instance.Enemies.Count == 0;

            if (isWaveClear)
            {
                if (GameManager.Instance.IsAutoWave)
                {
                    // Auto Mode: Disable button, show waiting text
                    if (_btnStartWave.Enabled)
                    {
                        _btnStartWave.Enabled = false;
                        _btnStartWave.Text = "AUTO...";
                        _btnStartWave.BackColor = Color.Gray;
                    }
                }
                else
                {
                    // Manual Mode: Enable button for player to click
                    if (!_btnStartWave.Enabled)
                    {
                        _btnStartWave.Enabled = true;
                        _btnStartWave.Text = "NEXT WAVE >>";
                        _btnStartWave.BackColor = Color.LightGreen;
                    }
                }
            }
            else
            {
                // Wave is running
                if (_btnStartWave.Enabled)
                {
                    _btnStartWave.Enabled = false;
                    _btnStartWave.Text = "RUNNING...";
                    _btnStartWave.BackColor = Color.Orange;
                }
            }
        }
    }
}