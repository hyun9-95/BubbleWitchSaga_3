#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerPhase : IBattlePhaseProcessor
{
    public BattlePlayerPhaseModel Model { get; private set; }

    public void SetModel(BattlePlayerPhaseModel model)
    {
        Model = model;
    }
    
    public BattlePhase Phase => BattlePhase.Player;

    private BattleGrid grid;
    private BattleViewController battleViewController;
    private BubbleNode launchedBubbleNode;

    public async UniTask Initialize(BattleGrid grid)
    {
        this.grid = grid;

        await ShowBattleView();
    }

    public async UniTask OnStartPhase(IBattlePhaseParam param)
    {
        if (battleViewController == null)
        {
            await ShowBattleView();
            return;
        }

        await battleViewController.Process();
    }

    public async UniTask OnProcess()
    {
        while (launchedBubbleNode != null)
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
    }

    public async UniTask OnEndPhase()
    {
        battleViewController.EnableClickBlocker(true);
    }

    public BattleNextPhaseInfo OnNextPhase()
    {
        BattleInteractionPhaseParam interactionPhaseParam = new();
        interactionPhaseParam.SetLaunchedBubbleNode(launchedBubbleNode);

        BattleNextPhaseInfo nextPhaseInfo = new(BattlePhase.Interaction, interactionPhaseParam);
        launchedBubbleNode = null;

        return nextPhaseInfo;
    }

    private async UniTask ShowBattleView()
    {
        battleViewController = new BattleViewController();
        BattleViewModel viewModel = new BattleViewModel();

        BattleRingSlotModel battleRingSlotModel = new BattleRingSlotModel();
        battleRingSlotModel.SetSlotCount(IntDefine.MAX_RINGSLOT_COUNT);
        battleRingSlotModel.SetRemainBubbleCount(Model.UserBubbleCount);
        viewModel.SetBattleRingSlotModel(battleRingSlotModel);

        BattleBubbleLauncherModel battleLauncherModel = new BattleBubbleLauncherModel();
        battleLauncherModel.SetOnFindClosestEmptyCell(OnFindClosestEmptyCell);
        battleLauncherModel.SetOnLaunch(OnLaunch);

        viewModel.SetBattleBubbleLauncherModel(battleLauncherModel);

        battleViewController.SetModel(viewModel);

        await UIManager.Instance.ChangeView(battleViewController);
    }

    private BattleCell OnFindClosestEmptyCell(CellPosition cellPos, Vector2 hitPos)
    {
        return grid.GetClosestEmptyCell(cellPos, hitPos);
    }

    private void OnLaunch(List<Vector3> movePath)
    {
        OnLaunchAsync(movePath).Forget();
    }

    private async UniTask OnLaunchAsync(List<Vector3> movePath)
    {
        launchedBubbleNode = await battleViewController.LaunchCurrentBubble(movePath);
    }
}
