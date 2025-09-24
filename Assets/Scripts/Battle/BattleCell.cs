using UnityEngine;

public class BattleCell
{
    public CellPosition CellPos { get; private set; }
    public Vector3 Position { get; private set; }
    public BubbleNode Bubble { get; set; }
    public bool IsEmpty => Bubble == null;
    public bool Closed {  get; private set; }
    public BattleCell(CellPosition gridPos, Vector3 pos)
    {
        CellPos = gridPos;
        Position = pos;
        Bubble = null;
        Closed = false;
    }

    public void SetClosed(bool value)
    {
        Closed = value;
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

    public void SetRootPos(CellPosition cellPos)
    {
        if (Bubble == null)
            return;

        Bubble.Model.SetRootPos(cellPos);
    }
}
