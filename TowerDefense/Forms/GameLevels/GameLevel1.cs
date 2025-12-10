using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Data;
using TowerDefense.Entities.Towers;

namespace TowerDefense.Forms.GameLevels
{
    public partial class GameLevel1 : Form
    {
        private Timer _gameTimer;
        private bool _isPaused = false;

        // --- CÁC BIẾN CHO UI NÂNG CẤP THÁP (DYNAMIC) ---
        private Panel _pnlTowerActions;
        private Label _lblTowerInfo;
        private Button _btnUpgrade;
        private Button _btnSell;
        private Tower _selectedTower; // Tháp đang được chọn

        // --- CONSTRUCTOR ---
        public GameLevel1(int levelId)
        {
            this.Text = $"Tower Defense - Level {levelId}";
            InitializeComponent();          // Load Designer
            InitializeDynamicControls();    // Load bảng Upgrade

            SoundManager.LoadSounds(); // Nạp âm thanh vào RAM

            // Start Game Logic
            GameManager.Instance.StartGame(levelId);
            SelectTower(1);

            // Game Loop
            _gameTimer = new Timer();
            _gameTimer.Interval = 16;
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // Event Click
            this.MouseClick += OnBoardClick;
        }

        // Constructor mặc định (để tránh lỗi Designer)
        public GameLevel1() : this(1) { }

        // --- KHỞI TẠO CÁC CONTROL ĐỘNG (BẢNG NÂNG CẤP) ---
        private void InitializeDynamicControls()
        {
            _pnlTowerActions = new Panel();
            _pnlTowerActions.Size = new System.Drawing.Size(200, 100);
            _pnlTowerActions.BackColor = Color.FromArgb(220, 0, 0, 0); // Đen trong suốt
            _pnlTowerActions.Visible = false;
            this.Controls.Add(_pnlTowerActions);

            _lblTowerInfo = new Label { ForeColor = Color.White, Location = new Point(10, 10), AutoSize = true, Text = "Info" };
            _pnlTowerActions.Controls.Add(_lblTowerInfo);

            _btnUpgrade = new Button { Text = "UPGRADE", Location = new Point(10, 40), Size = new System.Drawing.Size(80, 40), BackColor = Color.LightGreen };
            _btnUpgrade.Click += (s, e) => PerformUpgrade();
            _pnlTowerActions.Controls.Add(_btnUpgrade);

            _btnSell = new Button { Text = "SELL", Location = new Point(100, 40), Size = new System.Drawing.Size(80, 40), BackColor = Color.Red, ForeColor = Color.White };
            _btnSell.Click += (s, e) => PerformSell();
            _pnlTowerActions.Controls.Add(_btnSell);

            // Nút đóng nhanh
            Button btnClose = new Button { Text = "X", Location = new Point(170, 0), Size = new System.Drawing.Size(30, 25), BackColor = Color.Gray };
            btnClose.Click += (s, e) => { _pnlTowerActions.Visible = false; _selectedTower = null; };
            _pnlTowerActions.Controls.Add(btnClose);
        }

        // ====================================================
        // PHẦN 1: GAME LOOP
        // ====================================================
        private void GameLoop(object sender, EventArgs e)
        {
            if (_isPaused) return;

            GameManager.Instance.Update(0.016f);

            // Logic bật lại nút Start Wave
            var waveMgr = GameManager.Instance.WaveMgr;
            if (!waveMgr.IsWaveRunning && GameManager.Instance.Enemies.Count == 0)
            {
                if (!_btnStartWave.Enabled)
                {
                    _btnStartWave.Enabled = true;
                    _btnStartWave.Text = "NEXT WAVE >>";
                    _btnStartWave.BackColor = Color.LightGreen;
                }
            }

            this.Invalidate();
        }

        // ====================================================
        // PHẦN 2: XỬ LÝ SỰ KIỆN (INPUT)
        // ====================================================

        private void OnBoardClick(object sender, MouseEventArgs e)
        {
            if (_isPaused || e.Button != MouseButtons.Left) return;

            // 1. Kiểm tra xem có click vào tháp nào không? (ĐỂ NÂNG CẤP)
            foreach (var tower in GameManager.Instance.Towers)
            {
                // Hit test đơn giản: Click trong phạm vi 20px quanh tháp
                if (Math.Abs(tower.X - e.X) < 20 && Math.Abs(tower.Y - e.Y) < 20)
                {
                    SelectTowerOnBoard(tower);
                    return; // Đã chọn tháp -> Không xây
                }
            }

            // 2. Nếu không click vào tháp -> Thử Xây tháp mới
            if (e.Y > 480) return; // Không click vào vùng UI

            _pnlTowerActions.Visible = false; // Ẩn bảng nâng cấp
            _selectedTower = null;

            if (!GameManager.Instance.TryBuildTower(e.X, e.Y))
            {
                MessageBox.Show("Không đủ tiền!");
            }
            else
            {
                // --- THÊM DÒNG NÀY ---
                SoundManager.Play("build"); // Phát tiếng "Keng" khi xây
            }
        }

        private void SelectTowerOnBoard(Tower tower)
        {
            _selectedTower = tower;

            // Tính vị trí hiện bảng
            int pX = (int)tower.X + 25;
            int pY = (int)tower.Y - 50;
            if (pX + 200 > 800) pX = (int)tower.X - 225;
            if (pY < 0) pY = 0;

            _pnlTowerActions.Location = new Point(pX, pY);
            _pnlTowerActions.Visible = true;
            _pnlTowerActions.BringToFront();

            UpdateUpgradePanelUI();
        }

        private void UpdateUpgradePanelUI()
        {
            if (_selectedTower == null) return;
            _lblTowerInfo.Text = $"{_selectedTower.Name} (Lv.{_selectedTower.Level})\nDmg: {_selectedTower.BaseDamage}";

            if (_selectedTower.Level >= 3)
            {
                _btnUpgrade.Text = "MAX";
                _btnUpgrade.Enabled = false;
            }
            else
            {
                _btnUpgrade.Text = $"UPGRADE\n{_selectedTower.UpgradeCost} G";
                _btnUpgrade.Enabled = GameManager.Instance.PlayerMoney >= _selectedTower.UpgradeCost;
            }

            _btnSell.Text = $"SELL\n+{_selectedTower.SellValue} G";
        }

        private void PerformUpgrade()
        {
            if (_selectedTower != null && GameManager.Instance.PlayerMoney >= _selectedTower.UpgradeCost)
            {
                GameManager.Instance.PlayerMoney -= _selectedTower.UpgradeCost;
                _selectedTower.Upgrade();
                UpdateUpgradePanelUI();
            }
        }

        private void PerformSell()
        {
            if (_selectedTower != null)
            {
                GameManager.Instance.PlayerMoney += _selectedTower.SellValue;
                GameManager.Instance.Towers.Remove(_selectedTower);
                _pnlTowerActions.Visible = false;
                _selectedTower = null;
            }
        }

        // --- SỰ KIỆN CÁC NÚT UI ---
        private void BtnStartWave_Click(object sender, EventArgs e)
        {
            if (!GameManager.Instance.WaveMgr.IsWaveRunning)
            {
                GameManager.Instance.WaveMgr.StartNextWave();
                _btnStartWave.Enabled = false;
                _btnStartWave.Text = "RUNNING...";
                _btnStartWave.BackColor = Color.Orange;
            }
            this.Focus();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            _isPaused = !_isPaused;
            _btnPause.Text = _isPaused ? ">" : "II";
            if (_isPaused) _gameTimer.Stop(); else _gameTimer.Start();
            this.Focus();
        }

        private void SelectTower(int type)
        {
            GameManager.Instance.SelectedTowerType = type;
            _btnSelectArcher.BackColor = (type == 1) ? Color.Yellow : Color.LightBlue;
            _btnSelectCannon.BackColor = (type == 2) ? Color.Yellow : Color.Gray;
            this.Focus();
        }

        private void BtnMeteor_Click(object sender, EventArgs e)
        {
            GameManager.Instance.SkillMgr.CastMeteor();
            this.Focus();
        }

        private void BtnFreeze_Click(object sender, EventArgs e)
        {
            GameManager.Instance.SkillMgr.CastFreeze();
            this.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveLoadSystem.SaveGame();
            MessageBox.Show("Saved!");
            this.Focus();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            SaveLoadSystem.LoadGame();
            MessageBox.Show("Loaded!");
            this.Focus();
        }

        // ====================================================
        // PHẦN 3: RENDER
        // ====================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Nền
            Image bg = ResourceManager.GetImage("Grass");
            if (bg != null) g.DrawImage(bg, 0, 0, 800, 600);
            else g.Clear(Color.LightGreen);

            // 2. Đường đi
            var path = GameManager.Instance.CurrentMapPath;
            if (path != null && path.Count > 1)
            {
                using (Pen p = new Pen(Color.FromArgb(180, 210, 180, 140), 40))
                {
                    p.LineJoin = LineJoin.Round;
                    g.DrawLines(p, path.ToArray());
                }
                // Điểm đầu cuối
                g.FillEllipse(Brushes.DarkGray, path[0].X - 20, path[0].Y - 20, 40, 40);
                g.FillRectangle(Brushes.Purple, path[path.Count - 1].X - 25, path[path.Count - 1].Y - 25, 50, 50);
            }

            // 3. Vẽ Tháp & Quái & Đạn
            GameManager.Instance.Render(g);

            // 4. Vẽ vùng chọn tháp (nếu có)
            if (_selectedTower != null && _selectedTower.IsActive)
            {
                // Giả lập tầm bắn tăng theo level
                float r = 200f * (1 + (_selectedTower.Level - 1) * 0.2f);
                using (Pen p = new Pen(Color.White, 2)) { p.DashStyle = DashStyle.Dash; g.DrawEllipse(p, _selectedTower.X - r, _selectedTower.Y - r, r * 2, r * 2); }
            }

            // 5. Vẽ HUD
            DrawHUD(g);
        }

        private void DrawHUD(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 0)), 0, 0, 800, 40);
            Font f = new Font("Arial", 14, FontStyle.Bold);
            g.DrawString($"Wave: {GameManager.Instance.WaveMgr.CurrentWave}", f, Brushes.White, 20, 8);
            g.DrawString($"Gold: {GameManager.Instance.PlayerMoney}", f, Brushes.Gold, 200, 8);
            g.DrawString($"Lives: {GameManager.Instance.PlayerLives}", f, Brushes.Red, 400, 8);

            // Update UI nút skill
            var sm = GameManager.Instance.SkillMgr;
            if (sm.MeteorCooldown > 0) { _btnSkillMeteor.Enabled = false; _btnSkillMeteor.Text = $"{sm.MeteorCooldown:0.0}"; _btnSkillMeteor.BackColor = Color.Gray; }
            else { _btnSkillMeteor.Enabled = true; _btnSkillMeteor.Text = "METEOR"; _btnSkillMeteor.BackColor = Color.OrangeRed; }

            if (sm.FreezeCooldown > 0) { _btnSkillFreeze.Enabled = false; _btnSkillFreeze.Text = $"{sm.FreezeCooldown:0.0}"; _btnSkillFreeze.BackColor = Color.Gray; }
            else { _btnSkillFreeze.Enabled = true; _btnSkillFreeze.Text = "FREEZE"; _btnSkillFreeze.BackColor = Color.Cyan; }

            // Game Over Check
            if (GameManager.Instance.PlayerLives <= 0) HandleGameOver(g);
        }

        private void HandleGameOver(Graphics g)
        {
            using (SolidBrush b = new SolidBrush(Color.FromArgb(180, 0, 0, 0))) g.FillRectangle(b, 0, 0, 800, 600);
            g.DrawString("GAME OVER", new Font("Arial", 50, FontStyle.Bold), Brushes.Red, 200, 200);
            _gameTimer.Stop();

            string name = Prompt.ShowDialog("Nhập tên lưu điểm:", "Game Over");
            if (string.IsNullOrEmpty(name)) name = "Unknown";

            int score = (GameManager.Instance.PlayerMoney / 10) + (GameManager.Instance.WaveMgr.CurrentWave * 100);
            HighScoreManager.SaveScore(name, score);
            HistoryManager.SaveLog(false, GameManager.Instance.WaveMgr.CurrentWave);

            this.Close();
        }
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form() { Width = 300, Height = 180, FormBorderStyle = FormBorderStyle.FixedDialog, Text = caption, StartPosition = FormStartPosition.CenterScreen };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, AutoSize = true };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 240 };
            Button confirmation = new Button() { Text = "Ok", Left = 180, Width = 80, Top = 80, DialogResult = DialogResult.OK };
            prompt.Controls.Add(textBox); prompt.Controls.Add(confirmation); prompt.Controls.Add(textLabel);
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}