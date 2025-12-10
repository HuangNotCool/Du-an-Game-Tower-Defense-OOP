using System.Collections.Generic;
using System.Drawing;
using TowerDefense.Entities.Enemies;
using TowerDefense.Entities.Enemies;

namespace TowerDefense.Components
{
    public class CombatComponent
    {
        // ... (Giữ nguyên các biến cũ: _range, _reloadTime, _currentCooldown) ...
        private float _range;
        private float _reloadTime;
        private float _currentCooldown = 0;

        public CombatComponent(float range, float reloadTime)
        {
            _range = range;
            _reloadTime = reloadTime;
        }

        public void Update(float deltaTime)
        {
            if (_currentCooldown > 0) _currentCooldown -= deltaTime;
        }

        public bool CanShoot() => _currentCooldown <= 0;
        public void ResetCooldown() => _currentCooldown = _reloadTime;

        // --- CODE MỚI THÊM ---

        // Hàm tìm mục tiêu: Chọn con quái nào gần Base nhất (hoặc đầu danh sách) trong tầm bắn
        public Enemy FindTarget(List<Enemy> enemies, PointF shooterPos)
        {
            // Duyệt qua tất cả quái đang sống
            foreach (var enemy in enemies)
            {
                if (!enemy.IsActive) continue;

                // Tính khoảng cách
                float dx = enemy.X - shooterPos.X;
                float dy = enemy.Y - shooterPos.Y;
                float distSqr = dx * dx + dy * dy;

                // Nếu trong tầm bắn (dùng bình phương để tối ưu)
                if (distSqr <= _range * _range)
                {
                    return enemy; // Trả về ngay con đầu tiên tìm thấy (đơn giản nhất)
                    // Sau này sẽ nâng cấp thuật toán chọn con "nguy hiểm nhất"
                }
            }
            return null; // Không tìm thấy ai
        }
    }
}