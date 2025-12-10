using System.Drawing;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;
using System;

namespace TowerDefense.Entities.Towers
{
    // Bỏ 'sealed' nếu có, để có thể kế thừa
    public class Tower : BaseEntity
    {
        // Đổi private thành protected để lớp con có thể chỉnh sửa
        protected CombatComponent _combat;

        public int Price { get; set; } = 100; // Giá tiền
        public string Name { get; set; } = "Tower";
        public Color Color { get; set; } = Color.Blue;

        // Constructor cho lớp cha nhận tham số cấu hình
        public Tower(float x, float y, float range, float reloadTime, int damage)
        {
            this.X = x;
            this.Y = y;
            _combat = new CombatComponent(range, reloadTime);
            // Lưu ý: Chúng ta cần cập nhật Damage cho đạn, 
            // nhưng tạm thời Projectile đang hardcode damage.
            // Ta sẽ sửa logic bắn sau để linh hoạt hơn.
        }

        // Constructor mặc định (nếu cần)
        public Tower(float x, float y) : this(x, y, 150f, 1.0f, 25) { }

        private float _currentRotation = 0f; // Góc quay (độ)

        public override void Update(float deltaTime)
        {
            _combat.Update(deltaTime);
            if (_combat.CanShoot())
            {
                // Lấy mục tiêu (Bạn cần sửa FindTarget trả về Enemy thay vì bắn luôn bên trong)
                var target = _combat.FindTarget(GameManager.Instance.Enemies, new PointF(X, Y));

                if (target != null)
                {
                    // --- TÍNH GÓC QUAY ---
                    float dx = target.X - X;
                    float dy = target.Y - Y;
                    // Công thức Atan2 trả về Radian, cần đổi sang Độ
                    _currentRotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

                    Shoot(target);
                    _combat.ResetCooldown();
                }
            }
        }

        // Cho phép lớp con ghi đè (override) hành động bắn nếu muốn (VD: Tháp băng bắn đạn băng)
        protected virtual void Shoot(Entities.Enemies.Enemy target)
        {
            // Mặc định bắn đạn thường
            var bullet = new Projectile(this.X, this.Y, target, 25);
            GameManager.Instance.Projectiles.Add(bullet);
        }

        public override void Render(Graphics g)
        {
            // Lấy ảnh từ ResourceManager dựa trên tên (Archer/Cannon)
            Image sprite = ResourceManager.GetImage(this.Name);

            if (sprite != null)
            {
                // --- LOGIC XOAY HÌNH ---
                // 1. Lưu trạng thái đồ họa hiện tại
                var state = g.Save();

                // 2. Dời gốc tọa độ về tâm của tháp
                g.TranslateTransform(X, Y);

                // 3. Tính góc quay (nếu đang có mục tiêu)
                // Lấy mục tiêu từ CombatComponent (bạn cần đổi _combat thành public hoặc có hàm GetTarget)
                // Tạm thời ta tính góc dựa trên mục tiêu gần nhất tìm thấy

                // (Mẹo: Để đơn giản, ta lưu góc quay vào một biến trong Tower.Update)
                g.RotateTransform(_currentRotation);

                // 4. Vẽ ảnh (Lưu ý: Vẽ tại -Width/2, -Height/2 vì gốc tọa độ đang ở tâm)
                g.DrawImage(sprite, -Width / 2, -Height / 2, Width, Height);

                // 5. Khôi phục trạng thái cũ (để không ảnh hưởng các vật thể khác)
                g.Restore(state);
            }
            else
            {
                // Fallback: Vẽ hình vuông nếu chưa có ảnh
                g.FillRectangle(new SolidBrush(this.Color), X - 16, Y - 16, 32, 32);
            }

            // Debug: Vẽ viền tầm bắn khi di chuột vào (Code này làm sau)
            // g.DrawEllipse(Pens.LightGray, X - 150, Y - 150, 300, 300);
        }
    }
}