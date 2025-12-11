using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefense.Core;
using TowerDefense.Entities;          // Chứa Projectile, FloatingText, Particle
using TowerDefense.Entities.Enemies;  // Chứa Enemy
using TowerDefense.Entities.Towers;   // Chứa Tower
using TowerDefense.Configs;           // Chứa GameConfig

namespace TowerDefense.Managers
{
    public class GameManager
    {
        // =========================================================
        // 1. SINGLETON & DANH SÁCH QUẢN LÝ
        // =========================================================
        private static GameManager _instance;
        public static GameManager Instance => _instance ?? (_instance = new GameManager());

        // Danh sách thực thể
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<Tower> Towers { get; private set; } = new List<Tower>();
        public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();
        public List<FloatingText> Effects { get; private set; } = new List<FloatingText>(); // Chữ bay
        public List<Particle> Particles { get; private set; } = new List<Particle>();       // Hiệu ứng nổ

        // Các Manager con
        public WaveManager WaveMgr { get; private set; } = new WaveManager();
        public LevelManager LevelMgr { get; private set; } = new LevelManager();
        public SkillManager SkillMgr { get; private set; } = new SkillManager();

        // =========================================================
        // 2. TRẠNG THÁI GAME (GAME STATE)
        // =========================================================
        public int PlayerMoney { get; set; }
        public int PlayerLives { get; set; }

        // ID tháp đang chọn (-1 là không chọn)
        public int SelectedTowerType { get; set; } = -1;

        // Tốc độ game (1.0 = thường, 2.0 = nhanh)
        public float GameSpeed { get; set; } = 1.0f;

        // Chế độ Auto Wave
        public bool IsAutoWave { get; set; } = false;
        private float _autoWaveTimer = 0f;

        // Cờ chiến thắng
        public bool IsVictory { get; set; } = false;

        // Bản đồ
        public List<Point> CurrentMapPath { get; private set; }

        // Random cho hiệu ứng
        private Random _rnd = new Random();

        private GameManager() { }

        // =========================================================
        // 3. KHỞI TẠO GAME
        // =========================================================
        public void StartGame(int levelId = 1)
        {
            // Reset toàn bộ list
            Enemies.Clear();
            Towers.Clear();
            Projectiles.Clear();
            Effects.Clear();
            Particles.Clear();

            // Reset chỉ số
            PlayerMoney = 650;
            PlayerLives = 20;
            GameSpeed = 1.0f;
            IsAutoWave = false;
            _autoWaveTimer = 0f;
            IsVictory = false;
            SelectedTowerType = -1;

            // Load Map & Reset Manager
            CurrentMapPath = LevelMgr.LoadLevelPath(levelId);
            WaveMgr = new WaveManager();
            SkillMgr = new SkillManager();
        }

        // =========================================================
        // 4. GAME LOOP: UPDATE (LOGIC)
        // =========================================================
        public void Update(float deltaTime)
        {
            // Áp dụng tốc độ game (x2 Speed)
            float scaledDeltaTime = deltaTime * GameSpeed;

            // A. Update Manager con
            SkillMgr.Update(scaledDeltaTime);
            HandleWaveLogic(scaledDeltaTime);

            // B. Update Quái (Duyệt ngược để xóa an toàn)
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                enemy.Update(scaledDeltaTime);

                if (!enemy.IsActive)
                {
                    if (enemy.Health > 0) // Về đích
                    {
                        PlayerLives--;
                        ShowFloatingText("-1 ❤", enemy.X, enemy.Y - 20, Color.Red);
                    }
                    else // Bị giết
                    {
                        PlayerMoney += enemy.RewardGold;
                        ShowFloatingText($"+{enemy.RewardGold}G", enemy.X, enemy.Y - 20, Color.Gold);
                    }
                    Enemies.RemoveAt(i);
                }
            }

            // C. Update Tháp
            for (int i = Towers.Count - 1; i >= 0; i--)
            {
                Towers[i].Update(scaledDeltaTime);
                if (!Towers[i].IsActive) Towers.RemoveAt(i); // Xóa tháp nếu bị phá hủy
            }

            // D. Update Đạn
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Update(scaledDeltaTime);
                if (!Projectiles[i].IsActive) Projectiles.RemoveAt(i);
            }

            // E. Update Chữ bay (Effects)
            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                Effects[i].Update(scaledDeltaTime);
                if (!Effects[i].IsActive) Effects.RemoveAt(i);
            }

            // F. Update Hạt nổ (Particles)
            for (int i = Particles.Count - 1; i >= 0; i--)
            {
                Particles[i].Update(scaledDeltaTime);
                if (!Particles[i].IsActive) Particles.RemoveAt(i);
            }
            // Giới hạn số lượng hạt để tránh lag
            if (Particles.Count > 200) Particles.RemoveRange(0, 50);
        }

        private void HandleWaveLogic(float dt)
        {
            // 1. Wave đang chạy
            if (WaveMgr.IsWaveRunning)
            {
                int enemyId = WaveMgr.Update(dt);
                if (enemyId != -1) SpawnEnemy(enemyId);
            }
            // 2. Hết Wave & Hết quái -> Check Thắng hoặc Nghỉ
            else if (Enemies.Count == 0)
            {
                if (WaveMgr.CurrentWave >= LevelMgr.MaxWaves)
                {
                    IsVictory = true;
                    return;
                }

                if (IsAutoWave)
                {
                    _autoWaveTimer += dt;
                    if (_autoWaveTimer >= 3.0f)
                    {
                        WaveMgr.StartNextWave();
                        _autoWaveTimer = 0;
                        SoundManager.Play("win");
                    }
                }
                else _autoWaveTimer = 0;
            }
        }

        // =========================================================
        // 5. GAME LOOP: RENDER (VẼ)
        // =========================================================
        public void Render(Graphics g)
        {
            // Vẽ theo lớp: Tháp -> Quái -> Đạn -> Hạt -> Chữ
            foreach (var tower in Towers) tower.Render(g);
            foreach (var enemy in Enemies) enemy.Render(g);
            foreach (var proj in Projectiles) proj.Render(g);
            foreach (var p in Particles) p.Render(g);
            foreach (var effect in Effects) effect.Render(g);
        }

        // =========================================================
        // 6. CÁC HÀM HỖ TRỢ & FACTORY
        // =========================================================

        private void SpawnEnemy(int typeId)
        {
            if (CurrentMapPath != null && CurrentMapPath.Count > 0)
            {
                Enemies.Add(new Enemy(CurrentMapPath, typeId));
            }
        }

        public void ShowFloatingText(string text, float x, float y, Color color)
        {
            Effects.Add(new FloatingText(text, x, y, color));
        }

        // --- HỆ THỐNG PARTICLE (HIỆU ỨNG) ---
        public void CreateExplosion(float x, float y, Color color)
        {
            int count = 15;
            for (int i = 0; i < count; i++)
            {
                float angle = (float)(_rnd.NextDouble() * Math.PI * 2);
                float speed = _rnd.Next(50, 200);
                float size = _rnd.Next(4, 12);
                float life = (float)(_rnd.NextDouble() * 0.5 + 0.2);
                Particles.Add(new Particle(x, y, color, size, speed, angle, life));
            }
        }

        public void CreateHitEffect(float x, float y)
        {
            for (int i = 0; i < 5; i++)
            {
                float angle = (float)(_rnd.NextDouble() * Math.PI * 2);
                Particles.Add(new Particle(x, y, Color.WhiteSmoke, 4, 100, angle, 0.3f));
            }
        }

        public void CreateIceEffect(float x, float y)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = (float)(_rnd.NextDouble() * Math.PI * 2);
                Particles.Add(new Particle(x, y, Color.Cyan, 5, 80, angle, 0.5f));
            }
        }

        // =========================================================
        // 7. LOGIC XÂY DỰNG (GRID SYSTEM)
        // =========================================================

        public bool TryBuildTower(float x, float y)
        {
            if (SelectedTowerType < 0 || SelectedTowerType >= GameConfig.Towers.Length) return false;

            var stat = GameConfig.Towers[SelectedTowerType];

            if (PlayerMoney >= stat.Price)
            {
                PlayerMoney -= stat.Price;
                Towers.Add(new Tower(x, y, SelectedTowerType));
                return true;
            }
            return false;
        }

        public bool CanPlaceTower(float x, float y)
        {
            // Check trùng tháp
            foreach (var tower in Towers)
            {
                if (Math.Abs(tower.X - x) < 35 && Math.Abs(tower.Y - y) < 35) return false;
            }
            // Check đè đường đi
            if (IsOnPath(x, y)) return false;

            return true;
        }

        // --- LOGIC CHECK VA CHẠM ĐƯỜNG ĐI (ĐÃ SỬA LỖI GRID) ---
        // --- LOGIC CHECK VA CHẠM ĐƯỜNG ĐI (ĐÃ TỐI ƯU HITBOX) ---
        private bool IsOnPath(float x, float y)
        {
            if (CurrentMapPath == null || CurrentMapPath.Count < 2) return false;

            // TWEAK 1: Thu nhỏ vùng va chạm của Tháp (Tower Hitbox)
            // Thay vì dùng kích thước thật (36px), ta chỉ dùng vùng tâm (10px)
            // Nghĩa là: Chỉ cần cái "chân đế" ở giữa không chạm đường là được phép xây
            float coreSize = 10f;
            RectangleF towerRect = new RectangleF(x - coreSize / 2, y - coreSize / 2, coreSize, coreSize);

            // TWEAK 2: Giảm nhẹ bán kính đường đi (Path Hitbox)
            // Hình vẽ là 60px (Radius 30), nhưng Logic ta tính là 30px (Radius 15)
            // Để tạo cảm giác "thoáng" hơn, cho phép tháp lấn nhẹ vào lề đường
            float pathRadius = 28f;

            for (int i = 0; i < CurrentMapPath.Count - 1; i++)
            {
                Point p1 = CurrentMapPath[i];
                Point p2 = CurrentMapPath[i + 1];

                // Tạo hình chữ nhật bao quanh đoạn đường
                float left = Math.Min(p1.X, p2.X) - pathRadius;
                float right = Math.Max(p1.X, p2.X) + pathRadius;
                float top = Math.Min(p1.Y, p2.Y) - pathRadius;
                float bottom = Math.Max(p1.Y, p2.Y) + pathRadius;

                RectangleF pathRect = new RectangleF(left, top, right - left, bottom - top);

                // Kiểm tra giao nhau
                if (towerRect.IntersectsWith(pathRect))
                {
                    return true; // Chặn
                }
            }
            return false; // Cho phép xây
        }

        // (Bạn có thể xóa hàm GetDistanceToSegment cũ đi vì không dùng nữa)
    }
}