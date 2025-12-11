using System;
using System.Drawing;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;
using TowerDefense.Configs;         // Để lấy thông số tháp
using TowerDefense.Entities.Enemies;

namespace TowerDefense.Entities.Towers
{
    public class Tower : BaseEntity
    {
        // --- COMPONENTS & STATE ---
        protected CombatComponent _combat;
        protected float _rotation = 0f;

        // --- THUỘC TÍNH CƠ BẢN ---
        public string Name { get; set; }
        public int Price { get; set; }
        public Color Color { get; set; }
        public string ProjectileType { get; set; } // Loại đạn: "Arrow", "Bomb"...

        // --- CHỈ SỐ CHIẾN ĐẤU ---
        public int BaseDamage { get; protected set; }
        public int Level { get; private set; } = 1;

        // --- HỆ THỐNG MÁU (DESTRUCTIBLE TOWER) ---
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        // --- KINH TẾ ---
        public int UpgradeCost => Price; // Giá nâng cấp = Giá mua
        public int SellValue => (Price + (Level - 1) * UpgradeCost) / 2;

        // =========================================================
        // CONSTRUCTORS
        // =========================================================

        // 1. Constructor khởi tạo từ ID (Dùng cho GameConfig)
        public Tower(float x, float y, int towerTypeId)
        {
            this.X = x;
            this.Y = y;
            this.Width = 32;
            this.Height = 32;

            // Lấy thông số từ Config (Data-Driven)
            // Đảm bảo towerTypeId nằm trong phạm vi mảng Towers
            if (towerTypeId < 0 || towerTypeId >= GameConfig.Towers.Length) towerTypeId = 0;

            var stat = GameConfig.Towers[towerTypeId];

            this.Name = stat.Name;
            this.Price = stat.Price;
            this.Color = stat.Color;
            this.BaseDamage = stat.Damage;
            this.MaxHealth = stat.MaxHealth;
            this.Health = stat.MaxHealth;
            this.ProjectileType = stat.ProjectileType;

            // Khởi tạo Component chiến đấu
            _combat = new CombatComponent(stat.Range, stat.ReloadTime);
        }

        // 2. Constructor thủ công (Để test hoặc tạo tháp custom không có trong config)
        public Tower(float x, float y, float range, float reloadTime, int damage, int hp)
        {
            this.X = x; this.Y = y;
            this.BaseDamage = damage;
            this.MaxHealth = hp;
            this.Health = hp;
            this.Name = "CustomTower";
            this.Color = Color.Blue;
            this.ProjectileType = "Arrow";
            _combat = new CombatComponent(range, reloadTime);
        }

        // =========================================================
        // GAME LOGIC (UPDATE)
        // =========================================================
        public override void Update(float deltaTime)
        {
            // 1. Hồi chiêu
            _combat.Update(deltaTime);

            // 2. Tìm mục tiêu
            var target = _combat.FindTarget(GameManager.Instance.Enemies, new PointF(X, Y));

            if (target != null)
            {
                // 3. Tính góc xoay (Rotation)
                float dx = target.X - X;
                float dy = target.Y - Y;

                // Chuyển đổi Radian sang Độ. 
                // Lưu ý: Sprite gốc nên hướng về bên phải (0 độ).
                _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

                // 4. Bắn
                if (_combat.CanShoot())
                {
                    Shoot(target);
                    _combat.ResetCooldown();
                }
            }
        }

        protected virtual void Shoot(Enemy target)
        {
            // Tạo đạn với loại đạn (ProjectileType) lấy từ Config
            var bullet = new Projectile(this.X, this.Y, target, BaseDamage, ProjectileType);

            GameManager.Instance.Projectiles.Add(bullet);

            // Âm thanh
            if (ProjectileType == "Bomb") SoundManager.Play("explosion"); // Tiếng to hơn cho Cannon
            else SoundManager.Play("shoot");
        }

        // =========================================================
        // TƯƠNG TÁC (NÂNG CẤP & NHẬN SÁT THƯƠNG)
        // =========================================================

        public void Upgrade()
        {
            if (Level >= 3) return;

            Level++;

            // Tăng chỉ số
            BaseDamage = (int)(BaseDamage * 1.5f); // +50% Damage
            Health = MaxHealth; // Hồi đầy máu khi nâng cấp

            // Tăng tầm bắn (Tạo lại component với range mới)
            // Lấy range hiện tại (từ property Range trong CombatComponent) * 1.2
            float newRange = _combat.Range * 1.2f;

            // Giữ nguyên tốc độ bắn (hoặc có thể giảm cooldown nếu muốn)
            // Lưu ý: ReloadTime không lưu trong combat component public, nên ta lấy tạm giá trị mặc định hoặc logic riêng
            // Ở đây mình "hack" bằng cách giữ nguyên component cũ nhưng logic range trong FindTarget cần cập nhật.
            // Cách tốt nhất: Re-new component.
            _combat = new CombatComponent(newRange, 0.8f); // Tạm set reload 0.8f chung, sau này nên lưu ReloadTime vào biến class

            GameManager.Instance.ShowFloatingText("LEVEL UP!", X, Y - 30, Color.Cyan);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            // Hiệu ứng số damage bay lên
            GameManager.Instance.ShowFloatingText($"-{damage}", X, Y - 10, Color.Orange);

            if (Health <= 0)
            {
                this.IsActive = false; // Tháp bị phá hủy
                SoundManager.Play("explosion"); // Tiếng nổ
                GameManager.Instance.ShowFloatingText("DESTROYED!", X, Y, Color.Red);
            }
        }

        // =========================================================
        // RENDER (VẼ)
        // =========================================================
        public override void Render(Graphics g)
        {
            // 1. Vẽ Sprite Tháp (Xoay)
            Image sprite = ResourceManager.GetImage(this.Name);

            if (sprite != null)
            {
                var state = g.Save();
                g.TranslateTransform(X, Y);
                g.RotateTransform(_rotation);
                g.DrawImage(sprite, -Width / 2, -Height / 2, Width, Height);
                g.Restore(state);
            }
            else
            {
                // Fallback: Vẽ hình vuông màu
                g.FillRectangle(new SolidBrush(this.Color), X - 16, Y - 16, 32, 32);
                g.DrawRectangle(Pens.Black, X - 16, Y - 16, 32, 32);
            }

            // 2. Vẽ Level (Lv1, Lv2...)
            g.DrawString($"Lv{Level}", new Font("Arial", 7, FontStyle.Bold), Brushes.White, X - 10, Y - 25);
            if (Level >= 2) g.FillEllipse(Brushes.Gold, X - 12, Y + 12, 5, 5);
            if (Level >= 3) g.FillEllipse(Brushes.Gold, X + 6, Y + 12, 5, 5);

            // 3. Vẽ Thanh Máu (Health Bar) - Chỉ vẽ khi mất máu
            if (Health < MaxHealth)
            {
                float hpPercent = (float)Health / MaxHealth;
                if (hpPercent < 0) hpPercent = 0;

                // Vẽ thanh máu ngay dưới tháp
                g.FillRectangle(Brushes.Red, X - 16, Y + 18, 32, 4);
                g.FillRectangle(Brushes.Lime, X - 16, Y + 18, 32 * hpPercent, 4);
                g.DrawRectangle(Pens.Black, X - 16, Y + 18, 32, 4);
            }
        }
    }
}