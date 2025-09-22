public class BattleViewModel : IBaseViewModel
{
    public BattleRingSlotModel BattleRingSlotModel { get; private set; }

    public void SetBattleRingSlotModel(BattleRingSlotModel model)
    {
        BattleRingSlotModel = model;
    }
}
