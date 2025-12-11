using System;
using TowerDefense.Configs; // Để biết tổng số loại quái

namespace TowerDefense.Managers
{
    public class WaveManager
    {
        // --- TRẠNG THÁI WAVE ---
        public int CurrentWave { get; private set; } = 0;
        public bool IsWaveRunning { get; private set; } = false;

        // --- BIẾN LOGIC ---
        private float _spawnTimer = 0;       // Đếm ngược thời gian sinh quái
        private float _spawnInterval = 1.0f; // Khoảng cách thời gian giữa 2 con quái
        private int _enemiesLeftToSpawn = 0; // Số lượng quái còn lại cần sinh trong wave này

        // Random để chọn loại quái ngẫu nhiên trong dải cho phép
        private Random _random = new Random();

        // =========================================================
        // LOGIC KHỞI ĐỘNG WAVE MỚI
        // =========================================================
        public void StartNextWave()
        {
            CurrentWave++;
            IsWaveRunning = true;

            // 1. Tính số lượng quái (Tăng dần theo Wave)
            // Công thức: 5 con cơ bản + (Wave * 2)
            // VD: Wave 1 = 7 con, Wave 10 = 25 con
            _enemiesLeftToSpawn = 5 + (CurrentWave * 2);

            // 2. Tính tốc độ ra quái (Càng về sau ra càng nhanh)
            // Giảm 0.05s mỗi wave, tối thiểu là 0.2s/con
            _spawnInterval = Math.Max(0.2f, 1.5f - (CurrentWave * 0.05f));

            _spawnTimer = 0; // Reset timer để con đầu tiên ra ngay lập tức
        }

        // =========================================================
        // UPDATE LOOP (GỌI TỪ GAMEMANAGER)
        // =========================================================
        // Trả về: ID của loại quái cần sinh (-1 nếu không sinh)
        public int Update(float deltaTime)
        {
            if (!IsWaveRunning) return -1;

            // Nếu đã sinh hết quái -> Dừng trạng thái sinh
            if (_enemiesLeftToSpawn <= 0)
            {
                IsWaveRunning = false;
                return -1;
            }

            // Đếm ngược
            _spawnTimer -= deltaTime;

            if (_spawnTimer <= 0)
            {
                // Reset đồng hồ
                _spawnTimer = _spawnInterval;
                _enemiesLeftToSpawn--;

                // Trả về ID quái để GameManager tạo
                return GetEnemyTypeForCurrentWave();
            }

            return -1;
        }

        // =========================================================
        // THUẬT TOÁN CHỌN QUÁI (QUAN TRỌNG)
        // =========================================================
        private int GetEnemyTypeForCurrentWave()
        {
            int totalEnemyTypes = GameConfig.Enemies.Length; // 20 loại

            // --- LOGIC: CỬA SỔ TRƯỢT (SLIDING WINDOW) ---
            // Wave 1: Random từ 0-2 (Slime, Rat, Bat)
            // Wave 5: Random từ 2-5 (Bat, Goblin, Skeleton)
            // ...
            // Wave cao sẽ mở khóa quái mới và bỏ qua quái quá yếu

            // Tính chỉ số bắt đầu (Min)
            int minType = (CurrentWave - 1) / 2;

            // Tính chỉ số kết thúc (Max) - Mở rộng đa dạng 3-4 loại mỗi wave
            int maxType = minType + 3;

            // --- LOGIC BOSS WAVE (Mỗi 5 Wave) ---
            if (CurrentWave % 5 == 0)
            {
                // Wave 5, 10, 15... sẽ spawn quái mạnh nhất trong tier hiện tại
                minType = maxType;
            }

            // --- KẸP GIÁ TRỊ (CLAMP) ---
            // Không được nhỏ hơn 0
            if (minType < 0) minType = 0;

            // Không được vượt quá danh sách quái có trong Config
            if (maxType >= totalEnemyTypes) maxType = totalEnemyTypes - 1;
            if (minType > maxType) minType = maxType;

            // Random ra ID cuối cùng
            return _random.Next(minType, maxType + 1);
        }
    }
}