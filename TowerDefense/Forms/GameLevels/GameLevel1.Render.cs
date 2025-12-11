using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Configs;

namespace TowerDefense.Forms.GameLevels
{
    public partial class GameLevel1
    {
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
            DrawPath(g);

            // 3. Thực thể
            GameManager.Instance.Render(g);

            // 4. Vòng tròn tầm bắn
            if (_selectedTower != null && _selectedTower.IsActive)
            {
                // Ước lượng Range (tốt nhất là thêm prop Range vào Tower.cs)
                float r = 200f;
                using (Pen p = new Pen(Color.White, 2)) { p.DashStyle = DashStyle.Dash; g.DrawEllipse(p, _selectedTower.X - r, _selectedTower.Y - r, r * 2, r * 2); }
            }

            // 5. Hiệu ứng Màn hình Đỏ
            if (_hurtTimer > 0)
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(100, 255, 0, 0)))
                {
                    g.FillRectangle(b, 0, 0, 800, 600);
                }
            }

            // 6. HUD
            DrawHUD(g);
        }

        private void DrawPath(Graphics g)
        {
            var path = GameManager.Instance.CurrentMapPath;
            if (path != null && path.Count > 1)
            {
                using (Pen p = new Pen(Color.FromArgb(180, 210, 180, 140), 40))
                {
                    p.LineJoin = LineJoin.Round;
                    g.DrawLines(p, path.ToArray());
                }
                g.FillEllipse(Brushes.DarkGray, path[0].X - 20, path[0].Y - 20, 40, 40);
                var end = path[path.Count - 1];
                g.FillRectangle(Brushes.Purple, end.X - 25, end.Y - 25, 50, 50);
            }
        }

        private void DrawHUD(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0, 0)), 0, 0, 800, 40);
            Font f = new Font("Arial", 14, FontStyle.Bold);
            g.DrawString($"Wave: {GameManager.Instance.WaveMgr.CurrentWave}", f, Brushes.White, 20, 8);
            g.DrawString($"Gold: {GameManager.Instance.PlayerMoney}", f, Brushes.Gold, 200, 8);
            g.DrawString($"Lives: {GameManager.Instance.PlayerLives}", f, Brushes.Red, 400, 8);

            // Update UI Skill Cooldown
            var sm = GameManager.Instance.SkillMgr;
            if (sm.MeteorCooldown > 0)
            {
                _btnSkillMeteor.Enabled = false; _btnSkillMeteor.Text = $"{sm.MeteorCooldown:0.0}"; _btnSkillMeteor.BackColor = Color.Gray;
            }
            else
            {
                _btnSkillMeteor.Enabled = true; _btnSkillMeteor.Text = "METEOR"; _btnSkillMeteor.BackColor = Color.OrangeRed;
            }

            if (sm.FreezeCooldown > 0)
            {
                _btnSkillFreeze.Enabled = false; _btnSkillFreeze.Text = $"{sm.FreezeCooldown:0.0}"; _btnSkillFreeze.BackColor = Color.Gray;
            }
            else
            {
                _btnSkillFreeze.Enabled = true; _btnSkillFreeze.Text = "FREEZE"; _btnSkillFreeze.BackColor = Color.Cyan;
            }

            // Game Over
            if (GameManager.Instance.PlayerLives <= 0) HandleGameOver(g);
        }

        // Trong hàm HandleGameOver(Graphics g) hoặc nơi bạn xử lý thua

        private void HandleGameOver(Graphics g)
        {
            _gameTimer.Stop();

            // --- SỬ DỤNG FORM GAME OVER MỚI ---
            // Truyền vào số Wave và Tiền để tính điểm
            using (var gameOverForm = new GameOverForm(GameManager.Instance.WaveMgr.CurrentWave, GameManager.Instance.PlayerMoney))
            {
                if (gameOverForm.ShowDialog() == DialogResult.OK)
                {
                    string name = gameOverForm.PlayerName;

                    // Tính điểm (Logic tính điểm nên thống nhất 1 chỗ, ở đây tính lại cho chắc)
                    int score = (GameManager.Instance.PlayerMoney / 10) + (GameManager.Instance.WaveMgr.CurrentWave * 100);

                    // Lưu dữ liệu
                    Managers.HighScoreManager.SaveScore(name, score);
                    Managers.HistoryManager.SaveLog(false, GameManager.Instance.WaveMgr.CurrentWave);
                }
            }

            this.Close(); // Đóng màn chơi, quay về Menu
        }
    }
}