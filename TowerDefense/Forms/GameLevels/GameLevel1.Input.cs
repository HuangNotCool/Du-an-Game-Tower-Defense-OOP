using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Data;
using TowerDefense.Entities.Towers;
using TowerDefense.Configs;
using TowerDefense.Utils;

namespace TowerDefense.Forms.GameLevels
{
    public partial class GameLevel1
    {
        // --- INITIALIZE TOWER LIST (Fixes CS0103) ---
        private void InitializeTowerSelector()
        {
            if (_flowTowerPanel == null) return;
            _flowTowerPanel.Controls.Clear();

            // Iterate through all tower types in GameConfig
            for (int i = 0; i < GameConfig.Towers.Length; i++)
            {
                var stat = GameConfig.Towers[i];

                // Create custom button
                GameButton btn = new GameButton();
                btn.Size = new Size(65, 65);
                btn.BorderRadius = 15;
                btn.Tag = i; // Store tower ID in Tag
                btn.Margin = new Padding(5);

                // Set Image
                Image towerImg = ResourceManager.GetImage(stat.Name);
                if (towerImg != null)
                {
                    btn.Image = ResizeImage(towerImg, 32, 32);
                    btn.TextImageRelation = TextImageRelation.ImageAboveText;
                }

                btn.Text = $"{stat.Price}";
                btn.Font = new Font("Arial", 8, FontStyle.Bold);
                btn.ForeColor = Color.White;

                // Default Colors
                btn.Color1 = ControlPaint.Dark(stat.Color);
                btn.Color2 = Color.Black;

                // Click Event
                btn.Click += (s, e) => SelectTower((int)btn.Tag);

                _flowTowerPanel.Controls.Add(btn);
            }
        }

        // Helper: Resize image
        private Image ResizeImage(Image img, int w, int h)
        {
            Bitmap b = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, w, h);
            }
            return b;
        }

        // --- SELECT TOWER LOGIC (Fixes missing buttons) ---
        private void SelectTower(int typeId)
        {
            GameManager.Instance.SelectedTowerType = typeId;

            // Iterate through dynamic buttons in panel to highlight selected one
            if (_flowTowerPanel != null)
            {
                foreach (Control c in _flowTowerPanel.Controls)
                {
                    if (c is GameButton btn)
                    {
                        int id = (int)btn.Tag;
                        var stat = GameConfig.Towers[id];

                        if (id == typeId)
                        {
                            // Selected: Bright
                            btn.Color1 = stat.Color;
                            btn.Color2 = ControlPaint.Light(stat.Color);
                        }
                        else
                        {
                            // Not Selected: Dark
                            btn.Color1 = ControlPaint.Dark(stat.Color);
                            btn.Color2 = Color.Black;
                        }
                        btn.Invalidate(); // Redraw button
                    }
                }
            }
            this.Focus();
        }

        // --- UPGRADE PANEL UI (DYNAMIC) ---
        private void InitializeDynamicControls()
        {
            _pnlTowerActions = new Panel { Size = new Size(200, 110), BackColor = Color.FromArgb(220, 0, 0, 0), Visible = false };
            this.Controls.Add(_pnlTowerActions);

            _lblTowerInfo = new Label { ForeColor = Color.White, Location = new Point(10, 10), AutoSize = true, Text = "Info" };
            _pnlTowerActions.Controls.Add(_lblTowerInfo);

            _btnUpgrade = new Button { Text = "UPGRADE", Location = new Point(10, 40), Size = new Size(80, 40), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            _btnUpgrade.Click += (s, e) => PerformUpgrade();
            _pnlTowerActions.Controls.Add(_btnUpgrade);

            _btnSell = new Button { Text = "SELL", Location = new Point(100, 40), Size = new Size(80, 40), BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            _btnSell.Click += (s, e) => PerformSell();
            _pnlTowerActions.Controls.Add(_btnSell);

            Button btnClose = new Button { Text = "X", Location = new Point(170, 0), Size = new Size(30, 25), BackColor = Color.Gray, FlatStyle = FlatStyle.Flat };
            btnClose.Click += (s, e) => DeselectTower();
            _pnlTowerActions.Controls.Add(btnClose);
        }

        // =========================================================
        // MOUSE CLICK HANDLERS
        // =========================================================
        private void OnBoardClick(object sender, MouseEventArgs e)
        {
            if (_isPaused || e.Button != MouseButtons.Left) return;

            // 1. Check Click Tower (Upgrade)
            foreach (var tower in GameManager.Instance.Towers)
            {
                if (Math.Abs(tower.X - e.X) < 20 && Math.Abs(tower.Y - e.Y) < 20)
                {
                    SelectTowerOnBoard(tower);
                    return;
                }
            }

            // 2. Check Build New Tower
            if (e.Y > 480) return; // Ignore clicks in UI area

            DeselectTower();

            int gridSize = 40;
            int snapX = (e.X / gridSize) * gridSize + (gridSize / 2);
            int snapY = (e.Y / gridSize) * gridSize + (gridSize / 2);

            if (GameManager.Instance.CanPlaceTower(snapX, snapY))
            {
                if (GameManager.Instance.TryBuildTower(snapX, snapY))
                {
                    SoundManager.Play("build");
                    GameManager.Instance.ShowFloatingText("-$$$", snapX, snapY - 20, Color.Yellow);
                }
                else
                {
                    GameManager.Instance.ShowFloatingText("Need Gold!", snapX, snapY, Color.Red);
                }
            }
            else
            {
                GameManager.Instance.ShowFloatingText("X", snapX, snapY, Color.Red);
            }
        }

        // --- BUTTON EVENTS ---
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

        private void BtnSpeed_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.GameSpeed == 1.0f)
            {
                GameManager.Instance.GameSpeed = 2.0f;
                ((GameButton)sender).Text = "x2"; ((GameButton)sender).BackColor = Color.OrangeRed;
            }
            else
            {
                GameManager.Instance.GameSpeed = 1.0f;
                ((GameButton)sender).Text = "x1"; ((GameButton)sender).BackColor = Color.Khaki;
            }
            this.Focus();
        }

        private void BtnAutoWave_Click(object sender, EventArgs e)
        {
            GameManager.Instance.IsAutoWave = !GameManager.Instance.IsAutoWave;
            if (GameManager.Instance.IsAutoWave)
            {
                ((GameButton)sender).Text = "AUTO: ON"; ((GameButton)sender).BackColor = Color.Green;
            }
            else
            {
                ((GameButton)sender).Text = "AUTO: OFF"; ((GameButton)sender).BackColor = Color.Gray;
            }
            this.Focus();
        }

        private void BtnMeteor_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.SkillMgr.CastMeteor())
                GameManager.Instance.ShowFloatingText("METEOR!!!", 400, 300, Color.OrangeRed);
            this.Focus();
        }

        private void BtnFreeze_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.SkillMgr.CastFreeze())
                GameManager.Instance.ShowFloatingText("FREEZE!!!", 400, 300, Color.Cyan);
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

        // --- UPGRADE HELPER ---
        private void SelectTowerOnBoard(Tower tower)
        {
            _selectedTower = tower;
            int pX = (int)tower.X + 25;
            int pY = (int)tower.Y - 50;
            if (pX + 200 > 800) pX = (int)tower.X - 225;
            if (pY < 0) pY = 0;

            _pnlTowerActions.Location = new Point(pX, pY);
            _pnlTowerActions.Visible = true;
            _pnlTowerActions.BringToFront();
            UpdateUpgradeUI();
        }

        private void DeselectTower()
        {
            _pnlTowerActions.Visible = false;
            _selectedTower = null;
        }

        private void UpdateUpgradeUI()
        {
            if (_selectedTower == null) return;
            _lblTowerInfo.Text = $"{_selectedTower.Name} (Lv.{_selectedTower.Level})\nDmg: {_selectedTower.BaseDamage}";

            if (_selectedTower.Level >= 3)
            {
                _btnUpgrade.Text = "MAX"; _btnUpgrade.Enabled = false; _btnUpgrade.BackColor = Color.Gray;
            }
            else
            {
                _btnUpgrade.Text = $"UPGRADE\n{_selectedTower.UpgradeCost} G";
                _btnUpgrade.Enabled = GameManager.Instance.PlayerMoney >= _selectedTower.UpgradeCost;
                _btnUpgrade.BackColor = _btnUpgrade.Enabled ? Color.LightGreen : Color.Gray;
            }
            _btnSell.Text = $"SELL\n+{_selectedTower.SellValue} G";
        }

        private void PerformUpgrade()
        {
            if (_selectedTower != null && GameManager.Instance.PlayerMoney >= _selectedTower.UpgradeCost)
            {
                GameManager.Instance.PlayerMoney -= _selectedTower.UpgradeCost;
                _selectedTower.Upgrade();
                SoundManager.Play("upgrade");
                GameManager.Instance.ShowFloatingText("UPGRADED!", _selectedTower.X, _selectedTower.Y - 30, Color.Cyan);
                UpdateUpgradeUI();
            }
        }

        private void PerformSell()
        {
            if (_selectedTower != null)
            {
                GameManager.Instance.PlayerMoney += _selectedTower.SellValue;
                GameManager.Instance.ShowFloatingText($"+{_selectedTower.SellValue} G", _selectedTower.X, _selectedTower.Y, Color.Gold);
                GameManager.Instance.Towers.Remove(_selectedTower);
                DeselectTower();
            }
        }
    }
}