using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TowerDefense.Managers
{
    public static class ResourceManager
    {
        public static Dictionary<string, Image> Images = new Dictionary<string, Image>();
        private static string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Images");

        public static void LoadResources()
        {
            // 1. Load các ảnh cơ bản (UI, Nền)
            LoadImage("Grass", "grass.png");

            // 2. TỰ ĐỘNG LOAD TẤT CẢ ẢNH TRONG THƯ MỤC
            // (Yêu cầu tên file ảnh phải trùng với tên trong Config. VD: "Slime.png", "Archer.png")
            if (Directory.Exists(_basePath))
            {
                string[] files = Directory.GetFiles(_basePath, "*.png");
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string key = Path.GetFileNameWithoutExtension(file); // Slime.png -> Key: Slime

                    // Chỉ load nếu chưa có trong dictionary
                    if (!Images.ContainsKey(key))
                    {
                        LoadImage(key, fileName);
                    }
                }
            }

            // 3. Load thủ công các loại đạn (nếu tên file khác key)
            // Nếu bạn đặt tên file là Arrow.png, Bomb.png thì không cần dòng này nữa
            if (!Images.ContainsKey("Arrow")) LoadImage("Arrow", "arrow.png");
            if (!Images.ContainsKey("Bomb")) LoadImage("Bomb", "bomb.png");
            if (!Images.ContainsKey("Ice")) LoadImage("Ice", "ice_ball.png"); // Ví dụ
        }

        private static void LoadImage(string key, string fileName)
        {
            try
            {
                string fullPath = Path.Combine(_basePath, fileName);
                if (File.Exists(fullPath))
                {
                    Images[key] = Image.FromFile(fullPath);
                }
                // Không tạo ảnh lỗi màu hồng ở đây nữa để tránh spam log
            }
            catch { }
        }

        public static Image GetImage(string key)
        {
            if (Images.ContainsKey(key)) return Images[key];
            return null; // Trả về null để Logic Render tự vẽ hình fallback
        }
    }
}