#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BattleInteractionPhase : IBattlePhaseProcessor
{
    public BattlePhase Phase => BattlePhase.Interaction;

    private BattleGrid grid;
    private BubbleNode launchedBubbleNode;
    private BattleViewController battleViewController;

    public async UniTask Initialize(BattleGrid grid, BattleViewController viewController)
    {
        this.grid = grid;
        battleViewController = viewController;
    }

    public async UniTask OnStartPhase(IBattlePhaseParam param)
    {
        if (param is BattleInteractionPhaseParam interactionParam)
        {
            launchedBubbleNode = interactionParam.LaunchedBubbleNode;
        }
    }

    public async UniTask OnProcess()
    {
        if (launchedBubbleNode != null)
        {
            await ProcessBubbleInteraction();
        }
    }

    public async UniTask OnEndPhase()
    {
        launchedBubbleNode = null;
    }

    public BattleNextPhaseInfo OnNextPhase()
    {
        // 보스 HP 0일때는 End로.. 추후 구현
        return new BattleNextPhaseInfo(BattlePhase.Stage);
    }

    private async UniTask ProcessBubbleInteraction()
    {
        PlaceBubbleOnGrid();
        await CheckAndRemoveMatches();
        await ApplyGravity();
        await ProcessChainReactions();
    }

    private void PlaceBubbleOnGrid()
    {
        if (launchedBubbleNode == null)
            return;

        var cell = grid.GetCell(launchedBubbleNode.Model.CellPosition);

        if (cell != null)
        {
            cell.SetBubble(launchedBubbleNode);
            launchedBubbleNode.SetColliderEnable(true);
        }
    }

    private async UniTask CheckAndRemoveMatches()
    {
        if (launchedBubbleNode == null)
            return;

        var removeBubbles = new List<BubbleNode>();
        var matchingBubbles = new Queue<BubbleNode>();
        HashSet<CellPosition> visitNodes = new HashSet<CellPosition>();

        BubbleColor targetColor = launchedBubbleNode.Model.BubbleColor;
        matchingBubbles.Enqueue(launchedBubbleNode);
        removeBubbles.Add(launchedBubbleNode);
        visitNodes.Add(launchedBubbleNode.Model.CellPosition);

        while (matchingBubbles.Count > 0)
        {
            var currentBubble = matchingBubbles.Dequeue();
            var neighborPositions = grid.GetNeighborPositions(currentBubble.Model.CellPosition);

            foreach (var neighborPos in neighborPositions)
            {
                var cell = grid.GetCell(neighborPos);

                if (cell == null || cell.IsEmpty || visitNodes.Contains(neighborPos))
                    continue;

                var bubble = cell.Bubble;

                if (bubble.Model.IsColorType && bubble.Model.BubbleColor == targetColor)
                {
                    visitNodes.Add(neighborPos);
                    removeBubbles.Add(bubble);
                    matchingBubbles.Enqueue(bubble);
                }
            }
        }

        if (removeBubbles.Count >= 3)
        {
            await RemoveBubbles(removeBubbles);
        }
    }

    private async UniTask RemoveBubbles(List<BubbleNode> bubblesToRemove)
    {
        foreach (var bubble in bubblesToRemove)
        {
            var cell = grid.GetCell(bubble.Model.CellPosition);

            if (cell != null)
            {
                cell.RemoveBubbe();
                bubble.FadeOff().Forget();
            }
        }
    }

    private async UniTask ApplyGravity()
    {

    }

    private async UniTask ProcessChainReactions()
    {

    }
}
