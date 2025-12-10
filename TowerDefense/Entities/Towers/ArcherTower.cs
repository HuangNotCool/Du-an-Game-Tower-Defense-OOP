using System.Drawing;
using TowerDefense.Managers;

namespace TowerDefense.Entities.Towers
{
    public class ArcherTower : Tower
    {
        public ArcherTower(float x, float y) : base(x, y, 200f, 0.8f, 20)
        {
            this.Name = "Archer";
            this.Price = 100;
            this.Color = Color.Blue;
        }

        // Archer bắn đạn nhỏ, nhanh
        protected override void Shoot(Entities.Enemies.Enemy target)
        {
            var bullet = new Projectile(this.X, this.Y, target, 20); // Damage 20
            GameManager.Instance.Projectiles.Add(bullet);
        }
    }
}