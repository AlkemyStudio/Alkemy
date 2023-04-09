using UnityEngine;

namespace Terrain
{
    /// <summary>
    /// This class is used to get the position of the terrain
    /// </summary>
    public static class TerrainUtils
    {
        /// <summary>
        /// This method is used to get the position of the terrain
        /// </summary>
        /// <param name="worldPosition"> The world position </param>
        /// <returns> The position of the terrain </returns>
        public static Vector3 GetTerrainPosition(Vector3 worldPosition)
        {
            return new Vector3(Mathf.FloorToInt(worldPosition.x) + 0.5F, 0.5f, Mathf.FloorToInt(worldPosition.z) + 0.5F);
        }

        /// <summary>
        /// Get the tile position from a world position
        /// </summary>
        /// <param name="worldPosition"> The world position </param>
        /// <returns> The tile position </returns>
        public static Vector2Int GetTilePosition(Vector3 worldPosition)
        {
            int tilePosX = Mathf.FloorToInt(worldPosition.x);
            int tilePosZ = Mathf.FloorToInt(worldPosition.z);
            return new Vector2Int(tilePosX, tilePosZ);
        }
    }
}