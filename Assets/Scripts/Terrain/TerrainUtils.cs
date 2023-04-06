using UnityEngine;

namespace Terrain
{
    public static class TerrainUtils
    {
        public static Vector3 GetTerrainPosition(Vector3 worldPosition)
        {
            return new Vector3(Mathf.FloorToInt(worldPosition.x) + 0.5F, 0.5f, Mathf.FloorToInt(worldPosition.z) + 0.5F);
        }

        public static Vector2Int GetTilePosition(Vector3 worldPosition)
        {
            int tilePosX = Mathf.FloorToInt(worldPosition.x);
            int tilePosZ = Mathf.FloorToInt(worldPosition.z);
            return new Vector2Int(tilePosX, tilePosZ);
        }
    }
}