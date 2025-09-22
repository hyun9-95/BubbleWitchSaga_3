public class BattleStageModel : IBaseUnitModel
{
    public int SpawnCount { get; private set; }

    public void SetSpawnCount(int spawnCount)
    {
        SpawnCount = spawnCount;
    }
}
