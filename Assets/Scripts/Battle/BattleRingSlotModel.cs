using System;
using UnityEngine;

public class BattleRingSlotModel : IBaseUnitModel
{
    public int SlotCount { get; private set; }

    public int RemainBubbleCount { get; private set; }

    public Action<BubbleColor> OnChangeBubbleColor { get; private set; } 

    public void SetSlotCount(int slotCount)
    {
        SlotCount = slotCount;
    }

    public void SetOnChangeBubbleColor(Action<BubbleColor> color)
    {
        OnChangeBubbleColor = color;
    }

    public void SetRemainBubbleCount(int remainBubbleCount)
    {
        RemainBubbleCount = remainBubbleCount;
    }

    public void ReduceSpawnCount()
    {
        RemainBubbleCount--;
    }
}
