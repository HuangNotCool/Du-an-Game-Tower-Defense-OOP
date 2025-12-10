using System.IO;
using Newtonsoft.Json; // Cần thư viện vừa cài
using TowerDefense.Managers;
using TowerDefense.Entities.Towers;

namespace TowerDefense.Data
{
    public static class SaveLoadSystem
    {
        private static string _filePath = "savegame.json"; // File nằm cùng chỗ với file .exe

        public static void SaveGame()
        {
            // 1. Gom dữ liệu từ GameManager vào SaveData
            var data = new GameSaveData
            {
                Money = GameManager.Instance.PlayerMoney,
                Lives = GameManager.Instance.PlayerLives,
                CurrentWave = GameManager.Instance.WaveMgr.CurrentWave,
                LevelId = 1 // Tạm thời hardcode map 1
            };

            // 2. Lưu danh sách tháp
            foreach (var tower in GameManager.Instance.Towers)
            {
                // Xác định loại tháp dựa trên tên class hoặc thuộc tính Name
                string type = (tower is ArcherTower) ? "Archer" : "Cannon";

                data.Towers.Add(new TowerData
                {
                    Type = type,
                    X = tower.X,
                    Y = tower.Y
                });
            }

            // 3. Ghi ra file JSON
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static void LoadGame()
        {
            if (!File.Exists(_filePath)) return;

            // 1. Đọc file
            string json = File.ReadAllText(_filePath);
            var data = JsonConvert.DeserializeObject<GameSaveData>(json);

            if (data == null) return;

            // 2. Khôi phục dữ liệu vào GameManager
            GameManager.Instance.StartGame(); // Reset game trước

            GameManager.Instance.PlayerMoney = data.Money;
            GameManager.Instance.PlayerLives = data.Lives;

            // Khôi phục Wave (Hack nhẹ vào WaveManager)
            // Bạn cần thêm hàm SetWave vào WaveManager nếu muốn chuẩn, 
            // hoặc tạm thời gán thủ công nếu biến là public.
            // Ở đây ta giả sử đã Load lại đúng Wave.

            // 3. Khôi phục Tháp
            foreach (var tData in data.Towers)
            {
                if (tData.Type == "Archer")
                {
                    GameManager.Instance.Towers.Add(new ArcherTower(tData.X, tData.Y));
                }
                else if (tData.Type == "Cannon")
                {
                    GameManager.Instance.Towers.Add(new CannonTower(tData.X, tData.Y));
                }
            }
        }
    }
}