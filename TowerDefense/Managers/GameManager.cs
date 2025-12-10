using System.Collections.Generic;
using System.Drawing;
using TowerDefense.Core;
using TowerDefense.Entities;          // Chứa Projectile
using TowerDefense.Entities.Enemies;  // Chứa Enemy
using TowerDefense.Entities.Towers;   // Chứa Tower

namespace TowerDefense.Managers
{
    public class GameManager
    {
        // --- SINGLETON PATTERN ---
        private static GameManager _instance;
        public static GameManager Instance => _instance ?? (_instance = new GameManager());

        public SkillManager SkillMgr { get; private set; } = new SkillManager();

        // --- CÁC DANH SÁCH QUẢN LÝ THỰC THỂ ---
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<Tower> Towers { get; private set; } = new List<Tower>();
        public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

        // --- CÁC QUẢN LÝ CON (SUB-MANAGERS) ---
        public WaveManager WaveMgr { get; private set; } = new WaveManager();
        public LevelManager LevelMgr { get; private set; } = new LevelManager();

        // --- TRẠNG THÁI GAME ---
        public int PlayerMoney { get; set; }
        public int PlayerLives { get; set; }

        // Đường đi của màn chơi hiện tại
        public List<Point> CurrentMapPath { get; private set; }

        // Private Constructor
        private GameManager() { }

        // --- HÀM KHỞI TẠO GAME ---
        public void StartGame()
        {
            // 1. Xóa sạch dữ liệu cũ
            Enemies.Clear();
            Towers.Clear();
            Projectiles.Clear();

            // 2. Reset chỉ số người chơi
            PlayerMoney = 500; // Tiền khởi điểm
            PlayerLives = 20;  // Mạng khởi điểm

            // 3. Load Map (Ví dụ load Map 1)
            CurrentMapPath = LevelMgr.LoadLevelPath(1);

            // Lưu ý: Chúng ta KHÔNG gọi WaveMgr.StartNextWave() ở đây
            // để chờ người chơi bấm nút "START WAVE" trên giao diện.
        }

        // --- VÒNG LẶP LOGIC (UPDATE) ---
        public void Update(float deltaTime)
        {
            // 1. Update Wave Manager (Kiểm tra xem có cần sinh quái không)
            if (WaveMgr.Update(deltaTime))
            {
                SpawnEnemy();
            }

            // 2. Update Quái Vật (Duyệt ngược để xóa an toàn)
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                enemy.Update(deltaTime);

                // Kiểm tra điều kiện xóa:
                if (!enemy.IsActive)
                {
                    // Nếu quái biến mất nhưng vẫn còn máu -> Tức là đã đi lọt về đích
                    if (enemy.Health > 0)
                    {
                        PlayerLives--; // Trừ mạng người chơi
                    }
                    else
                    {
                        // Nếu quái chết do hết máu (bị bắn) -> Cộng tiền
                        PlayerMoney += enemy.RewardGold;
                    }

                    // Xóa khỏi list
                    Enemies.RemoveAt(i);
                }
            }

            // 3. Update Tháp
            foreach (var tower in Towers)
            {
                tower.Update(deltaTime);
            }

            // 4. Update Đạn (Duyệt ngược)
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                var proj = Projectiles[i];
                proj.Update(deltaTime);

                if (!proj.IsActive)
                {
                    Projectiles.RemoveAt(i);
                }
            }

            SkillMgr.Update(deltaTime); // Update hồi chiêu
        }

        // --- VÒNG LẶP VẼ (RENDER) ---
        public void Render(Graphics g)
        {
            // Vẽ theo thứ tự lớp: Tháp -> Quái -> Đạn
            foreach (var tower in Towers) tower.Render(g);
            foreach (var enemy in Enemies) enemy.Render(g);
            foreach (var proj in Projectiles) proj.Render(g);
        }

        // --- CÁC HÀM HỖ TRỢ ---

        // Sinh quái mới
        private void SpawnEnemy()
        {
            if (CurrentMapPath != null && CurrentMapPath.Count > 0)
            {
                Enemies.Add(new Enemy(CurrentMapPath));
            }
        }

        public int SelectedTowerType { get; set; } = 1;

        // --- CẬP NHẬT HÀM XÂY THÁP ---
        public bool TryBuildTower(float x, float y)
        {
            int cost = 0;
            Tower newTower = null;

            // Factory Pattern đơn giản
            switch (SelectedTowerType)
            {
                case 1: // Archer
                    cost = 100;
                    if (PlayerMoney >= cost) newTower = new ArcherTower(x, y);
                    break;
                case 2: // Cannon
                    cost = 200;
                    if (PlayerMoney >= cost) newTower = new CannonTower(x, y);
                    break;
            }

            if (newTower != null)
            {
                PlayerMoney -= cost;
                Towers.Add(newTower);
                return true; // Xây thành công
            }

            return false; // Không đủ tiền hoặc lỗi
        }
    }
}