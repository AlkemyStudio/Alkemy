using UnityEngine;

namespace Terrain
{
    public static class TerrainUtils
    {
        public static Vector3 GetTerrainPosition(Vector3 worldPosition)
        {
            return new Vector3(Mathf.FloorToInt(worldPosition.x) + 0.5F, 0.5f, Mathf.FloorToInt(worldPosition.z) + 0.5F);
        }
    }
}