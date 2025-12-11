// Entities/FloatingText.cs
using System.Drawing;
using TowerDefense.Core;

namespace TowerDefense.Entities
{
    public class FloatingText : IGameObject
    {
        public float X, Y;
        public string Text;
        public Color Color;
        public bool IsActive { get; set; } = true;
        private float _lifeTime = 1.0f;

        public FloatingText(string text, float x, float y, Color color)
        {
            Text = text; X = x; Y = y; Color = color;
        }

        public void Update(float deltaTime)
        {
            _lifeTime -= deltaTime;
            Y -= 30 * deltaTime; // Bay lên
            if (_lifeTime <= 0) IsActive = false;
        }

        public void Render(Graphics g)
        {
            g.DrawString(Text, new Font("Arial", 10, FontStyle.Bold), new SolidBrush(Color), X, Y);
        }
    }
}