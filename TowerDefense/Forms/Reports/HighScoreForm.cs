using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers;

namespace TowerDefense.Forms.Reports
{
    public partial class HighScoreForm : Form
    {
        public HighScoreForm()
        {
            InitializeComponent(); // Nếu dùng Designer

            this.Text = "TOP 10 HIGH SCORES";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            // Tạo bảng dữ liệu (DataGridView)
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill; // Tràn màn hình
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true; // Chỉ xem, không sửa

            // Lấy dữ liệu từ Manager
            var scores = HighScoreManager.LoadScores();

            // Gán dữ liệu vào bảng (C# tự động map properties Name, Score, Date vào cột)
            grid.DataSource = scores;

            this.Controls.Add(grid);
        }
    }
}