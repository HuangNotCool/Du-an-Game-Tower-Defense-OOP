using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TowerDefense.Managers
{
    public static class ResourceManager
    {
        // Dictionary để lưu trữ ảnh đã load (Cache)
        public static Dictionary<string, Image> Images = new Dictionary<string, Image>();

        // Đường dẫn gốc tới thư mục Assets
        // Cách lấy đường dẫn này hoạt động cả khi chạy Debug lẫn Release
        private static string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Assets\Images");

        public static void LoadResources()
        {
            // Tải ảnh Tháp
            LoadImage("Archer", "archer.png");
            LoadImage("Cannon", "cannon.png");

            // Tải ảnh Quái
            LoadImage("Enemy", "enemy.png");

            // Tải ảnh Nền (nếu có)
            LoadImage("Grass", "grass.png");
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
                else
                {
                    // Nếu không tìm thấy ảnh, tạo một ảnh màu tạm để không crash game
                    Bitmap bmp = new Bitmap(64, 64);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Magenta); // Màu hồng báo lỗi
                        g.DrawString(key, new Font("Arial", 8), Brushes.Black, 0, 0);
                    }
                    Images[key] = bmp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải ảnh {fileName}: {ex.Message}");
            }
        }

        // Hàm lấy ảnh an toàn
        public static Image GetImage(string key)
        {
            if (Images.ContainsKey(key)) return Images[key];
            return null;
        }
    }
}