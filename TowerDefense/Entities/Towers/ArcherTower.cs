using System.Drawing;
using TowerDefense.Data;     // Để lấy UserProfile
using TowerDefense.Managers; // <--- QUAN TRỌNG: Để gọi GameManager
using TowerDefense.Entities; // Để gọi Projectile

namespace TowerDefense.Entities.Towers
{
    public class ArcherTower : Tower
    {
        private int _bonusDamage = 0;

        public ArcherTower(float x, float y)
            : base(x, y, 200f, 0.8f, 20)
        {
            this.Name = "Archer";
            this.Price = 100;
            this.Color = Color.Blue;

            // --- ÁP DỤNG META-UPGRADE TỪ SHOP ---
            var profile = UserProfile.Instance;

            // Tính toán chỉ số cộng thêm
            _bonusDamage = profile.ArcherDamageLevel * 5;
            float bonusRange = profile.ArcherRangeLevel * 10f;

            // Cập nhật lại tầm bắn (Range) cho Component
            // (Tạo mới combat component với range đã được buff)
            this._combat = new Components.CombatComponent(200f + bonusRange, 0.8f);
        }

        protected override void Shoot(Entities.Enemies.Enemy target)
        {
            // Damage gốc (20) + Bonus từ Shop
            int totalDamage = 20 + _bonusDamage;

            var bullet = new Projectile(this.X, this.Y, target, totalDamage);

            // Bây giờ nó đã hiểu GameManager là ai
            GameManager.Instance.Projectiles.Add(bullet);
        }
    }
}