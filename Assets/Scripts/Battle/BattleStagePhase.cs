#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleStagePhase : BaseUnit<BattleStageModel>, IBattlePhaseProcessor
{
    public BattlePhase Phase => BattlePhase.Stage;

    [SerializeField]
    private List<BubbleSpawner> spawners;

    private BattleGrid grid;

    public async UniTask Initialize(BattleGrid grid)
    {
        this.grid = grid;
    }

    private async UniTask SpawnBubbles(BattleGrid grid)
    {
        List<UniTask> task = new List<UniTask>(spawners.Count);

        foreach (var spawner in spawners)
            task.Add(spawner.SpawnAsync(grid, Model.SpawnCount / spawners.Count));

        await UniTask.WhenAll(task);
    }

    public async UniTask OnStartPhase()
    {
        await SpawnBubbles(grid);
    }

    public void OnProcessPhase()
    {

    }

    public async UniTask OnEndPhase()
    {

    }
}
