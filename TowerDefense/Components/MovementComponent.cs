using System;
using System.Collections.Generic;
using System.Drawing;

namespace TowerDefense.Components
{
    public class MovementComponent
    {
        private List<Point> _pathPoints;
        private int _currentPointIndex = 0;

        private float _baseSpeed;    // Tốc độ gốc
        private float _currentSpeed; // Tốc độ hiện tại (sau khi bị slow)

        public bool HasReachedEnd { get; private set; } = false;

        public MovementComponent(List<Point> path, float speed)
        {
            _pathPoints = path;
            _baseSpeed = speed;
            _currentSpeed = speed; // Mặc định tốc độ hiện tại = tốc độ gốc
        }

        // --- HÀM MỚI: CHỈNH TỐC ĐỘ (QUAN TRỌNG CHO SKILL) ---
        public void SetSpeedScale(float scale)
        {
            // scale = 1.0 (bình thường), 0.5 (chậm 50%), 0.0 (đứng yên)
            _currentSpeed = _baseSpeed * scale;
        }

        public PointF Update(PointF currentPos, float deltaTime)
        {
            if (_pathPoints == null || _pathPoints.Count == 0 || HasReachedEnd)
                return currentPos;

            // 1. Xác định điểm đến tiếp theo
            Point target = _pathPoints[_currentPointIndex];

            // 2. Tính khoảng cách
            float dx = target.X - currentPos.X;
            float dy = target.Y - currentPos.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            // 3. Kiểm tra đã đến nơi chưa
            if (distance < 5f) // Tăng vùng nhận diện lên 5px cho mượt
            {
                _currentPointIndex++;
                if (_currentPointIndex >= _pathPoints.Count)
                {
                    HasReachedEnd = true;
                }
                return currentPos;
            }

            // 4. Di chuyển
            float moveX = (dx / distance) * _currentSpeed * deltaTime;
            float moveY = (dy / distance) * _currentSpeed * deltaTime;

            return new PointF(currentPos.X + moveX, currentPos.Y + moveY);
        }
    }
}