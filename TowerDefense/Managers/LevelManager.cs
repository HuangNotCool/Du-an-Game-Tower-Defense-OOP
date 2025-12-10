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
                    // MAP 1: Đường chữ L (Dễ)
                    // Xuất phát trái -> Giữa -> Xuất hiện ở đáy
                    path.Add(new Point(0, 200));
                    path.Add(new Point(400, 200));
                    path.Add(new Point(400, 500));
                    path.Add(new Point(800, 500));
                    break;

                case 2:
                    // MAP 2: Đường chữ U (Trung bình)
                    // Trên xuống -> Sang phải -> Lên trên
                    path.Add(new Point(150, 0));
                    path.Add(new Point(150, 450));
                    path.Add(new Point(650, 450));
                    path.Add(new Point(650, 0));
                    break;

                case 3:
                    // MAP 3: Đường Ziczac (Khó)
                    // Đi như rắn săn mồi
                    path.Add(new Point(0, 100));
                    path.Add(new Point(200, 100));
                    path.Add(new Point(200, 400));
                    path.Add(new Point(400, 400));
                    path.Add(new Point(400, 100));
                    path.Add(new Point(600, 100));
                    path.Add(new Point(600, 400));
                    path.Add(new Point(800, 400));
                    break;

                default:
                    // Mặc định Map 1
                    path.Add(new Point(0, 200));
                    path.Add(new Point(800, 200));
                    break;
            }

            return path;
        }
    }
}