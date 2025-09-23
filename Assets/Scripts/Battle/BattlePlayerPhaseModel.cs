using UnityEngine;

public class BattlePlayerPhaseModel : IBaseUnitModel
{
    public int UserBubbleCount { get; private set; }

    public void SetUserBubbleCount(int count)
    {
        UserBubbleCount = count;
    }
}
