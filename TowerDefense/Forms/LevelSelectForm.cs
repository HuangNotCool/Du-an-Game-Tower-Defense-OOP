using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;
using TowerDefense.Utils; // Để dùng GameButton

namespace TowerDefense.Forms
{
    public partial class LevelSelectForm : Form
    {
        private FlowLayoutPanel _pnlLevels; // Panel chứa danh sách nút

        public LevelSelectForm()
        {
            // 1. Cấu hình Form
            this.Text = "SELECT BATTLEFIELD";
            this.Size = new Size(850, 600); // Form to hơn để chứa đủ 10 nút
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 40);

            InitializeComponent(); // Nếu dùng Designer (hoặc bỏ qua)
            SetupUI();
        }

        private void SetupUI()
        {
            // Nền Gradient
            this.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                       Color.FromArgb(40, 40, 60), Color.FromArgb(10, 10, 20), 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Tiêu đề
            Label lbl = new Label
            {
                Text = "CAMPAIGN MAP",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(300, 20),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lbl);

            // Container chứa nút (Tự động xuống dòng)
            _pnlLevels = new FlowLayoutPanel
            {
                Location = new Point(50, 80),
                Size = new Size(750, 400),
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            this.Controls.Add(_pnlLevels);

            // --- TẠO TỰ ĐỘNG 10 LEVEL BUTTONS ---
            for (int i = 1; i <= 10; i++)
            {
                CreateLevelButton(i);
            }

            // Nút Back
            GameButton btnBack = new GameButton
            {
                Text = "BACK TO MENU",
                Size = new Size(200, 50),
                Location = new Point(325, 500),
                Color1 = Color.DarkSlateGray,
                Color2 = Color.Black
            };
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);
        }

        private void CreateLevelButton(int id)
        {
            // Xác định màu sắc theo độ khó
            Color baseColor = Color.LightGreen; // Dễ (1-3)
            string difficulty = "Easy";

            if (id >= 4 && id <= 6) { baseColor = Color.Orange; difficulty = "Normal"; }
            else if (id >= 7 && id <= 9) { baseColor = Color.OrangeRed; difficulty = "Hard"; }
            else if (id == 10) { baseColor = Color.Purple; difficulty = "EXTREME"; }

            GameButton btn = new GameButton
            {
                Text = $"LEVEL {id}\n{difficulty}",
                Size = new Size(130, 100),
                Color1 = ControlPaint.Light(baseColor),
                Color2 = baseColor,
                BorderRadius = 20,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Margin = new Padding(10) // Khoảng cách giữa các nút
            };

            btn.Click += (s, e) => OpenGameLevel(id);

            _pnlLevels.Controls.Add(btn);
        }

        private void OpenGameLevel(int levelId)
        {
            this.Hide();
            GameLevel1 game = new GameLevel1(levelId);
            game.ShowDialog();
            this.Show();
        }

        private void InitializeComponent() { }
    }
}