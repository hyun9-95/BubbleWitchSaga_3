#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleInteractionPhase : IBattlePhaseProcessor
{
    public BattlePhase Phase => BattlePhase.Interaction;

    private BattleGrid grid;
    private BubbleNode launchedBubbleNode;
    private BattleViewController battleViewController;
    private BattleCellDirection[] neighborDirections;

    #region Temp
    private bool isThreeMatched = false;
    private List<UniTask> fairyTasks = new();
    private HashSet<CellPosition> visitNodes = new();
    #endregion

    public async UniTask Initialize(BattleGrid grid, BattleViewController viewController)
    {
        this.grid = grid;
        battleViewController = viewController;
        neighborDirections = grid.GetNeighborDirections();
    }

    public async UniTask OnStartPhase(IBattlePhaseParam param)
    {
        if (param is BattleInteractionPhaseParam interactionParam)
            launchedBubbleNode = interactionParam.LaunchedBubbleNode;

        isThreeMatched = false;
        fairyTasks.Clear();
    }

    public async UniTask OnProcess()
    {
        if (launchedBubbleNode != null)
            await ProcessBubbleInteraction();
    }

    private async UniTask ProcessBubbleInteraction()
    {
        PlaceBubble();
        ThreeMatchBFS();

        if (fairyTasks.Count > 0)
        {
            await UniTask.WhenAll(fairyTasks);
        }
        else if (isThreeMatched)
        {
            await UniTaskUtils.DelaySeconds(FloatDefine.INTERACTION_PHASE_DELAY);
        }
    }

    public async UniTask OnEndPhase()
    {
        launchedBubbleNode = null;
    }

    public BattleNextPhaseInfo OnNextPhase()
    {
        return new BattleNextPhaseInfo(GetNextPhase());
    }

    private void PlaceBubble()
    {
        if (launchedBubbleNode == null)
            return;

        var cell = grid.GetCell(launchedBubbleNode.Model.CellPos);

        if (cell != null)
        {
            cell.SetBubble(launchedBubbleNode);
            launchedBubbleNode.SetColliderEnable(true);
        }
    }

    private void ThreeMatchBFS()
    {
        if (launchedBubbleNode == null)
            return;

        var bubblesToRemove = new HashSet<BubbleNode>();
        var matchingBubbles = new Queue<BubbleNode>();
        bool foundMagic = false;

        visitNodes.Clear();

        BubbleColor targetColor = launchedBubbleNode.Model.BubbleColor;
        matchingBubbles.Enqueue(launchedBubbleNode);
        bubblesToRemove.Add(launchedBubbleNode);
        visitNodes.Add(launchedBubbleNode.Model.CellPos);

        while (matchingBubbles.Count > 0)
        {
            var currentBubble = matchingBubbles.Dequeue();

            IterateValidNeighborCell(currentBubble.Model.CellPos, (neighborCell) =>
            {
                var bubble = neighborCell.Bubble;

                if (bubble.Model.IsColorType && bubble.Model.BubbleColor == targetColor)
                {
                    visitNodes.Add(neighborCell.CellPos);
                    bubblesToRemove.Add(bubble);
                    matchingBubbles.Enqueue(bubble);
                }
                else if (bubble.Model.BubbleType == BubbleType.Magic)
                {
                    foundMagic = true;

                    visitNodes.Add(neighborCell.CellPos);
                    bubblesToRemove.Add(bubble);

                    CollectMagicTargetBubbles(neighborCell.CellPos, bubblesToRemove);
                }
            });
        }

        // 3매치 OR 매직버블
        if (bubblesToRemove.Count >= 3 || foundMagic)
            RemoveBubbleBFS(bubblesToRemove);
    }

    private void CollectMagicTargetBubbles(CellPosition rootCellPos, HashSet<BubbleNode> bubblesToRemove)
    {
        IterateValidNeighborCell(rootCellPos, (neighborCell) =>
        {
            visitNodes.Add(neighborCell.CellPos);
            bubblesToRemove.Add(neighborCell.Bubble);
        });
    }

    private void RemoveBubbleBFS(HashSet<BubbleNode> bubblesToRemove)
    {
        isThreeMatched = true;

        var dropBubbles = new Queue<BubbleNode>();
        visitNodes.Clear();

        foreach (var bubble in bubblesToRemove)
        {
            var cell = grid.GetCell(bubble.Model.CellPos);

            if (bubble.Model.BubbleType == BubbleType.Fairy)
                fairyTasks.Add(OnFairyDamage(cell.Position));

            if (cell != null)
            {
                cell.RemoveBubble();
                bubble.FadeOff().Forget();
            }

            CollectDropBubbles(bubble.Model.CellPos, dropBubbles);
        }

        while (dropBubbles.Count > 0)
        {
            var dropBubble = dropBubbles.Dequeue();
            var dropCell = grid.GetCell(dropBubble.Model.CellPos);

            if (dropCell != null)
            {
                dropCell.RemoveBubble();
                dropBubble.DropFadeOff(grid.DropPosY).Forget();
            }

            CollectDropBubbles(dropBubble.Model.CellPos, dropBubbles);
        }
    }

    private void CollectDropBubbles(CellPosition rootCellPos, Queue<BubbleNode> dropBubbles)
    {
        IterateValidNeighborCell(rootCellPos, (neighborCell) =>
        {
            var neighborBubble = neighborCell.Bubble;

            if (neighborBubble.Model.RootPos.Equals(rootCellPos))
            {
                visitNodes.Add(neighborCell.CellPos);
                dropBubbles.Enqueue(neighborBubble);
            }
        });
    }

    private void IterateValidNeighborCell(CellPosition rootPos, System.Action<BattleCell> action)
    {
        foreach (var direction in neighborDirections)
        {
            var neighborCell = grid.GetDirectionCell(rootPos, direction);

            if (neighborCell == null)
                continue;

            var neighborCellPos = neighborCell.CellPos;

            if (neighborCell.IsEmpty || visitNodes.Contains(neighborCellPos))
                continue;

            action(neighborCell);
        }
    }

    private async UniTask OnFairyDamage(Vector2 startPos)
    {
        var bossPos = grid.GetBossCell().Position;

        var damageBubble = await BubbleFactory.Instance.CreateNewBubble(BubbleType.Empty);
        damageBubble.Model.SetMoveSpeed(FloatDefine.BATTLE_FAIRY_BUBBLE_SPEED);
        damageBubble.SetPosition(startPos);
        damageBubble.SetColliderEnable(false);

        await damageBubble.SmoothMove(bossPos, () => DealsFairyDamage(damageBubble));
    }

    // 대미지 + 페이드아웃
    private void DealsFairyDamage(BubbleNode bubble)
    {
        battleViewController.DealsFairyDamage();
        bubble.FadeOff().Forget();
    }

    // 승패조건 판별
    private BattlePhase GetNextPhase()
    {
        var viewModel = battleViewController.GetModel<BattleViewModel>();

        if (viewModel.HpBarModel.Value == 0)
        {
            return BattlePhase.Win;
        }
        else if (viewModel.BattleRingSlotModel.RemainBubbleCount == 0)
        {
            return BattlePhase.Defeat;
        }

        return BattlePhase.Stage;
    }
}
