using System.Collections.Generic;
using System.Drawing;
using System;

namespace TowerDefense.Components
{
    public class MovementComponent
    {
        private List<Point> _pathPoints;
        private int _currentPointIndex = 0;
        private float _baseSpeed; // Tốc độ gốc
        private float _currentSpeed; // Tốc độ hiện tại (có thể bị chậm)

        public bool HasReachedEnd { get; private set; } = false;

        public MovementComponent(List<Point> path, float speed)
        {
            _pathPoints = path;
            _baseSpeed = speed;
            _currentSpeed = speed;
        }

        // --- HÀM MỚI: ĐIỀU CHỈNH TỐC ĐỘ (Scale: 1.0 là bình thường, 0.5 là chậm một nửa) ---
        public void SetSpeedScale(float scale)
        {
            _currentSpeed = _baseSpeed * scale;
        }

        public PointF Update(PointF currentPos, float deltaTime)
        {
            if (_pathPoints == null || _pathPoints.Count == 0 || HasReachedEnd)
                return currentPos;

            Point target = _pathPoints[_currentPointIndex];
            float dx = target.X - currentPos.X;
            float dy = target.Y - currentPos.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance < 2f)
            {
                _currentPointIndex++;
                if (_currentPointIndex >= _pathPoints.Count) HasReachedEnd = true;
                return currentPos;
            }

            // Dùng _currentSpeed thay vì _baseSpeed
            float moveX = (dx / distance) * _currentSpeed * deltaTime;
            float moveY = (dy / distance) * _currentSpeed * deltaTime;

            return new PointF(currentPos.X + moveX, currentPos.Y + moveY);
        }
    }
}