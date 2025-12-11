using System;
using System.Drawing;
using System.Collections.Generic;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;
using TowerDefense.Configs;         // Để lấy thông số quái
using TowerDefense.Entities.Towers; // Để tương tác với Tower

namespace TowerDefense.Entities.Enemies
{
    public class Enemy : BaseEntity
    {
        // --- CHỈ SỐ CƠ BẢN ---
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public int RewardGold { get; set; }
        public Color Color { get; set; } // Màu đại diện nếu chưa có ảnh

        // --- CHỈ SỐ TẤN CÔNG (MỚI) ---
        public int DamageToTower { get; private set; } // Sát thương lên trụ
        public float AttackRange { get; private set; } // Tầm đánh trụ
        private float _attackCooldown = 1.0f;          // Tốc độ đánh (1s/hit)
        private float _currentAttackTimer = 0f;
        private Tower _targetTower;                    // Tháp đang bị nhắm mục tiêu

        // --- COMPONENTS & STATE ---
        private MovementComponent _movement;
        private float _slowTimer = 0;   // Thời gian bị làm chậm
        private float _rotation = 0f;   // Góc quay
        private PointF _lastPos;        // Vị trí cũ

        // =========================================================
        // CONSTRUCTOR
        // =========================================================
        public Enemy(List<Point> path, int enemyTypeId)
        {
            // 1. Load chỉ số từ GameConfig
            if (enemyTypeId < 0 || enemyTypeId >= GameConfig.Enemies.Length) enemyTypeId = 0;
            var stat = GameConfig.Enemies[enemyTypeId];

            this.Name = stat.Name;
            this.MaxHealth = stat.MaxHealth;
            this.Health = stat.MaxHealth;
            this.RewardGold = stat.Reward;
            this.DamageToTower = stat.DamageToTower;
            this.AttackRange = stat.AttackRange;
            this.Color = stat.Color;

            // Kích thước chuẩn
            this.Width = 32;
            this.Height = 32;

            // 2. Khởi tạo Component Di chuyển
            _movement = new MovementComponent(path, stat.Speed);

            // 3. Đặt vị trí xuất phát
            if (path != null && path.Count > 0)
            {
                X = path[0].X;
                Y = path[0].Y;
                _lastPos = new PointF(X, Y);
            }
        }

        // =========================================================
        // GAME LOGIC (UPDATE)
        // =========================================================
        public override void Update(float deltaTime)
        {
            // 1. Xử lý hiệu ứng làm chậm (Slow)
            if (_slowTimer > 0)
            {
                _slowTimer -= deltaTime;
                if (_slowTimer <= 0) _movement.SetSpeedScale(1.0f); // Hết chậm
            }

            // 2. Giảm hồi chiêu tấn công
            if (_currentAttackTimer > 0) _currentAttackTimer -= deltaTime;

            // 3. LOGIC TẤN CÔNG THÁP (AI)
            // Chỉ chạy nếu quái này có khả năng đánh trụ
            if (DamageToTower > 0)
            {
                // Nếu chưa có mục tiêu hoặc mục tiêu đã chết/quá xa -> Tìm mới
                if (_targetTower == null || !_targetTower.IsActive || GetDistance(_targetTower) > AttackRange)
                {
                    _targetTower = FindNearestTower();
                }

                // Nếu đã có mục tiêu trong tầm đánh
                if (_targetTower != null)
                {
                    // --- HÀNH ĐỘNG TẤN CÔNG ---

                    // A. Xoay mặt về phía tháp
                    CalculateRotation(_targetTower.X, _targetTower.Y);

                    // B. Gây sát thương
                    if (_currentAttackTimer <= 0)
                    {
                        _targetTower.TakeDamage(DamageToTower);
                        _currentAttackTimer = _attackCooldown; // Reset cooldown

                        // Hiệu ứng Visual (Chớp đỏ hoặc rung - Optional)
                    }

                    // C. DỪNG DI CHUYỂN (Return ngay lập tức)
                    return;
                }
            }

            // 4. LOGIC DI CHUYỂN (Chỉ chạy khi không đánh nhau)
            _lastPos = new PointF(X, Y); // Lưu vị trí cũ

            PointF newPos = _movement.Update(new PointF(X, Y), deltaTime);
            X = newPos.X;
            Y = newPos.Y;

            // Tính góc xoay theo hướng di chuyển
            CalculateRotation(X, Y, true);

            // Kiểm tra về đích
            if (_movement.HasReachedEnd)
            {
                this.IsActive = false; // GameManager sẽ trừ mạng sau
            }
        }

        // =========================================================
        // CÁC HÀM HỖ TRỢ (HELPER)
        // =========================================================

        private void CalculateRotation(float targetX, float targetY, bool isMoving = false)
        {
            float dx, dy;

            if (isMoving)
            {
                // Nếu đang đi: Tính hướng dựa trên sự thay đổi vị trí so với khung hình trước
                dx = X - _lastPos.X;
                dy = Y - _lastPos.Y;
                // Nếu đứng yên thì không xoay
                if (Math.Abs(dx) < 0.1f && Math.Abs(dy) < 0.1f) return;
            }
            else
            {
                // Nếu đang đánh: Tính hướng tới mục tiêu
                dx = targetX - X;
                dy = targetY - Y;
            }

            // Atan2 trả về Radian -> Đổi sang Độ
            _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
        }

        private Tower FindNearestTower()
        {
            Tower nearest = null;
            float minDst = float.MaxValue;

            foreach (var tower in GameManager.Instance.Towers)
            {
                if (!tower.IsActive) continue;

                float dst = GetDistance(tower);
                // Tìm tháp gần nhất VÀ phải nằm trong tầm đánh
                if (dst <= AttackRange && dst < minDst)
                {
                    minDst = dst;
                    nearest = tower;
                }
            }
            return nearest;
        }

        private float GetDistance(BaseEntity target)
        {
            float dx = X - target.X;
            float dy = Y - target.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                IsActive = false;
                // Tiền thưởng được xử lý bên GameManager khi check IsActive
            }
        }

        public void ApplySlow(float duration, float slowFactor)
        {
            _slowTimer = duration;
            _movement.SetSpeedScale(slowFactor);
        }

        // =========================================================
        // RENDER (VẼ)
        // =========================================================
        public override void Render(Graphics g)
        {
            // 1. Vẽ Sprite (Ưu tiên ảnh riêng theo tên, nếu không có thì dùng ảnh chung Enemy)
            // Ví dụ: Goblin.png, Orc.png...
            Image sprite = ResourceManager.GetImage(this.Name);
            if (sprite == null) sprite = ResourceManager.GetImage("Enemy"); // Fallback

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
                // Fallback: Vẽ hình tròn màu theo Config
                g.FillEllipse(new SolidBrush(this.Color), X - Width / 2, Y - Height / 2, Width, Height);
                // Vẽ viền đen để rõ hướng (chấm nhỏ phía trước)
                float dirX = X + (float)Math.Cos(_rotation * Math.PI / 180) * 15;
                float dirY = Y + (float)Math.Sin(_rotation * Math.PI / 180) * 15;
                g.FillEllipse(Brushes.White, dirX - 3, dirY - 3, 6, 6);
            }

            // 2. Vẽ Thanh Máu
            RenderHealthBar(g);
        }

        private void RenderHealthBar(Graphics g)
        {
            float hpPercent = (float)Health / MaxHealth;
            if (hpPercent < 0) hpPercent = 0;

            int barWidth = 32;
            int barHeight = 5;
            float barX = X - barWidth / 2;
            float barY = Y - Height / 2 - 10;

            g.FillRectangle(Brushes.Red, barX, barY, barWidth, barHeight);
            g.FillRectangle(Brushes.LightGreen, barX, barY, barWidth * hpPercent, barHeight);
            g.DrawRectangle(Pens.Black, barX, barY, barWidth, barHeight);
        }
    }
}