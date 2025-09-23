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

    public override async UniTask ShowAsync()
    {
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
    }

    private async UniTask RefillRingSlot()
    {
        await ringSlot.RefillBubble();
    }

    public async UniTask LaunchCurrentRingSlot(List<Vector3> path)
    {
        var currentBubble = ringSlot.CurrentBubble;

        if (currentBubble == null)
            return;

        await currentBubble.MoveAlongPath(path);
    }
}
