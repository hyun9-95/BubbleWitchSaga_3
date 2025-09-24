#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class BattleStagePhase : BaseUnit<BattleStageModel>, IBattlePhaseProcessor
{
    public BattlePhase Phase => BattlePhase.Stage;

    [SerializeField]
    private List<BubbleSpawner> spawners;

    private BattleGrid grid;
    private BattleViewController battleViewController;
    public async UniTask Initialize(BattleGrid grid, BattleViewController viewController)
    {
        this.grid = grid;

        if (Model.BossData != null)
        {
            var viewModel = viewController.GetModel<BattleViewModel>();
            SimpleBarModel simpleBarModel = new SimpleBarModel();
            simpleBarModel.SetMaxGauge(Model.BossData.HP);
            simpleBarModel.SetGauge(Model.BossData.HP);
            viewModel.SetHpBarModel(simpleBarModel);
        }

        battleViewController = viewController;
    }

    private async UniTask SpawnBubbles(BattleGrid grid)
    {
        List<UniTask> task = new List<UniTask>(spawners.Count);

        foreach (var spawner in spawners)
            task.Add(spawner.SpawnAsync(grid, Model.SpawnCount / spawners.Count));

        await UniTask.WhenAll(task);
    }

    public async UniTask OnStartPhase(IBattlePhaseParam param)
    {
        await SpawnBubbles(grid);
    }

    public async UniTask OnProcess()
    {

    }

    public async UniTask OnEndPhase()
    {

    }

    public BattleNextPhaseInfo OnNextPhase()
    {
        return new BattleNextPhaseInfo(BattlePhase.Player);
    }
}
