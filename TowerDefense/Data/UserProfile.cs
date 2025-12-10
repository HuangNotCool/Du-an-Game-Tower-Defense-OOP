using System.IO;
using Newtonsoft.Json;

namespace TowerDefense.Data
{
    public class UserProfile
    {
        // --- DỮ LIỆU VĨNH VIỄN ---
        public int Diamonds { get; set; } = 100; // Cho sẵn 100 kim cương để test

        // Cấp độ nâng cấp (Mặc định lv 0)
        public int ArcherDamageLevel { get; set; } = 0;
        public int ArcherRangeLevel { get; set; } = 0;
        public int CannonDamageLevel { get; set; } = 0;

        // --- QUẢN LÝ LƯU / TẢI ---
        private static string _filePath = "userprofile.json";
        private static UserProfile _instance;

        public static UserProfile Instance
        {
            get
            {
                if (_instance == null) Load();
                return _instance;
            }
        }

        public static void Load()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _instance = JsonConvert.DeserializeObject<UserProfile>(json);
            }
            else
            {
                _instance = new UserProfile(); // Tạo mới nếu chưa có
                Save();
            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}