using System.IO;
using System.Linq; // Cần dùng Linq để tìm ID tháp
using Newtonsoft.Json;
using TowerDefense.Managers;
using TowerDefense.Entities.Towers;
using TowerDefense.Configs; // Để tra cứu GameConfig

namespace TowerDefense.Data
{
    public static class SaveLoadSystem
    {
        private static string _filePath = "savegame.json";

        public static void SaveGame()
        {
            var data = new GameSaveData
            {
                Money = GameManager.Instance.PlayerMoney,
                Lives = GameManager.Instance.PlayerLives,
                CurrentWave = GameManager.Instance.WaveMgr.CurrentWave,

                // --- SỬA DÒNG NÀY: Lấy Level hiện tại từ LevelManager ---
                LevelId = GameManager.Instance.LevelMgr.CurrentLevelId
            };

            foreach (var tower in GameManager.Instance.Towers)
            {
                // SỬA: Lấy trực tiếp Name của tháp ("Archer", "Cannon"...)
                data.Towers.Add(new TowerData
                {
                    Type = tower.Name,
                    X = tower.X,
                    Y = tower.Y
                });
            }

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public static void LoadGame()
        {
            if (!File.Exists(_filePath)) return;

            string json = File.ReadAllText(_filePath);
            var data = JsonConvert.DeserializeObject<GameSaveData>(json);
            if (data == null) return;

            // Reset Game
            GameManager.Instance.StartGame(data.LevelId);

            // Khôi phục chỉ số
            GameManager.Instance.PlayerMoney = data.Money;
            GameManager.Instance.PlayerLives = data.Lives;

            // Khôi phục Wave (Cần hack nhẹ biến CurrentWave trong WaveManager nếu nó là private set)
            // Hoặc tạo hàm SetWave trong WaveManager. 
            // Tạm thời ta bỏ qua việc set lại Wave chính xác để tránh lỗi access modifier, 
            // hoặc bạn cần sửa WaveManager: public int CurrentWave { get; set; }

            // Khôi phục Tháp
            foreach (var tData in data.Towers)
            {
                // Tìm ID tháp dựa trên Tên (Type)
                int typeId = 0; // Mặc định là 0 (Archer)

                for (int i = 0; i < GameConfig.Towers.Length; i++)
                {
                    if (GameConfig.Towers[i].Name == tData.Type)
                    {
                        typeId = i;
                        break;
                    }
                }

                // Tạo tháp mới bằng ID (Constructor chuẩn Data-Driven)
                var newTower = new Tower(tData.X, tData.Y, typeId);
                GameManager.Instance.Towers.Add(newTower);
            }
        }
    }
}