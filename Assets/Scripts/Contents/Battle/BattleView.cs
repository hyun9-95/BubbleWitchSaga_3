using Cysharp.Threading.Tasks;
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
    }

    public async UniTask RefillRingSlot()
    {
        await ringSlot.RefillBubble();
    }
}
