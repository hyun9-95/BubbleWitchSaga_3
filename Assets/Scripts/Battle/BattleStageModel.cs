public class BattleStageModel : IBaseUnitModel
{
    public int SpawnCount { get; private set; }

    public DataBattleBoss BossData { get; private set; }

    public void SetSpawnCount(int spawnCount)
    {
        SpawnCount = spawnCount;
    }

    public void SetBossData(DataBattleBoss bossData)
    {
        BossData = bossData;
    }
}
