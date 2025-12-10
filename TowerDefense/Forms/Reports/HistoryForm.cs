using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers;

namespace TowerDefense.Forms.Reports
{
    public partial class HistoryForm : Form
    {
        public HistoryForm()
        {
            // Cấu hình Form
            this.Text = "MATCH HISTORY";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;

            // Load dữ liệu
            var logs = HistoryManager.LoadLogs();

            // Đảo ngược để xem trận mới nhất trước
            logs.Reverse();

            grid.DataSource = logs;

            this.Controls.Add(grid);
        }
    }
}