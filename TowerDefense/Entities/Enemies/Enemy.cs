using System;
using System.Drawing;
using System.Collections.Generic;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;
using TowerDefense.Configs;
using TowerDefense.Entities.Towers;

namespace TowerDefense.Entities.Enemies
{
    public class Enemy : BaseEntity
    {
        // =========================================================
        // 1. TỐI ƯU HÓA GDI+ (STATIC RESOURCES)
        // =========================================================
        private static readonly SolidBrush _hpBackBrush = new SolidBrush(Color.Red);
        private static readonly SolidBrush _hpForeBrush = new SolidBrush(Color.LimeGreen);
        private static readonly Pen _borderPen = new Pen(Color.Black, 1);

        // =========================================================
        // 2. THUỘC TÍNH & TRẠNG THÁI
        // =========================================================

        // Thông tin cơ bản
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public int RewardGold { get; set; }
        public Color Color { get; set; }

        // Chỉ số chiến đấu (Dành cho quái phá trụ)
        public int DamageToTower { get; private set; }
        public float AttackRange { get; private set; }
        private float _attackCooldown = 1.0f; // Tốc độ đánh mặc định
        private float _currentAttackTimer = 0f;
        private Tower _targetTower;

        // Components & State
        private MovementComponent _movement;
        private float _slowTimer = 0;
        private float _rotation = 0f;
        private PointF _lastPos;

        // =========================================================
        // 3. CONSTRUCTOR
        // =========================================================
        public Enemy(List<Point> path, int enemyTypeId)
        {
            // A. Load chỉ số từ Config (Data-Driven)
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
            this.Width = 36; // To hơn chút so với tháp
            this.Height = 36;

            // B. Khởi tạo Di chuyển
            _movement = new MovementComponent(path, stat.Speed);

            // C. Đặt vị trí đầu tiên
            if (path != null && path.Count > 0)
            {
                X = path[0].X;
                Y = path[0].Y;
                _lastPos = new PointF(X, Y);
            }
        }

        // =========================================================
        // 4. GAME LOGIC (UPDATE)
        // =========================================================
        public override void Update(float deltaTime)
        {
            // A. Xử lý hiệu ứng Làm chậm (Slow)
            if (_slowTimer > 0)
            {
                _slowTimer -= deltaTime;
                if (_slowTimer <= 0) _movement.SetSpeedScale(1.0f); // Khôi phục tốc độ
            }

            // B. Giảm hồi chiêu tấn công
            if (_currentAttackTimer > 0) _currentAttackTimer -= deltaTime;

            // C. LOGIC AI TẤN CÔNG THÁP (State: Attacking)
            if (DamageToTower > 0)
            {
                // 1. Tìm mục tiêu nếu chưa có hoặc mục tiêu không hợp lệ
                if (_targetTower == null || !_targetTower.IsActive || GetDistance(_targetTower) > AttackRange)
                {
                    _targetTower = FindNearestTower();
                }

                // 2. Nếu có mục tiêu -> Tấn công
                if (_targetTower != null)
                {
                    // Xoay mặt về phía tháp
                    CalculateRotation(_targetTower.X, _targetTower.Y);

                    // Gây sát thương
                    if (_currentAttackTimer <= 0)
                    {
                        _targetTower.TakeDamage(DamageToTower);
                        _currentAttackTimer = _attackCooldown;

                        // Visual effect nhỏ (rung lắc hoặc âm thanh) có thể thêm ở đây
                    }

                    // QUAN TRỌNG: Return để DỪNG DI CHUYỂN
                    return;
                }
            }

            // D. LOGIC DI CHUYỂN (State: Moving)
            _lastPos = new PointF(X, Y);

            PointF newPos = _movement.Update(new PointF(X, Y), deltaTime);
            X = newPos.X;
            Y = newPos.Y;

            // Tính góc xoay theo hướng di chuyển
            CalculateRotation(X, Y, isMoving: true);

            // Kiểm tra về đích
            if (_movement.HasReachedEnd)
            {
                this.IsActive = false; // GameManager sẽ trừ mạng sau
            }
        }

        // =========================================================
        // 5. CÁC HÀM HỖ TRỢ (HELPER)
        // =========================================================

        private void CalculateRotation(float targetX, float targetY, bool isMoving = false)
        {
            float dx, dy;

            if (isMoving)
            {
                dx = X - _lastPos.X;
                dy = Y - _lastPos.Y;
                // Nếu đứng yên (hoặc di chuyển quá ít) thì không xoay để tránh giật hình
                if (Math.Abs(dx) < 0.1f && Math.Abs(dy) < 0.1f) return;
            }
            else
            {
                dx = targetX - X;
                dy = targetY - Y;
            }

            // Atan2 trả về Radian -> Đổi sang Độ
            // Lưu ý: Sprite gốc phải hướng sang Phải (0 độ)
            _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
        }

        private Tower FindNearestTower()
        {
            Tower nearest = null;
            float minDst = float.MaxValue;

            // Duyệt danh sách tháp từ GameManager
            foreach (var tower in GameManager.Instance.Towers)
            {
                if (!tower.IsActive) continue;

                float dst = GetDistance(tower);
                // Chỉ chọn tháp nằm trong tầm đánh
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
                // Tiền thưởng sẽ được cộng ở GameManager
            }
        }

        public void ApplySlow(float duration, float slowFactor)
        {
            _slowTimer = duration;
            _movement.SetSpeedScale(slowFactor);
        }

        // =========================================================
        // 6. RENDER (VẼ)
        // =========================================================
        public override void Render(Graphics g)
        {
            // A. Vẽ Sprite (Xoay)
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
                // Vẽ fallback nếu không có ảnh
                using (SolidBrush b = new SolidBrush(this.Color))
                {
                    g.FillEllipse(b, X - Width / 2, Y - Height / 2, Width, Height);
                }
                // Vẽ chấm hướng
                float dirX = X + (float)Math.Cos(_rotation * Math.PI / 180) * 15;
                float dirY = Y + (float)Math.Sin(_rotation * Math.PI / 180) * 15;
                g.FillEllipse(Brushes.White, dirX - 3, dirY - 3, 6, 6);
            }

            // B. Vẽ Thanh Máu (Luôn nằm ngang, không xoay)
            RenderHealthBar(g);
        }

        private void RenderHealthBar(Graphics g)
        {
            float hpPercent = (float)Health / MaxHP;
            if (hpPercent < 0) hpPercent = 0;
            if (hpPercent > 1) hpPercent = 1;

            int barWidth = 32;
            int barHeight = 4;
            float barX = X - barWidth / 2;
            float barY = Y - Height / 2 - 8;

            // Dùng Brush tĩnh để tối ưu
            g.FillRectangle(_hpBackBrush, barX, barY, barWidth, barHeight);
            g.FillRectangle(_hpForeBrush, barX, barY, barWidth * hpPercent, barHeight);
            g.DrawRectangle(_borderPen, barX, barY, barWidth, barHeight);
        }

        // Helper tính MaxHP an toàn
        private int MaxHP => MaxHealth > 0 ? MaxHealth : 100;
    }
}