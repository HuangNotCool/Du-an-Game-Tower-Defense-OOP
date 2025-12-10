using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Data;

namespace TowerDefense.Forms.GameLevels
{
    // 'partial' để ghép với file Designer
    public partial class GameLevel1 : Form
    {
        private Timer _gameTimer;

        public GameLevel1()
        {
            // 1. Gọi giao diện từ Designer
            InitializeComponent();

            // 2. Cấu hình Logic Game ban đầu
            GameManager.Instance.StartGame();

            // Mặc định chọn tháp Archer (Type 1)
            SelectTower(1);

            // 3. Khởi tạo Game Loop (60 FPS)
            _gameTimer = new Timer();
            _gameTimer.Interval = 16;
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // 4. Đăng ký sự kiện click vào bàn cờ (xây tháp)
            this.MouseClick += OnBoardClick;
        }

        // ====================================================
        // PHẦN 1: GAME LOOP (VÒNG LẶP CHÍNH)
        // ====================================================
        private void GameLoop(object sender, EventArgs e)
        {
            // A. Cập nhật dữ liệu (Logic)
            GameManager.Instance.Update(0.016f);

            // B. Logic nút Start Wave (Tự động bật lại khi hết quái)
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

            // C. Vẽ lại màn hình (Gọi OnPaint)
            this.Invalidate();
        }

        // ====================================================
        // PHẦN 2: XỬ LÝ SỰ KIỆN (USER INPUT)
        // ====================================================

        // Xử lý nút Bắt đầu Wave
        private void BtnStartWave_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.WaveMgr.IsWaveRunning) return;

            GameManager.Instance.WaveMgr.StartNextWave();

            _btnStartWave.Enabled = false;
            _btnStartWave.Text = "WAVE RUNNING...";
            _btnStartWave.BackColor = Color.Orange;
            this.Focus(); // Trả focus về Form để nhận phím tắt nếu có
        }

        // Xử lý nút Chọn Tháp (Archer/Cannon)
        private void SelectTower(int type)
        {
            GameManager.Instance.SelectedTowerType = type;

            // Cập nhật màu nút để biết đang chọn cái nào
            _btnSelectArcher.BackColor = (type == 1) ? Color.Yellow : Color.LightBlue;
            _btnSelectCannon.BackColor = (type == 2) ? Color.Yellow : Color.Gray;
            this.Focus();
        }

        // Xử lý click chuột trên bản đồ để Xây tháp
        private void OnBoardClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Chặn xây đè lên khu vực UI điều khiển (y > 480)
                if (e.Y > 480) return;

                if (!GameManager.Instance.TryBuildTower(e.X, e.Y))
                {
                    MessageBox.Show("Không đủ tiền!");
                }
            }
        }

        // Xử lý Skill Mưa Thiên Thạch
        private void BtnMeteor_Click(object sender, EventArgs e)
        {
            if (GameManager.Instance.SkillMgr.CastMeteor())
            {
                // Có thể thêm âm thanh tại đây
            }
            this.Focus();
        }

        // Xử lý Skill Đóng Băng
        private void BtnFreeze_Click(object sender, EventArgs e)
        {
            GameManager.Instance.SkillMgr.CastFreeze();
            this.Focus();
        }

        // Xử lý Nút Save/Load (Nếu bạn chưa gán trong Designer thì gán ở đây)
        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveLoadSystem.SaveGame();
            MessageBox.Show("Đã lưu game!");
            this.Focus();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            SaveLoadSystem.LoadGame();
            MessageBox.Show("Đã tải game!");
            this.Focus();
        }

        // ====================================================
        // PHẦN 3: RENDER (VẼ HÌNH ẢNH)
        // ====================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Vẽ Nền
            Image bg = ResourceManager.GetImage("Grass");
            if (bg != null) g.DrawImage(bg, 0, 0, 800, 600);
            else g.Clear(Color.LightGreen);

            // 2. Vẽ Đường đi
            DrawPath(g);

            // 3. Vẽ Thực thể (Tháp, Quái, Đạn)
            GameManager.Instance.Render(g);

            // 4. Vẽ HUD (Thông số UI)
            DrawHUD(g);
        }

        private void DrawPath(Graphics g)
        {
            var path = GameManager.Instance.CurrentMapPath;
            if (path != null && path.Count > 1)
            {
                // Vẽ đường đất nâu
                using (Pen pathPen = new Pen(Color.FromArgb(180, 210, 180, 140), 40))
                {
                    pathPen.StartCap = LineCap.Round;
                    pathPen.EndCap = LineCap.Round;
                    pathPen.LineJoin = LineJoin.Round;
                    g.DrawLines(pathPen, path.ToArray());
                }

                // Vẽ điểm Đầu (Start) và Cuối (Base)
                Point start = path[0];
                Point end = path[path.Count - 1];

                g.FillEllipse(Brushes.DarkGray, start.X - 20, start.Y - 20, 40, 40);
                g.DrawString("START", new Font("Arial", 8, FontStyle.Bold), Brushes.White, start.X - 18, start.Y - 5);

                g.FillRectangle(Brushes.Purple, end.X - 25, end.Y - 25, 50, 50);
                g.DrawString("BASE", new Font("Arial", 8, FontStyle.Bold), Brushes.White, end.X - 15, end.Y - 5);
            }
        }

        private void DrawHUD(Graphics g)
        {
            // Thanh thông tin trên cùng
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 0)), 0, 0, 800, 40);

            Font font = new Font("Arial", 14, FontStyle.Bold);
            g.DrawString($"Wave: {GameManager.Instance.WaveMgr.CurrentWave}", font, Brushes.White, 20, 8);
            g.DrawString($"Gold: {GameManager.Instance.PlayerMoney}", font, Brushes.Gold, 200, 8);
            g.DrawString($"Lives: {GameManager.Instance.PlayerLives}", font, Brushes.Red, 400, 8);

            // Cập nhật text hiển thị cooldown trên nút Skill (UI Logic)
            UpdateSkillCooldownUI();

            // Kiểm tra Game Over
            if (GameManager.Instance.PlayerLives <= 0)
            {
                HandleGameOver(g);
            }
        }

        private void UpdateSkillCooldownUI()
        {
            var skill = GameManager.Instance.SkillMgr;

            // Logic cập nhật text cho nút Meteor
            if (skill.MeteorCooldown > 0)
            {
                _btnSkillMeteor.Enabled = false;
                _btnSkillMeteor.Text = $"{skill.MeteorCooldown:0.0}";
                _btnSkillMeteor.BackColor = Color.Gray;
            }
            else
            {
                _btnSkillMeteor.Enabled = true;
                _btnSkillMeteor.Text = "METEOR";
                _btnSkillMeteor.BackColor = Color.OrangeRed;
            }

            // Logic cập nhật text cho nút Freeze
            if (skill.FreezeCooldown > 0)
            {
                _btnSkillFreeze.Enabled = false;
                _btnSkillFreeze.Text = $"{skill.FreezeCooldown:0.0}";
                _btnSkillFreeze.BackColor = Color.Gray;
            }
            else
            {
                _btnSkillFreeze.Enabled = true;
                _btnSkillFreeze.Text = "FREEZE";
                _btnSkillFreeze.BackColor = Color.Cyan;
            }
        }

        private void HandleGameOver(Graphics g)
        {
            // Vẽ màn hình mờ đen
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                g.FillRectangle(brush, 0, 0, 800, 600);
            }

            g.DrawString("GAME OVER", new Font("Arial", 50, FontStyle.Bold), Brushes.Red, 200, 200);

            _gameTimer.Stop(); // Dừng game loop

            // Tính điểm
            int score = (GameManager.Instance.PlayerMoney / 10) + (GameManager.Instance.WaveMgr.CurrentWave * 100);

            // Hỏi tên lưu điểm (Dùng helper Prompt ở dưới)
            string playerName = Prompt.ShowDialog("Bạn đã thua! Nhập tên để lưu điểm:", "Game Over");
            if (string.IsNullOrEmpty(playerName)) playerName = "Unknown";

            // Lưu dữ liệu
            HighScoreManager.SaveScore(playerName, score);
            HistoryManager.SaveLog(false, GameManager.Instance.WaveMgr.CurrentWave);

            this.Close(); // Đóng form quay về menu
        }
    }

    // Class hỗ trợ hiện hộp thoại nhập tên nhanh
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, AutoSize = true, Font = new Font("Arial", 10) };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 280, Font = new Font("Arial", 12) };
            Button confirmation = new Button() { Text = "OK", Left = 200, Width = 80, Top = 90, DialogResult = DialogResult.OK, Height = 30 };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}