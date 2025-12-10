using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;
using TowerDefense.Forms.Reports;

namespace TowerDefense.Forms
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            // 1. Gọi hàm của Visual Studio (trong Designer.cs)
            // Dòng này bắt buộc phải có và phải nằm đầu tiên
            InitializeComponent();

            // 2. Cấu hình giao diện thủ công của chúng ta
            SetupMenuUI();
        }

        private void SetupMenuUI()
        {
            // Cấu hình Form
            this.Text = "Tower Defense Game";
            this.Size = new Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(50, 50, 70);

            // Tiêu đề
            Label lblTitle = new Label();
            lblTitle.Text = "DEFENSE OF THE TOWER";
            lblTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(80, 40);
            lblTitle.ForeColor = Color.Gold;
            this.Controls.Add(lblTitle);

            // Các Nút Chức Năng
            int startY = 120;
            int gap = 70;

            // Nút PLAY
            Button btnPlay = CreateMenuButton("PLAY GAME", startY, Color.OrangeRed);
            // SỬA ĐOẠN CLICK NÀY:
            btnPlay.Click += (s, e) =>
            {
                // Thay vì vào thẳng GameLevel1, ta mở LevelSelectForm
                LevelSelectForm levelSelect = new LevelSelectForm();
                this.Hide();
                levelSelect.ShowDialog();
                this.Show();
            };

            // Nút SHOP
            Button btnShop = CreateMenuButton("SHOP & UPGRADE", startY + gap, Color.Purple);
            btnShop.Click += (s, e) =>
            {
                ShopForm shop = new ShopForm();
                this.Hide();
                shop.ShowDialog();
                this.Show();
            };

            // Nút HIGH SCORES
            Button btnScore = CreateMenuButton("HIGH SCORES", startY + gap * 2, Color.White);
            btnScore.Click += (s, e) => { new HighScoreForm().ShowDialog(); };

            // Nút BESTIARY
            Button btnBestiary = CreateMenuButton("BESTIARY", startY + gap * 3, Color.White);
            btnBestiary.Click += (s, e) => { new BestiaryForm().ShowDialog(); };

            // Nút HISTORY
            Button btnHistory = CreateMenuButton("MATCH HISTORY", startY + gap * 4, Color.White);
            btnHistory.Click += (s, e) => { new HistoryForm().ShowDialog(); };

            // Nút EXIT
            Button btnExit = CreateMenuButton("EXIT GAME", startY + gap * 5, Color.Gray);
            btnExit.Click += (s, e) => Application.Exit();
        }

        private Button CreateMenuButton(string text, int y, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(250, 50);
            btn.Location = new Point(170, y);
            btn.Font = new Font("Arial", 14, FontStyle.Bold);
            btn.BackColor = color;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.Yellow;
            btn.MouseLeave += (s, e) => btn.BackColor = color;

            this.Controls.Add(btn);
            return btn;
        }
    }
}