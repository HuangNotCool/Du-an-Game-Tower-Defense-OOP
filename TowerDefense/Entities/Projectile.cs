using System;
using System.Drawing;
using TowerDefense.Entities.Enemies;
using TowerDefense.Entities.Towers;
using TowerDefense.Managers;

namespace TowerDefense.Entities
{
    public enum ProjectileType { Bullet, Missile, Laser, Ice }

    public class Projectile
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Enemy _target;
        private int _damage;
        private float _speed;
        private ProjectileType _type;
        private float _aoeRadius; // Bán kính nổ (nếu là Missile)

        public Projectile(float x, float y, Enemy target, int damage, float speed, ProjectileType type, float aoeRadius = 0)
        {
            X = x; Y = y;
            _target = target;
            _damage = damage;
            _speed = speed;
            _type = type;
            _aoeRadius = aoeRadius;
        }

        public void Update(float deltaTime)
        {
            if (_target == null || !_target.IsActive)
            {
                IsActive = false;
                return;
            }

            // Hướng bay
            float dx = _target.X - X;
            float dy = _target.Y - Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            // Tốc độ bay
            float move = _speed * deltaTime;

            // Nếu chạm mục tiêu
            if (dist <= move)
            {
                HitTarget();
                return;
            }

            // Di chuyển
            X += (dx / dist) * move;
            Y += (dy / dist) * move;
        }

        private void HitTarget()
        {
            IsActive = false;

            if (_target != null && _target.IsActive)
            {
                if (_type == ProjectileType.Missile)
                {
                    // --- FIX LỖI TẠI ĐÂY: Truyền bán kính nổ (aoeRadius) thay vì Color ---
                    // Tham số: X, Y, Radius
                    GameManager.Instance.CreateExplosion(X, Y, _aoeRadius > 0 ? _aoeRadius : 60f);

                    // Gây dame lan
                    ApplyAreaDamage(X, Y, _aoeRadius > 0 ? _aoeRadius : 60f, _damage);
                }
                else if (_type == ProjectileType.Ice)
                {
                    GameManager.Instance.CreateIceEffect(X, Y);
                    _target.TakeDamage(_damage);
                    _target.ApplySlow(2.0f, 0.5f); // Làm chậm 50% trong 2s
                }
                else
                {
                    // Đạn thường
                    GameManager.Instance.CreateHitEffect(X, Y);
                    _target.TakeDamage(_damage);
                }
            }
        }

        private void ApplyAreaDamage(float x, float y, float radius, int damage)
        {
            // Tìm tất cả quái trong vùng nổ
            foreach (var enemy in GameManager.Instance.Enemies)
            {
                float dx = enemy.X - x;
                float dy = enemy.Y - y;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        public void Render(Graphics g)
        {
            float size = 6;
            Brush brush = Brushes.Yellow;

            switch (_type)
            {
                case ProjectileType.Missile: brush = Brushes.OrangeRed; size = 8; break;
                case ProjectileType.Ice: brush = Brushes.Cyan; size = 6; break;
                case ProjectileType.Laser: brush = Brushes.Lime; size = 4; break;
            }

            g.FillEllipse(brush, X - size / 2, Y - size / 2, size, size);
        }
    }
}