using System;
using System.Drawing;
using System.Windows.Forms;
using TowerDefense.Data;

namespace TowerDefense.Forms
{
    public partial class ShopForm : Form
    {
        private Label _lblDiamonds;
        private Label _lblArcherDmg;
        private Button _btnUpgradeArcherDmg;

        public ShopForm()
        {
            // Gọi hàm của Visual Studio (trong Designer.cs)
            InitializeComponent();

            // Gọi hàm giao diện thủ công của chúng ta (Đã đổi tên)
            SetupShopUI();

            // Cập nhật dữ liệu
            UpdateUI();
        }

        // --- ĐỔI TÊN HÀM NÀY TỪ InitializeComponent THÀNH SetupShopUI ---
        private void SetupShopUI()
        {
            this.Text = "UPGRADE SHOP";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 40, 60);

            // Tiêu đề
            Label title = new Label { Text = "BLACKSMITH UPGRADES", Font = new Font("Arial", 16, FontStyle.Bold), ForeColor = Color.Gold, AutoSize = true, Location = new Point(120, 20) };
            this.Controls.Add(title);

            // Hiển thị Kim Cương
            _lblDiamonds = new Label { Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.Cyan, AutoSize = true, Location = new Point(20, 60) };
            this.Controls.Add(_lblDiamonds);

            // Item 1: Archer Damage
            CreateUpgradeItem(1, "Archer Damage", 100, (s, e) => BuyUpgrade("ArcherDmg"));

            // Nút Quay lại
            Button btnBack = new Button { Text = "BACK TO MENU", Size = new Size(120, 40), Location = new Point(180, 300), BackColor = Color.Gray, ForeColor = Color.White };
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);
        }

        private void CreateUpgradeItem(int index, string name, int y, EventHandler onClick)
        {
            int top = 100 + (index - 1) * 60;

            Label lblName = new Label { Text = name, ForeColor = Color.White, Font = new Font("Arial", 10), Location = new Point(30, top), AutoSize = true };
            this.Controls.Add(lblName);

            _lblArcherDmg = new Label { Text = "Lv 0", ForeColor = Color.Yellow, Location = new Point(150, top), AutoSize = true };
            this.Controls.Add(_lblArcherDmg);

            _btnUpgradeArcherDmg = new Button { Text = "Upgrade", Location = new Point(250, top - 5), Size = new Size(150, 30), BackColor = Color.Green, ForeColor = Color.White };
            _btnUpgradeArcherDmg.Click += onClick;
            this.Controls.Add(_btnUpgradeArcherDmg);
        }

        private void UpdateUI()
        {
            var profile = UserProfile.Instance;
            _lblDiamonds.Text = $"💎 Diamonds: {profile.Diamonds}";

            _lblArcherDmg.Text = $"Lv {profile.ArcherDamageLevel}";
            int cost = GetUpgradeCost(profile.ArcherDamageLevel);
            _btnUpgradeArcherDmg.Text = $"UPGRADE ({cost} 💎)";

            _btnUpgradeArcherDmg.Enabled = profile.Diamonds >= cost;
            _btnUpgradeArcherDmg.BackColor = profile.Diamonds >= cost ? Color.Green : Color.Gray;
        }

        private int GetUpgradeCost(int currentLevel)
        {
            return 50 + (currentLevel * 25);
        }

        private void BuyUpgrade(string type)
        {
            var profile = UserProfile.Instance;
            if (type == "ArcherDmg")
            {
                int cost = GetUpgradeCost(profile.ArcherDamageLevel);
                if (profile.Diamonds >= cost)
                {
                    profile.Diamonds -= cost;
                    profile.ArcherDamageLevel++;
                    UserProfile.Save();
                    MessageBox.Show("Nâng cấp thành công!");
                    UpdateUI();
                }
            }
        }
    }
}