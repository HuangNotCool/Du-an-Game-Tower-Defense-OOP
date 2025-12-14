using System;
using System.Drawing;
using TowerDefense.Configs;
using TowerDefense.Core;
using TowerDefense.Entities.Enemies;
using TowerDefense.Managers;

namespace TowerDefense.Entities.Towers
{
    public class Tower : BaseEntity
    {
        // Thông số cơ bản
        public int TypeId { get; private set; }
        public string Name { get; private set; }
        public float Range { get; private set; }
        public float Cooldown { get; private set; }
        public int BaseDamage { get; private set; }
        public int Price { get; private set; }
        public int UpgradeCost { get; private set; }
        public int SellValue { get; private set; }
        public Color Color { get; private set; }

        // Level
        public int Level { get; private set; } = 1;

        // Trạng thái chiến đấu
        private float _fireTimer = 0f;
        public Enemy Target { get; private set; }

        public Tower(int x, int y, int typeId)
        {
            this.X = x;
            this.Y = y;
            this.Width = 500;
            this.Height = 500;
            this.TypeId = typeId;

            // Load chỉ số từ Config
            var stat = GameConfig.Towers[typeId];
            this.Name = stat.Name;
            this.Range = stat.Range;
            this.Cooldown = stat.Cooldown;
            this.BaseDamage = stat.Damage;
            this.Price = stat.Price;
            this.Color = stat.Color;

            this.UpgradeCost = (int)(Price * 0.7f);
            this.SellValue = (int)(Price * 0.5f);
        }

        public override void Update(float deltaTime)
        {
            _fireTimer -= deltaTime;

            // 1. Tìm mục tiêu nếu chưa có hoặc mục tiêu ra khỏi tầm/chết
            if (Target == null || !Target.IsActive || GetDistance(Target) > Range)
            {
                Target = FindTarget();
            }

            // 2. Tấn công
            if (Target != null && _fireTimer <= 0)
            {
                Attack();
                _fireTimer = Cooldown;
            }
        }

        private void Attack()
        {
            // Xác định loại đạn dựa trên tên tháp
            ProjectileType pType = ProjectileType.Bullet;
            float pSpeed = 400f;
            float aoe = 0f;

            if (Name == "Sniper") { pSpeed = 800f; }
            else if (Name == "Cannon")
            {
                pType = ProjectileType.Missile;
                pSpeed = 300f;
                aoe = 60f; // Bán kính nổ
            }
            else if (Name == "Ice") { pType = ProjectileType.Ice; pSpeed = 350f; }
            else if (Name == "Minigun") { pType = ProjectileType.Bullet; pSpeed = 500f; }
            else if (Name == "God")
            {
                pType = ProjectileType.Missile;
                aoe = 100f; // Nổ to

                // --- FIX LỖI TẠI ĐÂY: Tháp God tạo vụ nổ ngay lập tức ---
                // Truyền bán kính 100f thay vì Color
                GameManager.Instance.CreateExplosion(Target.X, Target.Y, 100f);
            }

            // Tạo đạn bay
            var proj = new Projectile(X, Y, Target, BaseDamage, pSpeed, pType, aoe);

            if (GameManager.Instance.Projectiles != null)
                GameManager.Instance.Projectiles.Add(proj);

            // Âm thanh
            // SoundManager.Play("shoot");
        }

        private Enemy FindTarget()
        {
            Enemy best = null;
            float minDst = float.MaxValue;

            // Ưu tiên bắn con gần đích nhất (đã đi được quãng đường dài nhất)
            // Hoặc đơn giản là con gần tháp nhất
            foreach (var e in GameManager.Instance.Enemies)
            {
                float dst = GetDistance(e);
                if (dst <= Range)
                {
                    // Logic chọn mục tiêu: Gần nhất
                    if (dst < minDst)
                    {
                        minDst = dst;
                        best = e;
                    }
                }
            }
            return best;
        }

        private float GetDistance(BaseEntity e)
        {
            float dx = X - e.X;
            float dy = Y - e.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public void Upgrade()
        {
            Level++;
            BaseDamage = (int)(BaseDamage * 1.5f);
            Range += 20;
            Cooldown *= 0.9f; // Bắn nhanh hơn 10%

            UpgradeCost = (int)(UpgradeCost * 1.5f);
            SellValue += (int)(UpgradeCost * 0.4f);
        }

        public override void Render(Graphics g)
        {
            // Vẽ chân đế
            g.FillRectangle(Brushes.Gray, X - 10, Y - 10, 20, 20);

            // Vẽ tháp
            Image img = ResourceManager.GetImage(Name);
            if (img != null)
            {
                g.DrawImage(img, X - 30, Y - 60, 60, 60);
            }
            else
            {
                using (SolidBrush b = new SolidBrush(Color))
                    g.FillRectangle(b, X - 8, Y - 24, 16, 24);
            }

            // Vẽ cấp độ
            if (Level > 1)
            {
                using (Font f = new Font("Arial", 8, FontStyle.Bold))
                {
                    g.DrawString($"v{Level}", f, Brushes.Yellow, X - 10, Y - 5);
                }
            }
        }
    }
}