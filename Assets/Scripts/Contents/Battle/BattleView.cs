using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleView : BaseView
{
    public BattleViewModel Model => GetModel<BattleViewModel>();

    [SerializeField]
    private BattleRingSlot ringSlot;

    public override async UniTask ShowAsync()
    {
        if (ringSlot.Model == null)
        {
            ringSlot.SetModel(Model.BattleRingSlotModel);
            await ringSlot.InitializeSlot();
        }
    }

    public async UniTask RefillRingSlot()
    {
        await ringSlot.RefillBubble();
    }
}
