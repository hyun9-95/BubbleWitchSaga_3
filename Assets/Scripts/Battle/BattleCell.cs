using UnityEngine;

public class BattleCell
{
    public CellPosition CellPos { get; private set; }
    public Vector3 WorldPos { get; private set; }
    public BubbleNode Bubble { get; set; }
    public bool IsEmpty => Bubble == null;
    public BattleCell(CellPosition cellPos, Vector3 pos)
    {
        CellPos = cellPos;
        WorldPos = pos;
        Bubble = null;
    }

    public void SetBubble(BubbleNode bubble)
    {
        if (bubble == null)
            return;

        Bubble = bubble;
    }

    public void RemoveBubble()
    {
        if (Bubble == null)
            return;

        Bubble.OnRemoveFromCell();
        Bubble = null;
    }
}
