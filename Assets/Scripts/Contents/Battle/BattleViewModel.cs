public class BattleViewModel : IBaseViewModel
{
    public BattleRingSlotModel BattleRingSlotModel { get; private set; }
    public BattleBubbleLauncherModel BattleBubbleLauncherModel { get; private set; }

    public void SetBattleRingSlotModel(BattleRingSlotModel model)
    {
        BattleRingSlotModel = model;
    }

    public void SetBattleBubbleLauncherModel(BattleBubbleLauncherModel model)
    {
        BattleBubbleLauncherModel = model;
    }
}
