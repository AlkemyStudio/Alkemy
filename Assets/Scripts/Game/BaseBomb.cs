namespace IA;

public struct BombData
{
    public int power;
    public float fuseTime;
}

public class BaseBomb
{
    private Vector2 bombPositionOnTheGrid;
    private BombData bombData;

    public void SetupBomb(BombData bombData, Vector2 bombPositionOnTheGrid)
    {
        this.bombPositionOnTheGrid = bombPositionOnTheGrid;
        this.bombData = bombData;
        
        Couroutine.Start("Explode", bombData.fuseTime)
    }

    public void CancelTimer()
    {
        Couroutine.StopAll();
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