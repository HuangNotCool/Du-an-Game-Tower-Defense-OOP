using System;
using System.Drawing;
using TowerDefense.Core;
using TowerDefense.Entities.Enemies;
using TowerDefense.Core;
using TowerDefense.Entities.Enemies;

namespace TowerDefense.Entities
{
    public class Projectile : BaseEntity
    {
        private Enemy _target;
        private int _damage;
        private float _speed = 400f; // Tốc độ đạn bay (nhanh hơn quái)

        public Projectile(float x, float y, Enemy target, int damage)
        {
            this.X = x;
            this.Y = y;
            this.Width = 8;  // Đạn nhỏ
            this.Height = 8;
            _target = target;
            _damage = damage;
        }

        public override void Update(float deltaTime)
        {
            // 1. Nếu mục tiêu đã chết hoặc biến mất -> Hủy đạn
            if (_target == null || !_target.IsActive)
            {
                this.IsActive = false;
                return;
            }

            // 2. Tính toán hướng bay tới mục tiêu
            float dx = _target.X - X;
            float dy = _target.Y - Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            // 3. Nếu khoảng cách nhỏ (đã trúng)
            if (distance < 10f) // 10px là vùng va chạm
            {
                _target.TakeDamage(_damage); // Trừ máu quái
                this.IsActive = false;       // Đạn biến mất
                return;
            }

            // 4. Di chuyển đạn
            float moveX = (dx / distance) * _speed * deltaTime;
            float moveY = (dy / distance) * _speed * deltaTime;

            X += moveX;
            Y += moveY;
        }

        public override void Render(Graphics g)
        {
            // Vẽ viên đạn tròn màu đen
            g.FillEllipse(Brushes.Black, X - Width / 2, Y - Height / 2, Width, Height);
        }
    }
}