using System;
using System.Drawing;
using System.Collections.Generic; // Để dùng List
using TowerDefense.Core;
using TowerDefense.Entities.Enemies;
using TowerDefense.Managers;

namespace TowerDefense.Entities
{
    public class Projectile : BaseEntity
    {
        private Enemy _target;
        private int _damage;
        private float _speed = 600f;
        private string _type; // "Arrow", "Bomb", "Ice", "Magic", "Fire"
        private float _rotation = 0f;

        public Projectile(float x, float y, Enemy target, int damage, string type)
        {
            this.X = x; this.Y = y;
            this.Width = 16; this.Height = 16;
            _target = target;
            _damage = damage;
            _type = type;

            // Đạn tên lửa/phép bay chậm hơn chút để nhìn cho rõ
            if (_type == "Bomb" || _type == "Rocket") _speed = 400f;
        }

        public override void Update(float deltaTime)
        {
            // Nếu mục tiêu chết mà không phải đạn nổ -> Hủy đạn
            // (Đạn nổ thì vẫn bay đến vị trí cũ để nổ)
            if ((_target == null || !_target.IsActive) && _type != "Bomb" && _type != "Rocket")
            {
                this.IsActive = false;
                return;
            }

            // Tính hướng
            // Lưu ý: Nếu target chết, ta vẫn lấy tọa độ cuối cùng của nó (cần lưu lastPos nếu muốn chuẩn xác hơn)
            // Ở đây đơn giản hóa: nếu target null thì hủy luôn cho đỡ lỗi.
            if (_target == null) { IsActive = false; return; }

            float dx = _target.X - X;
            float dy = _target.Y - Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

            // VA CHẠM
            if (distance < 15f)
            {
                HitTarget();
                this.IsActive = false;
                return;
            }

            // Di chuyển
            float moveX = (dx / distance) * _speed * deltaTime;
            float moveY = (dy / distance) * _speed * deltaTime;
            X += moveX; Y += moveY;
        }

        private void HitTarget()
        {
            // 1. Xử lý sát thương chính
            if (_target != null && _target.IsActive)
            {
                _target.TakeDamage(_damage);

                // Hiệu ứng riêng cho từng loại đạn
                ApplySpecialEffects(_target);
            }

            // 2. Xử lý nổ lan (Area of Effect) cho Bomb/Rocket
            if (_type == "Bomb" || _type == "Rocket" || _type == "Fire")
            {
                Explode();
            }
        }

        private void ApplySpecialEffects(Enemy enemy)
        {
            switch (_type)
            {
                case "Ice": // Làm chậm 50% trong 2 giây
                    enemy.ApplySlow(2.0f, 0.5f);
                    GameManager.Instance.ShowFloatingText("Slowed!", enemy.X, enemy.Y - 30, Color.Cyan);
                    break;

                case "Fire": // Có thể thêm logic đốt máu theo thời gian (Dot)
                    // enemy.ApplyBurn(...);
                    break;

                case "Magic": // Có thể thêm logic xuyên giáp (tăng damage nếu giáp to)
                    break;
            }
        }

        private void Explode()
        {
            float explosionRadius = 100f; // Bán kính nổ
            int aoeDamage = _damage / 2;  // Sát thương lan (50% sát thương gốc)

            // Hiệu ứng nổ
            GameManager.Instance.ShowFloatingText("BOOM!", X, Y, Color.OrangeRed);
            SoundManager.Play("explosion");

            // Duyệt qua tất cả quái để xem con nào đứng gần
            // (Lưu ý: Dùng ToList() để tạo bản sao danh sách, tránh lỗi khi đang duyệt mà list thay đổi)
            foreach (var enemy in new List<Enemy>(GameManager.Instance.Enemies))
            {
                if (!enemy.IsActive) continue;

                // Tính khoảng cách từ vụ nổ đến quái
                float dist = (float)Math.Sqrt(Math.Pow(enemy.X - X, 2) + Math.Pow(enemy.Y - Y, 2));

                if (dist <= explosionRadius)
                {
                    enemy.TakeDamage(aoeDamage);

                    // Nếu là Fire thì lan cả hiệu ứng đốt, Ice lan hiệu ứng băng...
                    if (_type == "Ice") enemy.ApplySlow(2.0f, 0.5f);
                }
            }
        }

        public override void Render(Graphics g)
        {
            Image sprite = ResourceManager.GetImage(_type); // Arrow, Bomb, Ice...

            if (sprite != null)
            {
                var state = g.Save();
                g.TranslateTransform(X, Y);
                g.RotateTransform(_rotation);
                g.DrawImage(sprite, -Width / 2, -Height / 2, Width, Height);
                g.Restore(state);
            }
            else
            {
                // Fallback màu sắc
                Color c = Color.Black;
                if (_type == "Ice") c = Color.Cyan;
                if (_type == "Fire") c = Color.Orange;
                if (_type == "Magic") c = Color.Purple;

                g.FillEllipse(new SolidBrush(c), X - 4, Y - 4, 8, 8);
            }
        }
    }
}