using System.Collections.Generic;
using TowerDefense.Configs;

namespace TowerDefense.Managers
{
    public class WaveManager
    {
        public int CurrentWave { get; private set; } = 0;
        public bool IsWaveRunning { get; private set; } = false;

        private float _spawnTimer = 0;
        private float _spawnInterval = 1.5f; // 1.5 giây ra 1 con
        private int _enemiesLeftToSpawn = 0; // Số quái còn lại cần sinh trong wave này

        // Cấu hình Wave (sau này sẽ load từ JSON, giờ code cứng cho nhanh)
        public void StartNextWave()
        {
            CurrentWave++;
            IsWaveRunning = true;

            // Công thức: Wave càng cao quái càng đông
            // Wave 1: 5 con, Wave 2: 7 con...
            _enemiesLeftToSpawn = 5 + (CurrentWave * 2);

            _spawnTimer = 0;
        }

        // Hàm này được gọi liên tục trong GameLoop
        // Trả về true nếu đến lúc cần sinh quái
        public bool Update(float deltaTime)
        {
            if (!IsWaveRunning) return false;

            // Nếu đã sinh hết quái của wave này
            if (_enemiesLeftToSpawn <= 0)
            {
                IsWaveRunning = false;
                return false;
            }

            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0)
            {
                _spawnTimer = _spawnInterval; // Reset đồng hồ
                _enemiesLeftToSpawn--;
                return true; // Báo hiệu: "ĐẺ QUÁI ĐI!"
            }

            return false;
        }
    }
}