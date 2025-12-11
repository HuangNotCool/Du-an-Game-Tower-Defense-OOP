using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;
using TowerDefense.Configs;
using TowerDefense.Entities.Enemies;

namespace TowerDefense.Entities.Towers
{
    public class Tower : BaseEntity
    {
        // =========================================================
        // 1. TỐI ƯU HÓA GDI+ (TRÁNH NEW LIÊN TỤC GÂY LAG)
        // =========================================================
        private static readonly Font _lvFont = new Font("Arial", 7, FontStyle.Bold);
        private static readonly SolidBrush _hpBackBrush = new SolidBrush(Color.Red);
        private static readonly SolidBrush _hpForeBrush = new SolidBrush(Color.Lime);
        private static readonly Pen _borderPen = new Pen(Color.Black, 1);

        // =========================================================
        // 2. BIẾN & THUỘC TÍNH
        // =========================================================

        // Components
        protected CombatComponent _combat;
        protected float _rotation = 0f;

        // Thông tin cơ bản
        public string Name { get; set; }
        public int Price { get; set; }
        public Color Color { get; set; }
        public string ProjectileType { get; set; }

        // Chỉ số chiến đấu
        public int BaseDamage { get; protected set; }
        public int Level { get; private set; } = 1;

        // Public Range để UI vẽ vòng tròn tầm bắn
        public float Range => _combat.Range;

        // Chỉ số sinh tồn
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        // Kinh tế
        public int UpgradeCost => Price; // Giá nâng cấp bằng giá mua
        public int SellValue => (Price + (Level - 1) * UpgradeCost) / 2;

        // =========================================================
        // 3. CONSTRUCTORS
        // =========================================================

        // Constructor chính: Dùng ID để lấy thông tin từ Config
        public Tower(float x, float y, int towerTypeId)
        {
            this.X = x;
            this.Y = y;
            this.Width = 40;  // Kích thước hiển thị chuẩn
            this.Height = 40;

            // Lấy thông tin từ GameConfig
            if (towerTypeId < 0 || towerTypeId >= GameConfig.Towers.Length) towerTypeId = 0;
            var stat = GameConfig.Towers[towerTypeId];

            // Gán chỉ số
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

        // =========================================================
        // 4. GAME LOGIC (UPDATE)
        // =========================================================
        public override void Update(float deltaTime)
        {
            // 1. Hồi chiêu
            _combat.Update(deltaTime);

            // 2. Tìm mục tiêu
            var target = _combat.FindTarget(GameManager.Instance.Enemies, new PointF(X, Y));

            if (target != null)
            {
                // 3. Tính góc xoay (Hỗ trợ ảnh hướng sang phải - 0 độ)
                float dx = target.X - X;
                float dy = target.Y - Y;
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
            // Tạo đạn
            var bullet = new Projectile(this.X, this.Y, target, BaseDamage, ProjectileType);
            GameManager.Instance.Projectiles.Add(bullet);

            // Âm thanh
            if (ProjectileType == "Bomb" || ProjectileType == "Rocket")
                SoundManager.Play("explosion"); // Tiếng nổ cho súng to
            else
                SoundManager.Play("shoot");     // Tiếng pew pew thường
        }

        // =========================================================
        // 5. TƯƠNG TÁC (NÂNG CẤP & NHẬN DAM)
        // =========================================================

        public void Upgrade()
        {
            if (Level >= 3) return;

            Level++;

            // Tăng chỉ số
            BaseDamage = (int)(BaseDamage * 1.5f); // +50% Damage
            Health = MaxHealth; // Hồi máu

            // Tăng tầm bắn (Tạo lại component với range mới)
            float newRange = _combat.Range * 1.2f; // +20% Range
            // Giữ nguyên reload time cũ (hoặc giảm nếu muốn)
            // Lưu ý: Cần truy cập reloadTime cũ, ở đây ta giả định giữ nguyên reload gốc hoặc lấy từ config nếu lưu lại ID
            _combat = new CombatComponent(newRange, 0.8f);

            // Hiệu ứng
            GameManager.Instance.ShowFloatingText("LEVEL UP!", X, Y - 30, Color.Cyan);
            GameManager.Instance.CreateHitEffect(X, Y); // Bụi bay
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            GameManager.Instance.ShowFloatingText($"-{damage}", X, Y - 10, Color.Orange);

            if (Health <= 0)
            {
                this.IsActive = false; // Nổ tung
                SoundManager.Play("explosion");
                GameManager.Instance.ShowFloatingText("DESTROYED!", X, Y, Color.Red);
                GameManager.Instance.CreateExplosion(X, Y, Color.Gray); // Hiệu ứng khói xám
            }
        }

        // =========================================================
        // 6. RENDER (VẼ)
        // =========================================================
        public override void Render(Graphics g)
        {
            // A. Tự động chọn ảnh theo Level (Evolution)
            // Ví dụ: Archer_2.png, Cannon_3.png
            // Nếu không có ảnh Lv cao thì dùng ảnh gốc
            string imgKey = (Level > 1) ? $"{Name}_{Level}" : Name;
            Image sprite = ResourceManager.GetImage(imgKey);
            if (sprite == null) sprite = ResourceManager.GetImage(Name); // Fallback về ảnh gốc

            if (sprite != null)
            {
                // Kỹ thuật xoay ảnh
                var state = g.Save();
                g.TranslateTransform(X, Y);
                g.RotateTransform(_rotation);
                g.DrawImage(sprite, -Width / 2, -Height / 2, Width, Height);
                g.Restore(state);
            }
            else
            {
                // Vẽ hình vuông nếu thiếu ảnh
                using (SolidBrush b = new SolidBrush(this.Color))
                {
                    g.FillRectangle(b, X - 16, Y - 16, 32, 32);
                    g.DrawRectangle(_borderPen, X - 16, Y - 16, 32, 32);
                }
            }

            // B. Vẽ thông tin Level
            g.DrawString($"Lv{Level}", _lvFont, Brushes.White, X - 10, Y - 25);

            // Vẽ sao cấp độ
            if (Level >= 2) g.FillEllipse(Brushes.Gold, X - 10, Y + 15, 6, 6);
            if (Level >= 3) g.FillEllipse(Brushes.Gold, X + 4, Y + 15, 6, 6);

            // C. Vẽ Thanh Máu (Chỉ hiện khi mất máu)
            if (Health < MaxHealth)
            {
                float hpPercent = (float)Health / MaxHealth;
                if (hpPercent < 0) hpPercent = 0;

                // Dùng Brush static để tối ưu
                g.FillRectangle(_hpBackBrush, X - 16, Y + 22, 32, 4);
                g.FillRectangle(_hpForeBrush, X - 16, Y + 22, 32 * hpPercent, 4);
                g.DrawRectangle(_borderPen, X - 16, Y + 22, 32, 4);
            }
        }
    }
}