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
    private BattleNextPhaseInfo nextPhaseInfo;
    private bool launched = false;

    public async UniTask Initialize(BattleGrid grid, BattleViewController viewController)
    {
        this.grid = grid;

        BattleViewModel viewModel = viewController.GetModel<BattleViewModel>();

        BattleRingSlotModel battleRingSlotModel = new();
        battleRingSlotModel.SetSlotCount(IntDefine.MAX_RINGSLOT_COUNT);
        battleRingSlotModel.SetRemainBubbleCount(Model.UserBubbleCount);
        viewModel.SetBattleRingSlotModel(battleRingSlotModel);

        BattleBubbleLauncherModel battleLauncherModel = new();
        battleLauncherModel.SetOnFindClosestEmptyCell(OnFindClosestEmptyCell);
        battleLauncherModel.SetOnLaunch(OnLaunch);
        viewModel.SetBattleBubbleLauncherModel(battleLauncherModel);

        battleViewController = viewController;
    }

    public async UniTask OnStartPhase(IBattlePhaseParam param)
    {
        await battleViewController.Process();
        launched = false;
    }

    public async UniTask OnProcess()
    {
        while (launchedBubbleNode == null)
            await UniTask.NextFrame(TokenPool.Get(GetHashCode()));
    }

    public async UniTask OnEndPhase()
    {
        BattleInteractionPhaseParam interactionPhaseParam = new();
        interactionPhaseParam.SetLaunchedBubbleNode(launchedBubbleNode);

        nextPhaseInfo = new(BattlePhase.Interaction, interactionPhaseParam);
        launchedBubbleNode = null;
    }

    public BattleNextPhaseInfo OnNextPhase()
    {
        return nextPhaseInfo;
    }

    private BattleCell OnFindClosestEmptyCell(BubbleHitInfo hitInfo)
    {
        return grid.GetClosestEmptyCell(hitInfo);
    }

    private void OnLaunch(List<Vector3> movePath, CellPosition targetCellPos)
    {
        OnLaunchAsync(movePath, targetCellPos).Forget();
    }

    private async UniTask OnLaunchAsync(List<Vector3> movePath, CellPosition targetCellPos)
    {
        if (launched)
            return;

        launched = true;

        launchedBubbleNode = await battleViewController.
            LaunchCurrentBubble(movePath, targetCellPos);
    }
}
