#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleStagePhase : BaseUnit<BattleStageModel>, IBattlePhaseProcessor
{
    public BattlePhase Phase => BattlePhase.Stage;

    [SerializeField]
    private List<BubbleSpawner> spawners;

    private async UniTask SpawnBubbles(BattleGrid grid)
    {
        List<UniTask> task = new List<UniTask>(spawners.Count);

        foreach (var spawner in spawners)
            task.Add(spawner.SpawnAsync(grid, Model.SpawnCount / spawners.Count));

        await UniTask.WhenAll(task);
    }

    public async UniTask OnStartPhase(BattleGrid grid)
    {
        await SpawnBubbles(grid);
    }

    public void OnProcessPhase(BattleGrid grid)
    {

    }

    public async UniTask OnEndPhase(BattleGrid grid)
    {

    }
}
