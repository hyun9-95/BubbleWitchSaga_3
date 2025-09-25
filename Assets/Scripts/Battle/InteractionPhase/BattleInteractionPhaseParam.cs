public class BattleInteractionPhaseParam : IBattlePhaseParam
{
    public BubbleNode LaunchedBubbleNode { get; private set; }

    public void SetLaunchedBubbleNode(BubbleNode launchedBubbleNode)
    {
        LaunchedBubbleNode = launchedBubbleNode;
    }
}
