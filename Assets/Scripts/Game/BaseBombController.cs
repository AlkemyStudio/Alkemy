using System.Collections;
using DefaultNamespace;
using UnityEngine;

namespace Game
{

    public struct BombData
    {
        public int Power;
        public float FuseTime;
    }

    public class BaseBomb : MonoBehaviour
    {
        private Vector2Int bombPositionOnTheGrid;
        private BombData bombData;

        public void SetupBomb(BombData bombData, Vector2Int bombPositionOnTheGrid)
        {
            this.bombPositionOnTheGrid = bombPositionOnTheGrid;
            this.bombData = bombData;
            
            StartCoroutine(Arm());
        }

        public IEnumerator Arm()
        {
            yield return new WaitForSeconds(bombData.FuseTime);
            Explode();
        }

        public void CancelTimer()
        {
            StopAllCoroutines();
        }

        public void Explode()
        {
            // Remove the bomb 
            TerrainStateManager.GetInstance().SetTileState(
                bombPositionOnTheGrid.x,
                bombPositionOnTheGrid.y,
                TerrainTileState.None
            );

            // TODO: implement explode
        }
    }
}

