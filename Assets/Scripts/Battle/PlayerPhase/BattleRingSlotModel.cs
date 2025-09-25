using System;
using System.Collections.Generic;

public class BattleRingSlotModel : IBaseUnitModel
{
    public int SlotCount { get; private set; }

    public int RemainBubbleCount { get; private set; }

    public Action<BubbleColor> OnChangeBubbleColor { get; private set; }

    public List<BubbleNode> BubbleNodes { get; private set; }

    public void SetSlotCount(int slotCount)
    {
        SlotCount = slotCount;
        BubbleNodes = new List<BubbleNode>(slotCount);
    }

    public void SetOnChangeBubbleColor(Action<BubbleColor> color)
    {
        OnChangeBubbleColor = color;
    }

    public void SetRemainBubbleCount(int remainBubbleCount)
    {
        RemainBubbleCount = remainBubbleCount;
    }

    public void ReduceBubbleCount()
    {
        RemainBubbleCount--;
    }

    public BubbleNode ConsumeCurrentBubble()
    {
        if (BubbleNodes == null || BubbleNodes.Count == 0)
            return null;

        BubbleNode currentBubble = BubbleNodes[0];

        BubbleNodes[0] = null;

        return currentBubble;
    }
}
