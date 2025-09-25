using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSnakeSpawner : BubbleSpawner
{
    private enum StartDirection
    {
        Left,
        Right,
    }

    [SerializeField]
    private StartDirection direction;

    [SerializeField]
    private CellPosition cellPos;

    [SerializeField]
    private int maxColumnSpawn = 4;

    [SerializeField]
    private float snakeMoveSpeed = 7f;

    private List<BattleCell> spawnPath;
    private HashSet<BubbleNode> bubbleSet;
    private BubbleNode spawnBubble;

    public async override UniTask SpawnAsync(BattleGrid grid, int spawnCount)
    {
        if (spawnPath == null)
        {
            spawnPath = new(spawnCount);
            bubbleSet = new(spawnCount);

            BuildSpawnPath(grid, spawnCount);
        }

        await SpawnSnakeBubbleAsync(grid);
    }

    private void BuildSpawnPath(BattleGrid grid, int spawnCount)
    {
        spawnPath.Clear();

        CellPosition currentCellPos = cellPos;
        bool movingRight = direction == StartDirection.Right;

        int maxColumnCount = grid.BaseColumnsCount;
        int maxColumnIndex = maxColumnCount - 1;
        int currentAddColumnCount = 0;

        while (spawnPath.Count < spawnCount)
        {
            bool isOdd = (currentCellPos.row % 2 == 1);
            int minColumn = isOdd ? 1 : 0;
            int maxColumn = isOdd ? maxColumnIndex : maxColumnIndex - 1;

            bool isOutOfRange = movingRight ? currentCellPos.column >= maxColumn
                : currentCellPos.column <= minColumn;

            bool isOverAddColumnCount = currentAddColumnCount >= maxColumnSpawn;

            if (isOutOfRange || isOverAddColumnCount)
            {
                var nextDirection = movingRight ?
                    BattleCellDirection.BottomRight :
                    BattleCellDirection.BottomLeft;

                currentCellPos.Move(nextDirection);
                spawnPath.Add(grid.GetCell(currentCellPos));

                nextDirection = nextDirection == BattleCellDirection.BottomLeft ?
                    BattleCellDirection.BottomRight :
                    BattleCellDirection.BottomLeft;

                currentCellPos.Move(nextDirection);
                spawnPath.Add(grid.GetCell(currentCellPos));

                movingRight = !movingRight;
                currentAddColumnCount = 1;
            }
            else
            {
                if (movingRight)
                {
                    currentCellPos.column++;
                }
                else
                {
                    currentCellPos.column--;
                }

                spawnPath.Add(grid.GetCell(currentCellPos));
                currentAddColumnCount++;
            }
        }
    }

    private async UniTask SpawnSnakeBubbleAsync(BattleGrid grid)
    {
        int spawnCount = spawnPath.Count - bubbleSet.Count;
        var spawnCell = grid.GetCell(cellPos);
        var startPos = spawnCell.WorldPos;

        if (spawnBubble == null)
        {
            spawnBubble = await BubbleFactory.Instance.CreateNewBubble(BubbleType.Spawn);
            spawnBubble.SetPosition(startPos);
            spawnBubble.SetColliderEnable(true);
            spawnCell.SetBubble(spawnBubble);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            List<UniTask> tasks = new(bubbleSet.Count + 1);

            var newBubble = await BubbleFactory.Instance.CreateNewBubble(true, true);
            var newBubbleModel = newBubble.Model;
            newBubbleModel.SetMoveSpeed(snakeMoveSpeed);

            newBubble.SetModel(newBubbleModel);
            newBubble.SetPosition(startPos);

            var newCell = spawnPath[0];
            newCell.SetBubble(newBubble);
            newBubbleModel.SetCellPos(newCell.CellPos);

            tasks.Add(newBubble.SmoothMove(newCell.WorldPos));
            
            foreach (var bubble in bubbleSet)
            {
                var bubbleModel = bubble.Model;
                if (bubbleModel.Index + 1 >= spawnPath.Count)
                    continue;

                if (spawnPath[bubbleModel.Index] == null)
                {
                    Logger.Error($"Path 에러 : {bubbleModel.Index}");
                    continue;
                }

                bubbleModel.AddIndex();

                var cell = spawnPath[bubbleModel.Index];
                cell.SetBubble(bubble);
                bubbleModel.SetCellPos(cell.CellPos);

                // 스네이크 경로 기준 뒤에 있는 것을 루트로
                if (bubbleModel.Index - 1 > 0)
                {
                    int rootIndex = bubbleModel.Index - 1;
                    var rootCell = spawnPath[rootIndex];
                    bubbleModel.SetRootPos(rootCell.CellPos);
                }

                tasks.Add(bubble.SmoothMove(cell.WorldPos));
            }

            await UniTask.WhenAll(tasks);

            newBubbleModel.SetOnRemoveFromCell(() => { bubbleSet.Remove(newBubble); });
            bubbleSet.Add(newBubble);
        }
    }
}