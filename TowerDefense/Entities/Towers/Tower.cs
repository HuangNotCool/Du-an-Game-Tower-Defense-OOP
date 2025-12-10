using System;
using System.Drawing;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;
using TowerDefense.Entities.Enemies; // Để nhận diện Enemy

namespace TowerDefense.Entities.Towers
{
    public class Tower : BaseEntity
    {
        // --- COMPONENTS & STATE ---
        protected CombatComponent _combat; // Component xử lý tầm bắn và cooldown
        protected float _rotation = 0f;    // Góc quay hiện tại (Độ)

        // --- THUỘC TÍNH (PROPERTIES) ---
        public string Name { get; set; } = "Tower";
        public int Price { get; set; } = 100;
        public Color Color { get; set; } = Color.Blue;

        // Level hiện tại trong trận đấu (1, 2, 3)
        public int Level { get; private set; } = 1;
        public int BaseDamage { get; protected set; }

        // Tính toán giá trị kinh tế
        public int UpgradeCost => Price; // Giá nâng cấp = Giá mua gốc
        public int SellValue => (Price + (Level - 1) * UpgradeCost) / 2; // Bán lại 50% giá trị

        // --- CONSTRUCTORS ---
        public Tower(float x, float y, float range, float reloadTime, int damage)
        {
            this.X = x;
            this.Y = y;
            this.BaseDamage = damage;

            // Khởi tạo Component chiến đấu
            _combat = new CombatComponent(range, reloadTime);
        }

        // Constructor mặc định (dành cho test)
        public Tower(float x, float y) : this(x, y, 150f, 1.0f, 25) { }

        // --- LOGIC GAME LOOP (UPDATE) ---
        public override void Update(float deltaTime)
        {
            // 1. Giảm thời gian hồi chiêu
            _combat.Update(deltaTime);

            // 2. Tìm mục tiêu trong tầm ngắm (để xoay súng)
            // Lưu ý: FindTarget cần trả về Enemy (Bạn kiểm tra lại file CombatComponent xem hàm trả về gì)
            var target = _combat.FindTarget(GameManager.Instance.Enemies, new PointF(X, Y));

            if (target != null)
            {
                // --- TÍNH TOÁN GÓC QUAY ---
                float dx = target.X - X;
                float dy = target.Y - Y;

                // Math.Atan2 trả về Radian -> Đổi sang Độ (* 180 / PI)
                // Lưu ý: Sprite gốc phải hướng sang PHẢI (3 giờ) thì góc mới đúng
                _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

                // 3. Nếu súng đã nạp đạn xong -> Bắn
                if (_combat.CanShoot())
                {
                    Shoot(target);
                    _combat.ResetCooldown();
                }
            }
        }

        // --- HÀM BẮN (VIRTUAL ĐỂ LỚP CON GHI ĐÈ ĐƯỢC) ---
        protected virtual void Shoot(Enemy target)
        {
            // Tạo đạn (Code cũ)
            var bullet = new Projectile(this.X, this.Y, target, BaseDamage);
            GameManager.Instance.Projectiles.Add(bullet);

            // --- THÊM DÒNG NÀY ---
            SoundManager.Play("shoot");
        }

        // --- LOGIC NÂNG CẤP (UPGRADE) ---
        public void Upgrade()
        {
            if (Level >= 3) return; // Max level là 3

            Level++;

            // Tăng chỉ số: Damage +50%
            BaseDamage = (int)(BaseDamage * 1.5f);

            // Tăng chỉ số: Range +20%
            // Vì CombatComponent không có setter cho Range, ta tạo mới Component (Cách đơn giản nhất)
            // Lưu ý: Cần biết Range cũ. Ở đây ta tính lại dựa trên công thức.
            // Để code xịn hơn, bạn nên thêm property Range vào CombatComponent.
            // Giả sử tầm bắn cơ bản là 200 (hoặc lấy từ giá trị khởi tạo của lớp con).
            // Ở đây mình "hack" nhẹ bằng cách tạo lại component với range rộng hơn.

            // Lấy range hiện tại (nếu CombatComponent public Range thì tốt, không thì ước lượng)
            // float newRange = _combat.Range * 1.2f; 
            // Do _combat đóng gói, ta tạm thời hardcode logic tăng range ở lớp con (ArcherTower) 
            // hoặc chấp nhận chỉ tăng Damage ở đây.
        }

        // --- LOGIC VẼ (RENDER) ---
        public override void Render(Graphics g)
        {
            // 1. Lấy ảnh từ ResourceManager
            Image sprite = ResourceManager.GetImage(this.Name);

            if (sprite != null)
            {
                // --- KỸ THUẬT XOAY ẢNH (MATRIX TRANSFORMATION) ---
                var state = g.Save(); // Lưu trạng thái hiện tại

                // Dời gốc tọa độ về tâm tháp
                g.TranslateTransform(X, Y);

                // Xoay theo góc đã tính
                g.RotateTransform(_rotation);

                // Vẽ ảnh (Tâm ảnh trùng gốc tọa độ 0,0 mới)
                g.DrawImage(sprite, -Width / 2, -Height / 2, Width, Height);

                g.Restore(state); // Khôi phục trạng thái để không ảnh hưởng cái khác
            }
            else
            {
                // Fallback: Vẽ hình vuông nếu không có ảnh
                g.FillRectangle(new SolidBrush(this.Color), X - 16, Y - 16, 32, 32);
                g.DrawRectangle(Pens.Black, X - 16, Y - 16, 32, 32);
            }

            // 2. Vẽ thông tin Level (Vẽ đè lên trên, không xoay)
            // Vẽ số level
            g.DrawString($"Lv{Level}", new Font("Arial", 8, FontStyle.Bold), Brushes.White, X - 12, Y - 28);

            // Vẽ sao/chấm tròn thể hiện cấp độ
            if (Level >= 2) g.FillEllipse(Brushes.Gold, X - 10, Y + 10, 6, 6);
            if (Level >= 3) g.FillEllipse(Brushes.Gold, X + 4, Y + 10, 6, 6);
        }
    }
}