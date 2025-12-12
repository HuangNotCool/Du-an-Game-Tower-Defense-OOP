using System.Collections.Generic;
using System.Drawing;

namespace TowerDefense.Configs
{
    // =========================================================
    // 1. CÁC HẰNG SỐ HỆ THỐNG
    // =========================================================
    public static class GameConstants
    {
        public const int TILE_SIZE = 40;      // Kích thước ô lưới chuẩn
        public const int HALF_TILE = 20;      // Tâm ô lưới (40/2)
        public const float DEFAULT_GAME_SPEED = 1.0f;
    }

    // =========================================================
    // 2. CẤU TRÚC DỮ LIỆU (STRUCTS)
    // =========================================================

    // Cấu trúc dữ liệu cho Tháp
    public struct TowerStat
    {
        public string Name;
        public int Price;
        public int Damage;
        public float Range;
        public float ReloadTime;
        public float Cooldown; // <--- ĐÃ THÊM THUỘC TÍNH NÀY
        public int MaxHealth;         // Máu của tháp (để quái đánh)
        public string ProjectileType; // "Arrow", "Bomb", "Ice", "Magic", "Fire"...
        public Color Color;           // Màu đại diện (nếu chưa có ảnh)
    }

    // Cấu trúc dữ liệu cho Quái
    public struct EnemyStat
    {
        public string Name;
        public int MaxHealth;
        public float Speed;
        public int Reward;
        public int DamageToBase;  // Sát thương lên nhà chính
        public int DamageToTower; // Sát thương lên Tháp (Mới)
        public float AttackRange; // Tầm đánh tháp (Mới)
        public bool CanFly;       // Quái bay (bỏ qua đường đi - nâng cao)
        public Color Color;
    }

    // =========================================================
    // 3. DỮ LIỆU CẤU HÌNH (DATABASE)
    // =========================================================
    public static class GameConfig
    {
        // --- DANH SÁCH 10 LOẠI THÁP ---
        public static readonly TowerStat[] Towers = new TowerStat[]
        {
            // ID 0: Archer (Cơ bản - Rẻ, nhanh, yếu)
            new TowerStat { Name="Archer", Price=100, Damage=20, Range=200, ReloadTime=0.8f, MaxHealth=100, ProjectileType="Arrow", Color=Color.Blue },
            
            // ID 1: Cannon (Pháo - Đắt, chậm, nổ lan)
            new TowerStat { Name="Cannon", Price=250, Damage=50, Range=150, ReloadTime=2.0f, MaxHealth=200, ProjectileType="Bomb", Color=Color.Black },
            
            // ID 2: Sniper (Bắn tỉa - Rất xa, rất đau, rất chậm)
            new TowerStat { Name="Sniper", Price=400, Damage=150, Range=400, ReloadTime=3.0f, MaxHealth=50, ProjectileType="Arrow", Color=Color.ForestGreen },
            
            // ID 3: Minigun (Súng máy - Gần, cực nhanh, damage bé)
            new TowerStat { Name="Minigun", Price=500, Damage=8, Range=120, ReloadTime=0.1f, MaxHealth=150, ProjectileType="Arrow", Color=Color.Gray },
            
            // ID 4: Ice (Băng - Làm chậm)
            new TowerStat { Name="Ice", Price=300, Damage=15, Range=180, ReloadTime=1.0f, MaxHealth=100, ProjectileType="Ice", Color=Color.Cyan },
            
            // ID 5: Magic (Phép - Xuyên giáp)
            new TowerStat { Name="Magic", Price=600, Damage=80, Range=220, ReloadTime=1.5f, MaxHealth=80, ProjectileType="Magic", Color=Color.Purple },
            
            // ID 6: Bunker (Chống chịu - Máu cực trâu để chặn quái)
            new TowerStat { Name="Bunker", Price=150, Damage=10, Range=100, ReloadTime=1.0f, MaxHealth=1000, ProjectileType="Arrow", Color=Color.DarkSlateGray },
            
            // ID 7: Fire (Lửa - Thiêu đốt)
            new TowerStat { Name="Fire", Price=450, Damage=40, Range=160, ReloadTime=1.2f, MaxHealth=120, ProjectileType="Fire", Color=Color.OrangeRed },
            
            // ID 8: Rocket (Tên lửa - Tầm xa, nổ to)
            new TowerStat { Name="Rocket", Price=800, Damage=100, Range=300, ReloadTime=2.5f, MaxHealth=150, ProjectileType="Bomb", Color=Color.DarkRed },
            
            // ID 9: God (Thần - Siêu cấp vô địch)
            new TowerStat { Name="God", Price=5000, Damage=500, Range=500, ReloadTime=0.5f, MaxHealth=500, ProjectileType="Magic", Color=Color.Gold },
        };

        // --- DANH SÁCH 20 LOẠI QUÁI ---
        // --- DANH SÁCH 20 LOẠI QUÁI (ĐÃ BUFF SỨC MẠNH) ---
        public static readonly EnemyStat[] Enemies = new EnemyStat[]
        {
    // --- TIER 1: QUÁI YẾU (Wave 1-4) ---
    new EnemyStat { Name="Slime", MaxHealth=50, Speed=80, Reward=5, DamageToTower=0, AttackRange=0, Color=Color.Green }, // HP 30 -> 50
    new EnemyStat { Name="Rat", MaxHealth=40, Speed=130, Reward=5, DamageToTower=0, AttackRange=0, Color=Color.Gray },
    new EnemyStat { Name="Bat", MaxHealth=45, Speed=160, Reward=8, DamageToTower=0, AttackRange=0, CanFly=true, Color=Color.Black },
    new EnemyStat { Name="Goblin", MaxHealth=150, Speed=90, Reward=15, DamageToTower=10, AttackRange=30, Color=Color.DarkGreen }, // HP 60 -> 150
    new EnemyStat { Name="Skeleton", MaxHealth=200, Speed=70, Reward=18, DamageToTower=20, AttackRange=30, Color=Color.White },

    // --- TIER 2: QUÁI TRUNG BÌNH (Wave 5-9) ---
    new EnemyStat { Name="Orc", MaxHealth=400, Speed=60, Reward=30, DamageToTower=40, AttackRange=40, Color=Color.DarkOliveGreen }, // HP 150 -> 400
    new EnemyStat { Name="Wolf", MaxHealth=250, Speed=140, Reward=35, DamageToTower=25, AttackRange=30, Color=Color.Gray },
    new EnemyStat { Name="Spider", MaxHealth=300, Speed=110, Reward=32, DamageToTower=20, AttackRange=30, Color=Color.Brown },
    new EnemyStat { Name="Ghost", MaxHealth=350, Speed=60, Reward=40, DamageToTower=0, AttackRange=0, Color=Color.LightBlue },
    new EnemyStat { Name="Witch", MaxHealth=250, Speed=90, Reward=50, DamageToTower=50, AttackRange=150, Color=Color.Purple },

    // --- TIER 3: QUÁI MẠNH (Wave 10-14) ---
    new EnemyStat { Name="Troll", MaxHealth=1500, Speed=45, Reward=80, DamageToTower=100, AttackRange=50, Color=Color.DarkCyan }, // Tanker hồi máu
    new EnemyStat { Name="Gargoyle", MaxHealth=800, Speed=120, Reward=90, DamageToTower=60, AttackRange=40, CanFly=true, Color=Color.DarkSlateBlue },
    new EnemyStat { Name="Vampire", MaxHealth=1000, Speed=130, Reward=100, DamageToTower=60, AttackRange=40, Color=Color.Red },
    new EnemyStat { Name="Golem", MaxHealth=3000, Speed=35, Reward=150, DamageToTower=200, AttackRange=40, Color=Color.SandyBrown }, // Kẻ hủy diệt tháp
    new EnemyStat { Name="Assassin", MaxHealth=600, Speed=220, Reward=120, DamageToTower=80, AttackRange=30, Color=Color.Black }, // Chạy cực nhanh

    // --- TIER 4: BOSS & SIÊU QUÁI (Wave 15+) ---
    // Cyclops (Mini Boss)
    new EnemyStat { Name="Cyclops", MaxHealth=8000, Speed=55, Reward=300, DamageToTower=300, AttackRange=80, Color=Color.DarkOrange }, 
    // Hydra (Hồi máu)
    new EnemyStat { Name="Hydra", MaxHealth=12000, Speed=45, Reward=500, DamageToTower=200, AttackRange=120, Color=Color.DarkGreen }, 
    // Phoenix (Bay & Nhanh)
    new EnemyStat { Name="Phoenix", MaxHealth=6000, Speed=180, Reward=600, DamageToTower=150, AttackRange=150, CanFly=true, Color=Color.OrangeRed }, 
    // Titan (One-hit Tháp)
    new EnemyStat { Name="Titan", MaxHealth=20000, Speed=25, Reward=1000, DamageToTower=1000, AttackRange=80, Color=Color.DarkBlue }, 
    // FINAL BOSS
    new EnemyStat { Name="DEMON KING", MaxHealth=50000, Speed=70, Reward=5000, DamageToTower=500, AttackRange=250, Color=Color.Crimson },
        };
    }
}