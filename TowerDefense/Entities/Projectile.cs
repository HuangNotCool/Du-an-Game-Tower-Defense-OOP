using System;
using System.Drawing;
using TowerDefense.Entities.Enemies;
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
        private float _aoeRadius; // Bán kính nổ

        // Biến lưu tên ảnh đạn
        private string _textureKey;

        // Constructor mới nhận thêm textureKey
        public Projectile(float x, float y, Enemy target, int damage, float speed, ProjectileType type, float aoeRadius = 0, string textureKey = "Arrow")
        {
            X = x;
            Y = y;
            _target = target;
            _damage = damage;
            _speed = speed;
            _type = type;
            _aoeRadius = aoeRadius;
            _textureKey = textureKey; // Lưu lại để dùng khi Render
        }

        public void Update(float deltaTime)
        {
            if (_target == null || !_target.IsActive)
            {
                IsActive = false;
                return;
            }

            // Tính hướng bay
            float dx = _target.X - X;
            float dy = _target.Y - Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            // Tốc độ bay trong frame này
            float move = _speed * deltaTime;

            // Kiểm tra va chạm
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
                    // Nổ lan
                    GameManager.Instance.CreateExplosion(X, Y, _aoeRadius > 0 ? _aoeRadius : 60f);
                    ApplyAreaDamage(X, Y, _aoeRadius > 0 ? _aoeRadius : 60f, _damage);
                }
                else if (_type == ProjectileType.Ice)
                {
                    // Hiệu ứng băng
                    GameManager.Instance.CreateIceEffect(X, Y);
                    _target.TakeDamage(_damage);
                    _target.ApplySlow(2.0f, 0.5f); // Làm chậm 50%
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
            if (!IsActive) return;

            // 1. Xác định tên ảnh từ biến đã lưu
            string imageKey = string.IsNullOrEmpty(_textureKey) ? "Arrow" : _textureKey;

            // 2. Tinh chỉnh kích thước hiển thị dựa trên tên ảnh
            float size = 20;
            if (imageKey == "Bomb") size = 24;
            else if (imageKey == "Jet") size = 30;
            else if (imageKey == "MBullet") size = 22;

            // 3. Lấy ảnh và vẽ
            Image projImg = ResourceManager.GetImage(imageKey);

            if (projImg != null)
            {
                g.DrawImage(projImg, X - size / 2, Y - size / 2, size, size);
            }
            else
            {
                // Fallback: Vẽ chấm màu nếu không tìm thấy ảnh
                using (SolidBrush b = new SolidBrush(Color.Yellow))
                    g.FillEllipse(b, X - 5, Y - 5, 10, 10);
            }
        }
    }
}