using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TowerDefense.Managers;
using TowerDefense.Utils;

namespace TowerDefense.Forms.Reports
{
    public partial class HistoryForm : Form
    {
        private DataGridView _grid;

        public HistoryForm()
        {
            // 1. Gọi hàm khởi tạo (Hàm này được định nghĩa ở cuối file để tránh lỗi)
            InitializeComponent();

            // 2. Thiết lập giao diện Custom
            SetupUI();

            // 3. Tải dữ liệu
            LoadData();
        }

        private void SetupUI()
        {
            // Cấu hình Form
            this.Text = "BATTLE LOGS";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Nền Gradient
            this.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                       Color.FromArgb(40, 40, 60), Color.FromArgb(10, 10, 20), 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "📜 MATCH HISTORY 📜",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(150, 20),
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblTitle);

            // Custom DataGridView
            _grid = new DataGridView();
            _grid.Location = new Point(25, 70);
            _grid.Size = new Size(535, 330);
            _grid.BackgroundColor = Color.FromArgb(30, 30, 40);
            _grid.BorderStyle = BorderStyle.None;
            _grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _grid.GridColor = Color.DimGray;

            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.AllowUserToResizeRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.ScrollBars = ScrollBars.Vertical;

            // Style Header
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 20, 30);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 40;

            // Style Rows
            _grid.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 50);
            _grid.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 60, 80);
            _grid.DefaultCellStyle.SelectionForeColor = Color.White;
            _grid.RowTemplate.Height = 35;

            // Định nghĩa cột
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DATE", Width = 130, DataPropertyName = "Date" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "MAP", Width = 100, DataPropertyName = "MapName" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "RESULT", Width = 120, DataPropertyName = "Result" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "WAVE REACHED", Width = 150, DataPropertyName = "WaveReached" });

            // Căn chỉnh
            _grid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Sự kiện tô màu
            _grid.CellFormatting += Grid_CellFormatting;

            this.Controls.Add(_grid);

            // Nút Đóng
            GameButton btnClose = new GameButton
            {
                Text = "CLOSE",
                Size = new Size(150, 45),
                Location = new Point(225, 410),
                Color1 = Color.DarkSlateGray,
                Color2 = Color.Black,
                BorderRadius = 20
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadData()
        {
            var logs = HistoryManager.LoadLogs();
            logs.Reverse();
            _grid.DataSource = logs;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && _grid.Columns[e.ColumnIndex].HeaderText == "RESULT")
            {
                string value = e.Value?.ToString();

                if (value == "VICTORY")
                {
                    e.CellStyle.ForeColor = Color.LimeGreen;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
                else if (value == "DEFEAT")
                {
                    e.CellStyle.ForeColor = Color.OrangeRed;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            }
        }

        // --- HÀM GIẢ ĐỂ TRÁNH LỖI BIÊN DỊCH ---
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }
        private System.ComponentModel.IContainer components = null;
    }
}