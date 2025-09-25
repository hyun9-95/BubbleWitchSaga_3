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
    private BattleBoss boss;
    private BattleViewController battleViewController;

    public async UniTask Initialize(BattleGrid grid, BattleViewController viewController)
    {
        this.grid = grid;

        if (Model.BossData != null)
        {
            var maxHp = Mathf.FloorToInt(Model.BossData.HP);
            var viewModel = viewController.GetModel<BattleViewModel>();
            SimpleBarModel simpleBarModel = new SimpleBarModel();
            simpleBarModel.SetMaxValue(maxHp);
            simpleBarModel.SetValue(maxHp);
            viewModel.SetHpBarModel(simpleBarModel);

            boss = await AddressableManager.Instance.
                InstantiateAddressableMonoAsync<BattleBoss>(Model.BossData.PrefabPath);

            var bossCell = grid.GetBossCell();
            boss.SetPosition(bossCell.WorldPos);
            boss.DrawOutline();

            await FillBossArea(bossCell);
        }

        battleViewController = viewController;
    }

    private async UniTask FillBossArea(BattleCell bossCell)
    {
        var noneBubble = await BubbleFactory.Instance.CreateNewBubble(BubbleType.None);
        bossCell.SetBubble(noneBubble);
        noneBubble.SetPosition(bossCell.WorldPos);

        var directions = grid.GetNeighborDirections();

        foreach (var direction in directions)
        {
            var neighborCell = grid.GetDirectionCell(bossCell.CellPos, direction);

            if (neighborCell != null)
            {
                var neighborNoneBubbe = await BubbleFactory.Instance.CreateNewBubble(BubbleType.None);
                neighborCell.SetBubble(neighborNoneBubbe);
                neighborNoneBubbe.SetPosition(neighborCell.WorldPos);
            }
        }
    }

    private async UniTask SpawnBubbles(BattleGrid grid)
    {
        List<UniTask> tasks = new List<UniTask>(spawners.Count);

        foreach (var spawner in spawners)
            tasks.Add(spawner.SpawnAsync(grid, Model.SpawnCount / spawners.Count));

        await UniTask.WhenAll(tasks);
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
