using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleView : BaseView
{
    public BattleViewModel Model => GetModel<BattleViewModel>();

    [SerializeField]
    private BattleRingSlot ringSlot;

    [SerializeField]
    private BattleBubbleLauncher bubbleLauncher;

    [SerializeField]
    private AddressableLoader hpBarLoader;

    [SerializeField]
    private ClickBlocker clickBlocker;

    private SimpleBar hpBar;

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

        if (Model.HpBarModel != null && hpBar == null)
        {
            hpBar = await hpBarLoader.InstantiateAsyc<SimpleBar>();
            hpBar.SetModel(Model.HpBarModel);
            await hpBar.ShowAsync();
        }

        if (hpBar != null)
            hpBar.RefreshBar();

        await RefillRingSlot();
    }

    public void RefreshHpBar()
    {
        if (hpBar != null)
            hpBar.RefreshBar();
    }

    private async UniTask RefillRingSlot()
    {
        await ringSlot.RefillBubble();
    }

    public void EnableClickBlocker(bool enable)
    {
        clickBlocker.SafeSetActive(enable);
    }

    public BattleRingSlot GetRingSlot()
    {
        return ringSlot;
    }
}
