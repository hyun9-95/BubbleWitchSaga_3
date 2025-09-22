using UnityEngine;

public class BubbleNodeModel : IBaseUnitModel
{
    public int Index { get; private set; }
    public BubbleType BubbleType { get; private set; }
    public BubbleColor BubbleColor { get; private set; }
    public void AddIndex()
    {
        Index++;
    }
}
