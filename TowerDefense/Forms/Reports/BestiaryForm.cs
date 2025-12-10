using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Managers; // Để lấy ảnh từ ResourceManager

namespace TowerDefense.Forms.Reports
{
    // Class chứa dữ liệu để hiển thị
    public class EnemyInfo
    {
        public string Name { get; set; }
        public string ImageKey { get; set; }
        public int HP { get; set; }
        public string Speed { get; set; }
        public string Description { get; set; }
    }

    public partial class BestiaryForm : Form
    {
        private ListBox _listBox;
        private PictureBox _pictureBox;
        private Label _lblName;
        private Label _lblStats;
        private Label _lblDesc;
        private List<EnemyInfo> _data;

        public BestiaryForm()
        {
            InitializeComponent();
            LoadData();

            // Chọn mặc định con đầu tiên
            if (_listBox.Items.Count > 0) _listBox.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.Text = "MONSTER BESTIARY (TỪ ĐIỂN QUÁI)";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;

            // 1. ListBox bên trái
            _listBox = new ListBox();
            _listBox.Location = new Point(10, 10);
            _listBox.Size = new Size(150, 380);
            _listBox.SelectedIndexChanged += OnSelectionChanged;
            this.Controls.Add(_listBox);

            // 2. PictureBox bên phải
            _pictureBox = new PictureBox();
            _pictureBox.Location = new Point(180, 10);
            _pictureBox.Size = new Size(128, 128); // Ảnh to gấp đôi
            _pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            _pictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(_pictureBox);

            // 3. Label Tên
            _lblName = new Label();
            _lblName.Location = new Point(320, 10);
            _lblName.AutoSize = true;
            _lblName.Font = new Font("Arial", 16, FontStyle.Bold);
            _lblName.ForeColor = Color.DarkRed;
            this.Controls.Add(_lblName);

            // 4. Label Chỉ số
            _lblStats = new Label();
            _lblStats.Location = new Point(320, 50);
            _lblStats.AutoSize = true;
            _lblStats.Font = new Font("Arial", 12, FontStyle.Regular);
            this.Controls.Add(_lblStats);

            // 5. Label Mô tả (Story)
            _lblDesc = new Label();
            _lblDesc.Location = new Point(180, 150);
            _lblDesc.Size = new Size(380, 200); // Khung text rộng
            _lblDesc.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            _lblDesc.BorderStyle = BorderStyle.Fixed3D;
            _lblDesc.Padding = new Padding(5);
            this.Controls.Add(_lblDesc);
        }

        private void LoadData()
        {
            // Tạo dữ liệu giả (Mock Data) - Sau này có thể load từ JSON Config
            _data = new List<EnemyInfo>
            {
                new EnemyInfo { Name = "Slime", ImageKey = "Enemy", HP = 100, Speed = "Slow", Description = "Quái vật nhầy nhụa cơ bản. Di chuyển chậm chạp nhưng xuất hiện rất đông. Dùng để farm vàng đầu game." },
                new EnemyInfo { Name = "Goblin", ImageKey = "Enemy", HP = 250, Speed = "Medium", Description = "Lính bộ binh có trang bị nhẹ. Tốc độ trung bình. Nguy hiểm khi đi theo bầy." },
                new EnemyInfo { Name = "Wolf", ImageKey = "Enemy", HP = 150, Speed = "Fast", Description = "Sói tinh nhuệ. Tốc độ rất nhanh, dễ dàng lướt qua hàng phòng thủ nếu không có tháp làm chậm." },
                new EnemyInfo { Name = "Golem", ImageKey = "Enemy", HP = 1000, Speed = "Very Slow", Description = "Người đá khổng lồ. Máu cực trâu, đóng vai trò đỡ đạn cho các đơn vị khác." },
                new EnemyInfo { Name = "DRAGON BOSS", ImageKey = "Enemy", HP = 5000, Speed = "Fast", Description = "TRÙM CUỐI. Sở hữu sức mạnh hủy diệt. Kháng hiệu ứng làm chậm. Tiêu diệt để chiến thắng game!" }
            };

            // Đổ dữ liệu vào ListBox
            foreach (var item in _data)
            {
                _listBox.Items.Add(item.Name);
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (_listBox.SelectedIndex < 0) return;

            var info = _data[_listBox.SelectedIndex];

            // Cập nhật giao diện
            _lblName.Text = info.Name.ToUpper();
            _lblStats.Text = $"HP: {info.HP}\nSpeed: {info.Speed}";
            _lblDesc.Text = info.Description;

            // Lấy ảnh từ ResourceManager (Dùng key hoặc ảnh mặc định)
            // Lưu ý: Bạn cần chắc chắn ResourceManager.GetImage đã có
            Image img = ResourceManager.GetImage(info.ImageKey);
            if (img != null) _pictureBox.Image = img;
        }
    }
}