using System.Collections.Generic;
using System.Drawing;

namespace TowerDefense.Managers
{
    public class LevelManager
    {
        public int CurrentLevelId { get; private set; } = 1;
        public int MaxWaves { get; private set; } = 10;
        public string Theme { get; private set; } = "Grass"; // Grass, Sand, Snow, Stone

        public List<Point> LoadLevelPath(int levelId)
        {
            CurrentLevelId = levelId;
            var path = new List<Point>();

            // Cấu hình độ khó chung (Số Wave tăng dần theo Level)
            // Level 1: 5 Wave -> Level 10: 15 Wave
            MaxWaves = 5 + levelId;

            switch (levelId)
            {
                // --- CẤP ĐỘ DỄ (1-3) ---
                case 1: // Đường thẳng đơn giản
                    Theme = "Grass";
                    path.Add(new Point(0, 300));
                    path.Add(new Point(800, 300));
                    break;

                case 2: // Chữ L
                    Theme = "Grass";
                    path.Add(new Point(100, 0));
                    path.Add(new Point(100, 450));
                    path.Add(new Point(800, 450));
                    break;

                case 3: // Chữ U
                    Theme = "Sand";
                    path.Add(new Point(0, 100));
                    path.Add(new Point(650, 100));
                    path.Add(new Point(650, 500));
                    path.Add(new Point(0, 500));
                    break;

                // --- CẤP ĐỘ TRUNG BÌNH (4-6) ---
                case 4: // Chữ Z (Ziczac lớn)
                    Theme = "Sand";
                    path.Add(new Point(0, 100));
                    path.Add(new Point(700, 100));
                    path.Add(new Point(100, 500));
                    path.Add(new Point(800, 500));
                    break;

                case 5: // Chữ M
                    Theme = "Snow";
                    path.Add(new Point(50, 600));
                    path.Add(new Point(50, 100));
                    path.Add(new Point(400, 400)); // Điểm giữa
                    path.Add(new Point(750, 100));
                    path.Add(new Point(750, 600));
                    break;

                case 6: // Xoắn ốc vuông
                    Theme = "Snow";
                    path.Add(new Point(0, 50));
                    path.Add(new Point(750, 50));
                    path.Add(new Point(750, 500));
                    path.Add(new Point(150, 500));
                    path.Add(new Point(150, 200));
                    path.Add(new Point(400, 200)); // Kết thúc ở giữa
                    break;

                // --- CẤP ĐỘ KHÓ (7-9) ---
                case 7: // Rắn săn mồi (Snake) - Nhiều khúc cua
                    Theme = "Stone";
                    path.Add(new Point(50, 0));
                    path.Add(new Point(50, 500));
                    path.Add(new Point(200, 500));
                    path.Add(new Point(200, 100));
                    path.Add(new Point(350, 100));
                    path.Add(new Point(350, 500));
                    path.Add(new Point(500, 500));
                    path.Add(new Point(500, 100));
                    path.Add(new Point(800, 100));
                    break;

                case 8: // Bậc thang
                    Theme = "Stone";
                    path.Add(new Point(0, 500));
                    path.Add(new Point(200, 500));
                    path.Add(new Point(200, 400));
                    path.Add(new Point(400, 400));
                    path.Add(new Point(400, 300));
                    path.Add(new Point(600, 300));
                    path.Add(new Point(600, 200));
                    path.Add(new Point(800, 200));
                    break;

                case 9: // Chữ W kép
                    Theme = "Stone";
                    path.Add(new Point(50, 0));
                    path.Add(new Point(50, 500));
                    path.Add(new Point(200, 200));
                    path.Add(new Point(400, 500));
                    path.Add(new Point(600, 200));
                    path.Add(new Point(750, 500));
                    path.Add(new Point(750, 0));
                    break;

                // --- CẤP ĐỘ ĐỊA NGỤC (10) ---
                case 10: // Mê cung dài nhất
                    Theme = "Lava"; // Cần ảnh Lava.png hoặc nó sẽ dùng màu mặc định
                    MaxWaves = 20; // 20 Wave cho màn cuối
                    path.Add(new Point(0, 50));
                    path.Add(new Point(750, 50));
                    path.Add(new Point(750, 150));
                    path.Add(new Point(100, 150));
                    path.Add(new Point(100, 250));
                    path.Add(new Point(750, 250));
                    path.Add(new Point(750, 350));
                    path.Add(new Point(100, 350));
                    path.Add(new Point(100, 450));
                    path.Add(new Point(750, 450));
                    path.Add(new Point(750, 600));
                    break;

                default: // Fallback
                    path.Add(new Point(0, 300));
                    path.Add(new Point(800, 300));
                    break;
            }

            return path;
        }
    }
}