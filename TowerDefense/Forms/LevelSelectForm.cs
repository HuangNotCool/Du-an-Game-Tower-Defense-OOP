using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Forms.GameLevels;

namespace TowerDefense.Forms
{
    public partial class LevelSelectForm : Form
    {
        public LevelSelectForm()
        {
            InitializeComponent(); // Nếu dùng Designer (hoặc hàm giả bên dưới)
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "SELECT BATTLEFIELD";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 40); // Nền tối

            // Tiêu đề
            Label lblTitle = new Label();
            lblTitle.Text = "CHOOSE YOUR MAP";
            lblTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            lblTitle.ForeColor = Color.Gold;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(130, 30);
            this.Controls.Add(lblTitle);

            // Nút Level 1
            CreateLevelButton(1, "LEVEL 1\n(Easy)", 50, Color.LightGreen, "Đường thẳng cơ bản.\nThích hợp cho người mới.");

            // Nút Level 2
            CreateLevelButton(2, "LEVEL 2\n(Normal)", 220, Color.Orange, "Đường chữ U.\nCần chiến thuật đặt tháp.");

            // Nút Level 3
            CreateLevelButton(3, "LEVEL 3\n(Hard)", 390, Color.OrangeRed, "Đường Ziczac.\nQuái rất đông và nguy hiểm!");

            // Nút Back
            Button btnBack = new Button { Text = "BACK TO MENU", Size = new Size(150, 40), Location = new Point(220, 400), BackColor = Color.Gray, ForeColor = Color.White };
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);
        }

        private void CreateLevelButton(int id, string title, int x, Color color, string desc)
        {
            // Nút chính
            Button btn = new Button();
            btn.Text = title;
            btn.Size = new Size(140, 100);
            btn.Location = new Point(x, 100);
            btn.BackColor = color;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Arial", 12, FontStyle.Bold);
            btn.Click += (s, e) => OpenGameLevel(id);
            this.Controls.Add(btn);

            // Mô tả bên dưới
            Label lblDesc = new Label();
            lblDesc.Text = desc;
            lblDesc.ForeColor = Color.WhiteSmoke;
            lblDesc.Location = new Point(x, 210);
            lblDesc.Size = new Size(140, 100);
            lblDesc.TextAlign = ContentAlignment.TopCenter;
            this.Controls.Add(lblDesc);
        }

        private void OpenGameLevel(int levelId)
        {
            this.Hide(); // Ẩn bảng chọn

            // Mở Game Form và truyền ID màn chơi vào
            GameLevel1 game = new GameLevel1(levelId);
            game.ShowDialog(); // Chờ chơi xong

            this.Show(); // Hiện lại bảng chọn
        }

        // Hàm giả để tránh lỗi Designer
        private void InitializeComponent() { }
    }
}