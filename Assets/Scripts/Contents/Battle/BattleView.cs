using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleView : BaseView
{
    public BattleViewModel Model => GetModel<BattleViewModel>();

    [SerializeField]
    private BattleRingSlot ringSlot;

    [SerializeField]
    private BattleBubbleLauncher bubbleLauncher;

    [SerializeField]
    private ClickBlocker clickBlocker;

    public override async UniTask ShowAsync()
    {
        EnableClickBlocker(true);

        if (ringSlot.Model == null)
        {
            ringSlot.SetModel(Model.BattleRingSlotModel);
            await ringSlot.InitializeSlot();
        }

        if (bubbleLauncher.Model == null)
        {
            bubbleLauncher.SetModel(Model.BattleBubbleLauncherModel);
            await bubbleLauncher.Initialize();
        }

        await RefillRingSlot();
        EnableClickBlocker(false);
    }

    private async UniTask RefillRingSlot()
    {
        await ringSlot.RefillBubble();
    }

    public void EnableClickBlocker(bool enable)
    {
        clickBlocker.SafeSetActive(enable);
    }

    public async UniTask<BubbleNode> LaunchCurrentRingSlot(List<Vector3> path)
    {
        var currentBubble = ringSlot.CurrentBubble;

        if (currentBubble == null)
            return null;

        await currentBubble.MoveAlongPath(path);

        return currentBubble;
    }
}
