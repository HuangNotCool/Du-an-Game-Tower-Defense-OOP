using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;
using TowerDefense.Forms.Reports; // Namespace này sẽ tạo ở bước sau

namespace TowerDefense.Forms
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent(); // Nếu bạn tạo bằng Designer

            // Cấu hình Form
            this.Text = "Tower Defense Game";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightSlateGray;

            // Tiêu đề
            Label lblTitle = new Label();
            lblTitle.Text = "DEFENSE OF THE TOWER";
            lblTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(100, 50);
            lblTitle.ForeColor = Color.White;
            this.Controls.Add(lblTitle);

            // Nút Play
            Button btnPlay = CreateMenuButton("PLAY GAME", 150);
            btnPlay.Click += (s, e) =>
            {
                // Mở Form Game
                GameLevel1 game = new GameLevel1();
                this.Hide(); // Ẩn menu
                game.ShowDialog(); // Chờ chơi xong
                this.Show(); // Hiện lại menu khi tắt game
            };

            // Nút HighScore
            Button btnScore = CreateMenuButton("HIGH SCORES", 220);
            btnScore.Click += (s, e) =>
            {
                // Mở Form Báo cáo
                HighScoreForm report = new HighScoreForm();
                report.ShowDialog();
            };
            // ... Trong Constructor MainMenuForm ...

            // Nút Bestiary (Từ điển)
            Button btnBestiary = CreateMenuButton("BESTIARY", 290); // Chỉnh lại tọa độ Y
            btnBestiary.Click += (s, e) => { new Forms.Reports.BestiaryForm().ShowDialog(); };

            // Nút History (Lịch sử)
            Button btnHistory = CreateMenuButton("HISTORY", 360); // Chỉnh lại tọa độ Y
            btnHistory.Click += (s, e) => { new Forms.Reports.HistoryForm().ShowDialog(); };

            // Nút Exit (Đẩy xuống dưới cùng)
            Button btnExit = CreateMenuButton("EXIT", 430);
            btnExit.Click += (s, e) => Application.Exit();

            // Chỉnh lại chiều cao Form cho đủ chỗ
            this.Size = new Size(600, 550);
        }

        // Hàm hỗ trợ tạo nút cho nhanh
        private Button CreateMenuButton(string text, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(200, 50);
            btn.Location = new Point(190, y);
            btn.Font = new Font("Arial", 12, FontStyle.Bold);
            btn.BackColor = Color.White;
            this.Controls.Add(btn);
            return btn;
        }

    }
}