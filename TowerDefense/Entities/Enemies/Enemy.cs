using System.Collections.Generic;
using System.Drawing;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;

namespace TowerDefense.Entities.Enemies
{
    public class Enemy : BaseEntity
    {
        public int Health { get; set; } = 100;
        public int RewardGold { get; set; } = 10;
        private float _slowTimer = 0;

        // Sử dụng Component di chuyển
        private MovementComponent _movement;

        public Enemy(List<Point> path)
        {
            // Khởi tạo Component: Tốc độ 100 pixel/giây
            _movement = new MovementComponent(path, 100f);

            // Đặt vị trí ban đầu là điểm đầu tiên của đường đi
            if (path != null && path.Count > 0)
            {
                X = path[0].X;
                Y = path[0].Y;
            }
        }

        public override void Update(float deltaTime)
        {
            // Ủy quyền việc tính toán di chuyển cho Component
            PointF newPos = _movement.Update(new PointF(X, Y), deltaTime);
            X = newPos.X;
            Y = newPos.Y;

            // Kiểm tra nếu về đích (Ví dụ: Trừ máu người chơi - logic này sẽ làm ở GameManager sau)
            if (_movement.HasReachedEnd)
            {
                this.IsActive = false; // Tạm thời xóa quái
            }
        }

        public override void Render(Graphics g)
        {
            Image sprite = ResourceManager.GetImage("Enemy");

            if (sprite != null)
            {
                g.DrawImage(sprite, X - Width / 2, Y - Height / 2, Width, Height);
            }
            else
            {
                g.FillEllipse(Brushes.Red, X - Width / 2, Y - Height / 2, Width, Height);
            }

            // Vẽ thanh máu luôn nằm trên đầu quái
            float hpPercent = (float)Health / 100f; // Giả sử MaxHP = 100
            if (hpPercent < 0) hpPercent = 0;

            // Khung đỏ
            g.FillRectangle(Brushes.Red, X - 16, Y - 25, 32, 5);
            // Máu xanh
            g.FillRectangle(Brushes.LightGreen, X - 16, Y - 25, 32 * hpPercent, 5);
            // Viền đen
            g.DrawRectangle(Pens.Black, X - 16, Y - 25, 32, 5);
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0) IsActive = false;
        }

        // Hàm gọi từ bên ngoài để làm chậm quái
        public void ApplySlow(float duration, float slowFactor) // slowFactor = 0.5f nghĩa là giảm 50%
        {
            _slowTimer = duration;
            _movement.SetSpeedScale(slowFactor);
        }
    }
}