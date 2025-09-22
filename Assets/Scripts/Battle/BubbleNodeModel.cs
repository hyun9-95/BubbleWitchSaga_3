public class BubbleNodeModel : IBaseUnitModel
{
    public int Index { get; private set; }
    public BubbleType BubbleType { get; private set; }
    public BubbleColor BubbleColor { get; private set; }
    public float MoveSpeed { get; private set; }
    public void AddIndex()
    {
        Index++;
    }
    public void SetBubbleType(BubbleType type)
    {
        BubbleType = type;
    }

    public void SetBubbleColor(BubbleColor color)
    {
        BubbleColor = color;
    }

    public void SetMoveSpeed(float speed)
    {
        MoveSpeed = speed;
    }
}
