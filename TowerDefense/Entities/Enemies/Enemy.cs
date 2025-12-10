using System;
using System.Drawing;
using System.Collections.Generic;
using TowerDefense.Core;
using TowerDefense.Components;
using TowerDefense.Managers;

namespace TowerDefense.Entities.Enemies
{
    public class Enemy : BaseEntity
    {
        // --- CHỈ SỐ CƠ BẢN ---
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public int RewardGold { get; set; } = 10; // Tiền rơi ra khi chết

        // --- COMPONENTS & STATE ---
        private MovementComponent _movement;
        private float _slowTimer = 0;   // Thời gian bị làm chậm còn lại
        private float _rotation = 0f;   // Góc quay hiện tại
        private PointF _lastPos;        // Vị trí khung hình trước (để tính hướng)

        // --- CONSTRUCTOR ---
        public Enemy(List<Point> path)
        {
            // Cấu hình chỉ số (Sau này có thể truyền vào Constructor để tạo quái mạnh/yếu)
            MaxHealth = 100;
            Health = MaxHealth;
            this.Width = 32;
            this.Height = 32;

            // Khởi tạo Component di chuyển với tốc độ 100 pixel/s
            _movement = new MovementComponent(path, 100f);

            // Đặt vị trí xuất phát
            if (path != null && path.Count > 0)
            {
                X = path[0].X;
                Y = path[0].Y;
                _lastPos = new PointF(X, Y);
            }
        }

        // --- LOGIC GAME LOOP (UPDATE) ---
        public override void Update(float deltaTime)
        {
            // 1. Xử lý hiệu ứng làm chậm (Slow)
            if (_slowTimer > 0)
            {
                _slowTimer -= deltaTime;
                if (_slowTimer <= 0)
                {
                    // Hết thời gian chậm -> Khôi phục tốc độ gốc (Scale = 1.0)
                    _movement.SetSpeedScale(1.0f);
                }
            }

            // 2. Lưu vị trí cũ trước khi di chuyển
            _lastPos = new PointF(X, Y);

            // 3. Cập nhật vị trí mới từ Component
            PointF newPos = _movement.Update(new PointF(X, Y), deltaTime);
            X = newPos.X;
            Y = newPos.Y;

            // 4. Tính góc quay dựa trên hướng di chuyển
            // Chỉ xoay nếu có di chuyển thực sự (để tránh quái giật về 0 độ khi đứng yên)
            if (Math.Abs(X - _lastPos.X) > 0.1f || Math.Abs(Y - _lastPos.Y) > 0.1f)
            {
                float dx = X - _lastPos.X;
                float dy = Y - _lastPos.Y;

                // Atan2 trả về góc tính bằng Radian -> Đổi sang Độ
                _rotation = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
            }

            // 5. Kiểm tra về đích
            if (_movement.HasReachedEnd)
            {
                this.IsActive = false; // Xóa khỏi game (GameManager sẽ trừ mạng sau)
            }
        }

        // --- TƯƠNG TÁC (INTERFACE) ---

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                IsActive = false; // Chết
                // Cộng tiền thưởng (Có thể xử lý ở GameManager, hoặc cộng trực tiếp ở đây cũng được)
                // Tuy nhiên, logic chuẩn là GameManager kiểm tra IsActive để cộng tiền
            }
        }

        // Hàm nhận hiệu ứng làm chậm (từ Tháp băng hoặc Skill)
        public void ApplySlow(float duration, float slowFactor)
        {
            _slowTimer = duration;
            _movement.SetSpeedScale(slowFactor); // Ví dụ factor 0.5 là chậm 50%
        }

        // --- LOGIC VẼ (RENDER) ---
        public override void Render(Graphics g)
        {
            // 1. Vẽ hình ảnh Quái (Xoay theo hướng đi)
            Image sprite = ResourceManager.GetImage("Enemy");

            if (sprite != null)
            {
                var state = g.Save(); // Lưu trạng thái Graphics hiện tại

                // Dời gốc tọa độ về tâm quái
                g.TranslateTransform(X, Y);

                // Xoay khung hình
                g.RotateTransform(_rotation);

                // Vẽ ảnh (Tâm ảnh trùng gốc tọa độ)
                // Lưu ý: Nếu ảnh gốc của bạn hướng sang PHẢI -> Không cần cộng góc
                // Nếu ảnh gốc hướng lên TRÊN -> Cần g.RotateTransform(_rotation + 90);
                g.DrawImage(sprite, -Width / 2, -Height / 2, Width, Height);

                g.Restore(state); // Khôi phục trạng thái để không ảnh hưởng các vật thể khác
            }
            else
            {
                // Fallback: Vẽ hình tròn đỏ nếu thiếu ảnh
                g.FillEllipse(Brushes.Red, X - Width / 2, Y - Height / 2, Width, Height);
            }

            // 2. Vẽ Thanh Máu (Health Bar)
            // Vẽ sau cùng để nó nằm ĐÈ lên quái và KHÔNG bị xoay theo quái
            RenderHealthBar(g);
        }

        private void RenderHealthBar(Graphics g)
        {
            float hpPercent = (float)Health / MaxHP;
            // Xử lý trường hợp Health > MaxHP (nếu có buff) hoặc < 0
            if (hpPercent < 0) hpPercent = 0;
            if (hpPercent > 1) hpPercent = 1;

            // Kích thước thanh máu
            int barWidth = 32;
            int barHeight = 5;
            float barX = X - barWidth / 2;
            float barY = Y - Height / 2 - 10; // Vẽ trên đầu quái 10px

            // Vẽ nền đỏ (Máu đã mất)
            g.FillRectangle(Brushes.Red, barX, barY, barWidth, barHeight);

            // Vẽ phần xanh (Máu hiện tại)
            g.FillRectangle(Brushes.LightGreen, barX, barY, barWidth * hpPercent, barHeight);

            // Vẽ viền đen cho rõ
            g.DrawRectangle(Pens.Black, barX, barY, barWidth, barHeight);
        }

        // Property phụ trợ để tính toán thanh máu chuẩn
        private int MaxHP => MaxHealth > 0 ? MaxHealth : 100;
    }
}