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

        public int Level { get; private set; } = 1;

        // BIẾN LƯU TÊN ẢNH ĐẠN CỦA THÁP NÀY
        public string BulletImageKey { get; private set; }

        private float _fireTimer = 0f;
        public Enemy Target { get; private set; }

        public Tower(int x, int y, int typeId)
        {
            this.X = x;
            this.Y = y;
            this.Width = 60;
            this.Height = 60;
            this.TypeId = typeId;

            // Load chỉ số từ Config
            var stat = GameConfig.Towers[typeId];
            this.Name = stat.Name;
            this.Range = stat.Range;
            this.Cooldown = stat.ReloadTime;
            this.BaseDamage = stat.Damage;
            this.Price = stat.Price;
            this.Color = stat.Color;

            // ĐỌC TÊN ẢNH ĐẠN TỪ CONFIG
            this.BulletImageKey = stat.BulletImage;
            if (string.IsNullOrEmpty(this.BulletImageKey)) this.BulletImageKey = "Arrow";

            this.UpgradeCost = (int)(Price * 0.7f);
            this.SellValue = (int)(Price * 0.5f);
        }

        public override void Update(float deltaTime)
        {
            _fireTimer -= deltaTime;

            // 1. Tìm mục tiêu
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
            // Thiết lập loại đạn và hiệu ứng
            ProjectileType pType = ProjectileType.Bullet;
            float pSpeed = 400f;
            float aoe = 0f;

            // Cấu hình tốc độ và loại đạn (Logic hình ảnh đã được xử lý bởi BulletImageKey)
            if (Name == "Sniper") { pSpeed = 900f; }
            else if (Name == "Minigun") { pSpeed = 600f; }
            else if (Name == "Cannon") { pType = ProjectileType.Missile; pSpeed = 300f; aoe = 60f; }
            else if (Name == "Rocket") { pType = ProjectileType.Missile; pSpeed = 350f; aoe = 80f; }
            else if (Name == "Ice") { pType = ProjectileType.Ice; pSpeed = 350f; }
            else if (Name == "God")
            {
                // Tháp God tấn công ngay lập tức (không cần đạn bay)
                GameManager.Instance.CreateExplosion(Target.X, Target.Y, 100f);
                Target.TakeDamage(BaseDamage);
                return;
            }

            // TẠO ĐẠN: Truyền BulletImageKey vào để Projectile biết dùng ảnh nào
            var proj = new Projectile(X, Y, Target, BaseDamage, pSpeed, pType, aoe, this.BulletImageKey);

            if (GameManager.Instance.Projectiles != null)
                GameManager.Instance.Projectiles.Add(proj);
        }

        private Enemy FindTarget()
        {
            Enemy best = null;
            float minDst = float.MaxValue;

            foreach (var e in GameManager.Instance.Enemies)
            {
                float dst = GetDistance(e);
                if (dst <= Range)
                {
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
            Cooldown *= 0.9f;
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
                    g.FillRectangle(b, X - 15, Y - 30, 30, 50);
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