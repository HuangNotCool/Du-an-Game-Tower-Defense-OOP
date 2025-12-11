using System.Collections.Generic;

namespace TowerDefense.Data
{
    public class GameSaveData
    {
        public int Money { get; set; }
        public int Lives { get; set; }
        public int CurrentWave { get; set; }
        public int LevelId { get; set; }
        public List<TowerData> Towers { get; set; } = new List<TowerData>();
    }

    public class TowerData
    {
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}