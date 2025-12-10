using System.Drawing;
using TowerDefense.Managers;

namespace TowerDefense.Entities.Towers
{
    public class CannonTower : Tower
    {
        public CannonTower(float x, float y) : base(x, y, 120f, 2.0f, 50)
        {
            this.Name = "Cannon";
            this.Price = 200;
            this.Color = Color.Black;
        }

        // Cannon bắn đạn to, mạnh
        protected override void Shoot(Entities.Enemies.Enemy target)
        {
            var bullet = new Projectile(this.X, this.Y, target, 60); // Damage 60
            // Sau này có thể chỉnh bullet.Size to hơn ở đây
            GameManager.Instance.Projectiles.Add(bullet);
        }
    }
}