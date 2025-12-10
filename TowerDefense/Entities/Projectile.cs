using System;
using System.Drawing;
using TowerDefense.Core;
using TowerDefense.Entities.Enemies;

namespace TowerDefense.Entities
{
    public class Projectile : BaseEntity
    {
        private Enemy _target;
        private int _damage;
        private float _speed = 600f; // Đạn bay nhanh hơn

        public Projectile(float x, float y, Enemy target, int damage)
        {
            this.X = x;
            this.Y = y;
            this.Width = 6;
            this.Height = 6;
            _target = target;
            _damage = damage;
        }

        public override void Update(float deltaTime)
        {
            // Nếu mục tiêu chết hoặc biến mất -> Hủy đạn
            if (_target == null || !_target.IsActive)
            {
                this.IsActive = false;
                return;
            }

            // Hướng tới mục tiêu
            float dx = _target.X - X;
            float dy = _target.Y - Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            // Va chạm
            if (distance < 15f) // Vùng va chạm rộng hơn chút cho dễ trúng
            {
                _target.TakeDamage(_damage);
                this.IsActive = false;
                return;
            }

            // Di chuyển
            float moveX = (dx / distance) * _speed * deltaTime;
            float moveY = (dy / distance) * _speed * deltaTime;

            X += moveX;
            Y += moveY;
        }

        public override void Render(Graphics g)
        {
            // Vẽ đạn màu đen (có thể thay bằng ảnh nếu muốn)
            g.FillEllipse(Brushes.Black, X - 3, Y - 3, 6, 6);
        }
    }
}