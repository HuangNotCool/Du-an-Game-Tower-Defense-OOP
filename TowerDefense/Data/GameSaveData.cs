using System.Collections.Generic;

namespace TowerDefense.Data
{
	// Class này chỉ dùng để chứa dữ liệu, không có logic
	public class GameSaveData
	{
		public int Money { get; set; }
		public int Lives { get; set; }
		public int CurrentWave { get; set; }
		public int LevelId { get; set; }

		// Danh sách các tháp đã xây
		public List<TowerData> Towers { get; set; } = new List<TowerData>();
	}

	public class TowerData
	{
		public string Type { get; set; } // "Archer" hoặc "Cannon"
		public float X { get; set; }
		public float Y { get; set; }
	}
}