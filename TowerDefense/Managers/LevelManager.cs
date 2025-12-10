using System.Collections.Generic;
using System.Drawing;

namespace TowerDefense.Managers
{
    public class LevelManager
    {
        // Trả về đường đi của quái dựa trên Level ID
        public List<Point> LoadLevelPath(int levelId)
        {
            var path = new List<Point>();

            switch (levelId)
            {
                case 1:
                    // MAP 1: Đường chữ L đơn giản
                    path.Add(new Point(0, 200));   // Xuất phát bên trái
                    path.Add(new Point(400, 200)); // Đi thẳng đến giữa
                    path.Add(new Point(400, 500)); // Rẽ xuống
                    path.Add(new Point(800, 500)); // Rẽ phải về đích
                    break;

                case 2:
                    // MAP 2: Đường chữ U (Khó hơn)
                    path.Add(new Point(100, 0));   // Xuất phát từ trên
                    path.Add(new Point(100, 400)); // Đi xuống
                    path.Add(new Point(600, 400)); // Sang phải
                    path.Add(new Point(600, 0));   // Đi lên về đích
                    break;

                default:
                    // Default map
                    path.Add(new Point(0, 100));
                    path.Add(new Point(800, 100));
                    break;
            }

            return path;
        }
    }
}