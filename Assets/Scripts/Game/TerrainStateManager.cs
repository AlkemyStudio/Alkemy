namespace IA
{
	public enum TerrainTileState
	{
		None,
		Bomb,
		IndestructibleWall,
		Wall
	}

	public class TerrainStateManager
	{
		private static TerrainStateManager _instance;

		public const int Width = 17;
		public const int Height = 11;

		private TerrainTileState[] terrainState = new TerrainTileState[(Width + 2) * (Height + 2)];

		public void Init()
		{
			// This function is responsible for generating the terrain for the game.
			// The terrain is a 2D array of ints, each representing a type of tile.
			// The terrain is surrounded by a border of indestructible walls.
			// The following code iterates through each tile in the terrain, setting
			// the tile to a wall if it is not on the border, and to an indestructible
			// wall if it is on the border.
			for (int y = 0; y < Height + 2; y++)
			{
				for (int x = 0; x < Width + 2; x++)
				{
					if (y == 0 || y == Height + 1 || x == 0 || x == Width + 1 || (y % 2 == 0 && x % 2 == 0))
					{
						SetTileState(x, y, TerrainTileState.IndestructibleWall);
						continue;
					}

					SetTileState(x, y, TerrainTileState.Wall);
				}
			}

			// This code is responsible for creating the four corners of the map.
			// This is done by setting the tiles around the corners to None, thus
			// creating a gap in the indestructible wall border.
			// Left Top Corner
			SetTileState(1, 1, TerrainTileState.None);
			SetTileState(2, 1, TerrainTileState.None);
			SetTileState(3, 1, TerrainTileState.None);
			SetTileState(1, 2, TerrainTileState.None);

			// Right Top Corner
			SetTileState(Width, 1, TerrainTileState.None);
			SetTileState(Width - 1, 1, TerrainTileState.None);
			SetTileState(Width - 2, 1, TerrainTileState.None);
			SetTileState(Width, 2, TerrainTileState.None);

			// Right Bottom Corner
			SetTileState(Width, Height, TerrainTileState.None);
			SetTileState(Width - 1, Height, TerrainTileState.None);
			SetTileState(Width - 2, Height, TerrainTileState.None);
			SetTileState(Width, Height - 1, TerrainTileState.None);

			// Left Bottom Corner
			SetTileState(1, Height, TerrainTileState.None);
			SetTileState(2, Height, TerrainTileState.None);
			SetTileState(3, Height, TerrainTileState.None);
			SetTileState(1, Height - 1, TerrainTileState.None);
		}

		public static TerrainStateManager GetInstance()
		{
			if (_instance == null)
			{
				_instance = new TerrainStateManager();
			}

			return _instance;
		}

		public void SetTileState(int x, int y, TerrainTileState state)
		{
			terrainState[y * Width + x] = state;
		}

		public void RemoveTile(int x, int y)
		{
			SetTileState(x, y, TerrainTileState.None);
		}
	}
}