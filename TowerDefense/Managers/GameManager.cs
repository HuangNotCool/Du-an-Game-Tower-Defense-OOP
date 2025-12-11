using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefense.Core;
using TowerDefense.Entities;
using TowerDefense.Entities.Enemies;
using TowerDefense.Entities.Towers;
using TowerDefense.Configs;

namespace TowerDefense.Managers
{
    public class GameManager
    {
        // --- SINGLETON PATTERN ---
        private static GameManager _instance;
        public static GameManager Instance => _instance ?? (_instance = new GameManager());

        // --- DANH SÁCH QUẢN LÝ ---
        public List<Enemy> Enemies { get; private set; } = new List<Enemy>();
        public List<Tower> Towers { get; private set; } = new List<Tower>();
        public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();
        public List<FloatingText> Effects { get; private set; } = new List<FloatingText>();

        // --- CÁC MANAGER CON ---
        public WaveManager WaveMgr { get; private set; } = new WaveManager();
        public LevelManager LevelMgr { get; private set; } = new LevelManager();
        public SkillManager SkillMgr { get; private set; } = new SkillManager();

        // --- TRẠNG THÁI GAME ---
        public int PlayerMoney { get; set; }
        public int PlayerLives { get; set; }
        public int SelectedTowerType { get; set; } = 0;
        public float GameSpeed { get; set; } = 1.0f;

        public bool IsAutoWave { get; set; } = false;
        private float _autoWaveTimer = 0f;

        // --- BIẾN VICTORY (ĐÃ BỔ SUNG) ---
        public bool IsVictory { get; set; } = false;

        public List<Point> CurrentMapPath { get; private set; }

        private GameManager() { }

        // =========================================================
        // KHỞI TẠO GAME
        // =========================================================
        public void StartGame(int levelId = 1)
        {
            Enemies.Clear();
            Towers.Clear();
            Projectiles.Clear();
            Effects.Clear();

            PlayerMoney = 650;
            PlayerLives = 20;
            GameSpeed = 1.0f;
            IsAutoWave = false;
            _autoWaveTimer = 0f;

            // Reset cờ chiến thắng
            IsVictory = false;

            CurrentMapPath = LevelMgr.LoadLevelPath(levelId);
            WaveMgr = new WaveManager();
            SkillMgr = new SkillManager();
        }

        // =========================================================
        // GAME LOOP: UPDATE
        // =========================================================
        public void Update(float deltaTime)
        {
            float scaledDeltaTime = deltaTime * GameSpeed;

            SkillMgr.Update(scaledDeltaTime);
            HandleWaveLogic(scaledDeltaTime);

            // Update Quái
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                var enemy = Enemies[i];
                enemy.Update(scaledDeltaTime);

                if (!enemy.IsActive)
                {
                    if (enemy.Health > 0)
                    {
                        PlayerLives--;
                        ShowFloatingText("-1 ❤", enemy.X, enemy.Y - 20, Color.Red);
                    }
                    else
                    {
                        PlayerMoney += enemy.RewardGold;
                        ShowFloatingText($"+{enemy.RewardGold}G", enemy.X, enemy.Y - 20, Color.Gold);
                    }
                    Enemies.RemoveAt(i);
                }
            }

            // Update Tháp
            for (int i = Towers.Count - 1; i >= 0; i--)
            {
                Towers[i].Update(scaledDeltaTime);
                if (!Towers[i].IsActive) Towers.RemoveAt(i);
            }

            // Update Đạn
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Update(scaledDeltaTime);
                if (!Projectiles[i].IsActive) Projectiles.RemoveAt(i);
            }

            // Update Effect
            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                Effects[i].Update(scaledDeltaTime);
                if (!Effects[i].IsActive) Effects.RemoveAt(i);
            }
        }

        // --- XỬ LÝ LOGIC WAVE & VICTORY ---
        private void HandleWaveLogic(float dt)
        {
            // A. Wave đang chạy
            if (WaveMgr.IsWaveRunning)
            {
                int enemyId = WaveMgr.Update(dt);
                if (enemyId != -1) SpawnEnemy(enemyId);
            }
            // B. Wave dừng & Hết quái -> Check Thắng hoặc Nghỉ
            else if (Enemies.Count == 0)
            {
                // --- KIỂM TRA ĐIỀU KIỆN THẮNG (MỚI) ---
                // Nếu Wave hiện tại >= Tổng số Wave của Map -> THẮNG
                if (WaveMgr.CurrentWave >= LevelMgr.MaxWaves)
                {
                    IsVictory = true;
                    return;
                }

                // Logic Auto Wave (Nghỉ giữa hiệp)
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
                else
                {
                    _autoWaveTimer = 0;
                }
            }
        }

        // =========================================================
        // GAME LOOP: RENDER
        // =========================================================
        public void Render(Graphics g)
        {
            foreach (var tower in Towers) tower.Render(g);
            foreach (var enemy in Enemies) enemy.Render(g);
            foreach (var proj in Projectiles) proj.Render(g);
            foreach (var effect in Effects) effect.Render(g);
        }

        // =========================================================
        // HELPER
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

        public bool TryBuildTower(float x, float y)
        {
            if (SelectedTowerType < 0 || SelectedTowerType >= GameConfig.Towers.Length) return false;

            var stat = GameConfig.Towers[SelectedTowerType];
            int cost = stat.Price;

            if (PlayerMoney >= cost)
            {
                PlayerMoney -= cost;
                Tower newTower = new Tower(x, y, SelectedTowerType);
                Towers.Add(newTower);
                return true;
            }
            return false;
        }

        public bool CanPlaceTower(float x, float y)
        {
            foreach (var tower in Towers)
            {
                if (Math.Abs(tower.X - x) < 35 && Math.Abs(tower.Y - y) < 35) return false;
            }
            if (IsOnPath(x, y)) return false;
            return true;
        }

        private bool IsOnPath(float x, float y)
        {
            if (CurrentMapPath == null || CurrentMapPath.Count < 2) return false;
            float safeDistance = 40f;

            for (int i = 0; i < CurrentMapPath.Count - 1; i++)
            {
                Point p1 = CurrentMapPath[i];
                Point p2 = CurrentMapPath[i + 1];
                float dist = GetDistanceToSegment(new PointF(x, y), p1, p2);
                if (dist < safeDistance) return true;
            }
            return false;
        }

        private float GetDistanceToSegment(PointF pt, PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0)) return (float)Math.Sqrt(Math.Pow(pt.X - p1.X, 2) + Math.Pow(pt.Y - p1.Y, 2));

            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            if (t < 0) return (float)Math.Sqrt(Math.Pow(pt.X - p1.X, 2) + Math.Pow(pt.Y - p1.Y, 2));
            else if (t > 1) return (float)Math.Sqrt(Math.Pow(pt.X - p2.X, 2) + Math.Pow(pt.Y - p2.Y, 2));

            PointF closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
            return (float)Math.Sqrt(Math.Pow(pt.X - closest.X, 2) + Math.Pow(pt.Y - closest.Y, 2));
        }
    }
}