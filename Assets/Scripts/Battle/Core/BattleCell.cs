using System;
using UnityEngine;

public class BattleCell
{
    public BattleCell(CellPosition cellPos, Vector3 pos, Action<int> onSetBubble, Action<int> onRemoveBubble)
    {
        CellPos = cellPos;
        WorldPos = pos;
        OnSetBubble = onSetBubble;
        OnRemoveBubble = onRemoveBubble;
        Bubble = null;
    }

    #region Property
    public bool IsEmpty => Bubble == null;
    public CellPosition CellPos { get; private set; }
    public Vector3 WorldPos { get; private set; }
    public BubbleNode Bubble { get; private set; }
    public Action<int> OnSetBubble { get; private set; }
    public Action<int> OnRemoveBubble { get; private set; }
    #endregion

    #region Function
    public void SetBubble(BubbleNode bubble)
    {
        if (bubble == null)
            return;

        Bubble = bubble;
        OnSetBubble(CellPos.row);
    }

    public void RemoveBubble()
    {
        if (Bubble == null)
            return;

        Bubble.OnRemoveFromCell();
        Bubble = null;
        OnRemoveBubble(CellPos.row);
    }
    #endregion
}
