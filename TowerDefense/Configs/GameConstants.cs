using System.Collections.Generic;
using System.Drawing;

namespace TowerDefense.Configs
{
    // Cấu trúc dữ liệu cho Tháp
    public struct TowerStat
    {
        public string Name;
        public int Price;
        public int Damage;
        public float Range;
        public float ReloadTime;
        public int MaxHealth; // Máu của tháp (để quái đánh)
        public string ProjectileType; // "Arrow", "Bomb", "Laser", "Magic"...
        public Color Color; // Màu đại diện (nếu chưa có ảnh)
    }

    // Cấu trúc dữ liệu cho Quái
    public struct EnemyStat
    {
        public string Name;
        public int MaxHealth;
        public float Speed;
        public int Reward;
        public int DamageToBase; // Sát thương lên nhà chính
        public int DamageToTower; // Sát thương lên Tháp (Mới)
        public float AttackRange; // Tầm đánh tháp (Mới)
        public bool CanFly;       // Quái bay (bỏ qua đường đi - nâng cao)
        public Color Color;
    }

    public static class GameConfig
    {
        // --- DANH SÁCH 10 LOẠI THÁP ---
        public static readonly TowerStat[] Towers = new TowerStat[]
        {
            // 0. Archer (Cơ bản)
            new TowerStat { Name="Archer", Price=100, Damage=20, Range=200, ReloadTime=0.8f, MaxHealth=100, ProjectileType="Arrow", Color=Color.Blue },
            // 1. Cannon (Nổ lan)
            new TowerStat { Name="Cannon", Price=250, Damage=50, Range=150, ReloadTime=2.0f, MaxHealth=200, ProjectileType="Bomb", Color=Color.Black },
            // 2. Sniper (Bắn tỉa - Xa, Đau, Chậm)
            new TowerStat { Name="Sniper", Price=400, Damage=150, Range=400, ReloadTime=3.0f, MaxHealth=50, ProjectileType="Arrow", Color=Color.Green },
            // 3. Minigun (Xả đạn - Gần, Yếu, Cực nhanh)
            new TowerStat { Name="Minigun", Price=500, Damage=8, Range=120, ReloadTime=0.1f, MaxHealth=150, ProjectileType="Arrow", Color=Color.Gray },
            // 4. Ice (Làm chậm)
            new TowerStat { Name="Ice", Price=300, Damage=15, Range=180, ReloadTime=1.0f, MaxHealth=100, ProjectileType="Ice", Color=Color.Cyan },
            // 5. Magic (Sát thương phép - Xuyên giáp)
            new TowerStat { Name="Magic", Price=600, Damage=80, Range=220, ReloadTime=1.5f, MaxHealth=80, ProjectileType="Magic", Color=Color.Purple },
            // 6. Tanker (Tháp máu trâu - Để chặn quái đánh)
            new TowerStat { Name="Bunker", Price=150, Damage=10, Range=100, ReloadTime=1.0f, MaxHealth=1000, ProjectileType="Arrow", Color=Color.DarkSlateGray },
            // 7. Fire (Thiêu đốt)
            new TowerStat { Name="Fire", Price=450, Damage=40, Range=160, ReloadTime=1.2f, MaxHealth=120, ProjectileType="Fire", Color=Color.OrangeRed },
            // 8. Rocket (Tên lửa - Tầm xa, Nổ to)
            new TowerStat { Name="Rocket", Price=800, Damage=100, Range=300, ReloadTime=2.5f, MaxHealth=150, ProjectileType="Bomb", Color=Color.DarkRed },
            // 9. God (Siêu cấp)
            new TowerStat { Name="God", Price=5000, Damage=500, Range=500, ReloadTime=0.5f, MaxHealth=500, ProjectileType="Magic", Color=Color.Gold },
        };

        // --- DANH SÁCH 20 LOẠI QUÁI ---
        public static readonly EnemyStat[] Enemies = new EnemyStat[]
        {
            // Tier 1: Quái yếu (Wave 1-5)
            new EnemyStat { Name="Slime", MaxHealth=30, Speed=80, Reward=5, DamageToTower=0, AttackRange=0, Color=Color.Green },
            new EnemyStat { Name="Rat", MaxHealth=20, Speed=120, Reward=5, DamageToTower=0, AttackRange=0, Color=Color.Gray },
            new EnemyStat { Name="Bat", MaxHealth=25, Speed=150, Reward=8, DamageToTower=0, AttackRange=0, Color=Color.Black }, // Bay
            new EnemyStat { Name="Goblin", MaxHealth=60, Speed=90, Reward=10, DamageToTower=5, AttackRange=30, Color=Color.DarkGreen }, // Biết đánh nhẹ
            new EnemyStat { Name="Skeleton", MaxHealth=80, Speed=70, Reward=12, DamageToTower=10, AttackRange=30, Color=Color.White },

            // Tier 2: Quái trung bình (Wave 6-10)
            new EnemyStat { Name="Orc", MaxHealth=150, Speed=60, Reward=20, DamageToTower=20, AttackRange=40, Color=Color.DarkOliveGreen },
            new EnemyStat { Name="Wolf", MaxHealth=100, Speed=130, Reward=25, DamageToTower=15, AttackRange=30, Color=Color.Gray },
            new EnemyStat { Name="Spider", MaxHealth=120, Speed=100, Reward=22, DamageToTower=10, AttackRange=30, Color=Color.Brown },
            new EnemyStat { Name="Ghost", MaxHealth=200, Speed=50, Reward=30, DamageToTower=0, AttackRange=0, Color=Color.LightBlue }, // Kháng vật lý (nếu làm)
            new EnemyStat { Name="Witch", MaxHealth=100, Speed=80, Reward=35, DamageToTower=30, AttackRange=150, Color=Color.Purple }, // Đánh xa

            // Tier 3: Quái mạnh (Wave 11-15)
            new EnemyStat { Name="Troll", MaxHealth=500, Speed=40, Reward=50, DamageToTower=50, AttackRange=50, Color=Color.DarkCyan },
            new EnemyStat { Name="Gargoyle", MaxHealth=300, Speed=110, Reward=60, DamageToTower=40, AttackRange=40, Color=Color.DarkSlateBlue },
            new EnemyStat { Name="Vampire", MaxHealth=350, Speed=120, Reward=70, DamageToTower=40, AttackRange=40, Color=Color.Red },
            new EnemyStat { Name="Golem", MaxHealth=1000, Speed=30, Reward=100, DamageToTower=100, AttackRange=40, Color=Color.SandyBrown }, // Phá trụ
            new EnemyStat { Name="Assassin", MaxHealth=200, Speed=200, Reward=80, DamageToTower=50, AttackRange=30, Color=Color.Black },

            // Tier 4: Boss & Siêu quái (Wave 16-20)
            new EnemyStat { Name="Cyclops", MaxHealth=2000, Speed=50, Reward=200, DamageToTower=150, AttackRange=60, Color=Color.DarkOrange },
            new EnemyStat { Name="Hydra", MaxHealth=3000, Speed=40, Reward=300, DamageToTower=100, AttackRange=100, Color=Color.DarkGreen },
            new EnemyStat { Name="Phoenix", MaxHealth=1500, Speed=150, Reward=400, DamageToTower=80, AttackRange=100, Color=Color.OrangeRed },
            new EnemyStat { Name="Titan", MaxHealth=5000, Speed=20, Reward=500, DamageToTower=500, AttackRange=60, Color=Color.DarkBlue }, // One hit trụ
            new EnemyStat { Name="DEMON KING", MaxHealth=10000, Speed=60, Reward=1000, DamageToTower=200, AttackRange=200, Color=Color.Crimson }, // BOSS
        };
    }
}