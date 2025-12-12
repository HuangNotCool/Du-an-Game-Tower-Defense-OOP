using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;
using TowerDefense.Forms.Reports;
using TowerDefense.Managers;
using TowerDefense.Utils;

namespace TowerDefense.Forms
{
    public partial class MainMenuForm : Form
    {


        public MainMenuForm()
        {
            // 1. Gọi hàm của Visual Studio (trong Designer.cs)
            InitializeComponent();

            // 2. Cấu hình giao diện thủ công của chúng ta
            SetupMenuUI();
            SoundManager.PlayMusic("menu_theme.wav");
        }

        private void SetupMenuUI()
        {
            this.Text = "Tower Defense Game";
            this.Size = new Size(600, 650); // Tăng chiều cao một chút
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- THÊM ĐOẠN NÀY ĐỂ KHÓA KÍCH THƯỚC ---
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Không cho kéo giãn viền
            this.MaximizeBox = false; // Vô hiệu hóa nút Phóng to (Maximize)

            // Tạo màu nền Gradient cho Form Main Menu
            this.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                       Color.FromArgb(30, 30, 60), Color.FromArgb(5, 5, 15), 90F)) // Màu nền tối hơn chút để tăng tương phản
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // ==================================================================================
            // TẠO TIÊU ĐỀ GAME NỔI BẬT (HIỆU ỨNG ĐỔ BÓNG)
            // ==================================================================================
            // Sử dụng Font đậm hơn và to hơn
            Font titleFont = new Font("Arial Black", 28, FontStyle.Bold);
            string titleText = "DEFENSE OF THE TOWER";
            Point titlePos = new Point(30, 40);

            // 1. Lớp BÓNG (Shadow Layer) - Vẽ trước, màu tối, lệch một chút
            Label lblTitleShadow = new Label();
            lblTitleShadow.Text = titleText;
            lblTitleShadow.Font = titleFont;
            lblTitleShadow.AutoSize = true;
            // Lệch 4px sang phải và xuống dưới
            lblTitleShadow.Location = new Point(titlePos.X + 4, titlePos.Y + 4);
            lblTitleShadow.ForeColor = Color.FromArgb(50, 20, 0); // Màu nâu đen làm bóng
            lblTitleShadow.BackColor = Color.Transparent;
            this.Controls.Add(lblTitleShadow);

            // 2. Lớp CHÍNH (Main Layer) - Vẽ sau, màu sáng, nằm đè lên bóng
            Label lblTitleMain = new Label();
            lblTitleMain.Text = titleText;
            lblTitleMain.Font = titleFont;
            lblTitleMain.AutoSize = true;
            lblTitleMain.Location = titlePos;
            lblTitleMain.ForeColor = Color.Gold; // Màu vàng rực rỡ
            lblTitleMain.BackColor = Color.Transparent;
            this.Controls.Add(lblTitleMain);
            // Đảm bảo lớp chính nằm trên cùng
            lblTitleMain.BringToFront();

            // Subtitle (Phụ đề)
            Label lblSub = new Label();
            lblSub.Text = "ULTIMATE STRATEGY GAME";
            lblSub.Font = new Font("Arial", 12, FontStyle.Italic | FontStyle.Bold);
            lblSub.AutoSize = true;
            lblSub.Location = new Point(180, 100); // Dời xuống một chút vì tiêu đề chính to hơn
            lblSub.ForeColor = Color.LightGoldenrodYellow;
            lblSub.BackColor = Color.Transparent;
            this.Controls.Add(lblSub);
            lblSub.BringToFront();
            // ==================================================================================


            // Các Nút Chức Năng
            int startY = 160; // Dời vị trí bắt đầu xuống
            int gap = 75;

            // Nút PLAY
            Button btnPlay = CreateMenuButton("PLAY GAME", startY, Color.OrangeRed);
            btnPlay.Click += (s, e) =>
            {
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
            Button btnScore = CreateMenuButton("HIGH SCORES", startY + gap * 2, Color.DodgerBlue);
            btnScore.Click += (s, e) => { new HighScoreForm().ShowDialog(); };

            // Nút BESTIARY
            Button btnBestiary = CreateMenuButton("BESTIARY", startY + gap * 3, Color.ForestGreen);
            btnBestiary.Click += (s, e) => { new BestiaryForm().ShowDialog(); };

            // Nút HISTORY - ĐÃ ĐỔI MÀU XÁM THÀNH MÀU TEAL (XANH MÒNG KÉT)
            Button btnHistory = CreateMenuButton("MATCH HISTORY", startY + gap * 4, Color.Teal);
            btnHistory.Click += (s, e) => { new HistoryForm().ShowDialog(); };

            // Nút ABOUT (Thêm vào trước nút Exit)
            Button btnAbout = CreateMenuButton("ℹ  ABOUT GAME", startY + gap * 5, Color.DodgerBlue);
            btnAbout.Click += (s, e) => { new AboutForm().ShowDialog(); };

            // Nút EXIT (Đẩy xuống vị trí thứ 6)
            Button btnExit = CreateMenuButton("🚪  EXIT GAME", startY + gap * 6, Color.DarkRed);
            btnExit.Click += (s, e) => Application.Exit();

            // Tăng chiều cao Form lên chút nữa để chứa hết
            this.Size = new Size(600, 750);
        }

        // Cập nhật hàm tạo nút
        private Button CreateMenuButton(string text, int y, Color baseColor)
        {
            // Đổi Button thường thành GameButton
            GameButton btn = new GameButton();
            btn.Text = text;
            btn.Size = new Size(240, 60); // Nút to hơn một chút
            btn.Location = new Point(180, y); // Căn giữa (600/2 - 240/2 = 180)
            btn.Font = new Font("Arial", 12, FontStyle.Bold);

            // Cấu hình màu sắc Gradient
            btn.Color1 = ControlPaint.Light(baseColor);
            btn.Color2 = ControlPaint.Dark(baseColor);
            btn.HoverColor1 = baseColor;
            btn.HoverColor2 = ControlPaint.Light(baseColor);
            btn.BorderRadius = 20; // Bo tròn nhiều hơn

            // Thêm Icon giả lập vào đầu text
            if (text.Contains("PLAY")) btn.Text = "▶  " + text;
            else if (text.Contains("SHOP")) btn.Text = "🛒  " + text;
            else if (text.Contains("SCORES")) btn.Text = "🏆  " + text;
            else if (text.Contains("BESTIARY")) btn.Text = "👹  " + text;
            else if (text.Contains("HISTORY")) btn.Text = "📜  " + text;
            else if (text.Contains("EXIT")) btn.Text = "🚪  " + text;

            this.Controls.Add(btn);
            return btn;
        }
    }
}