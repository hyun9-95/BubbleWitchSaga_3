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

    private List<BattleCell> spawnPath = new List<BattleCell>();
    private HashSet<BubbleNode> bubbleList = new HashSet<BubbleNode>();

    public async override UniTask SpawnAsync(BattleGrid grid, int spawnCount)
    {
        if (spawnPath.Count == 0)
            BuildSpawnPath(grid, spawnCount);

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
        int spawnCount = spawnPath.Count - bubbleList.Count;
        var spawnCell = grid.GetCell(cellPos);
        var startPos = spawnCell.Position;

        var spawnBubble = await BubbleFactory.Instance.CreateNewBubble(BubbleType.Spawn);
        spawnBubble.SetPosition(startPos);
        spawnBubble.SetColliderEnable(true);
        spawnCell.SetBubble(spawnBubble);

        for (int i = 0; i < spawnCount; i++)
        {
            List<UniTask> tasks = new List<UniTask>(bubbleList.Count + 1);

            var newBubble = await BubbleFactory.Instance.CreateNewBubble(true, true);
            var model = newBubble.Model;
            model.SetMoveSpeed(snakeMoveSpeed);

            newBubble.SetModel(model);
            newBubble.SetPosition(startPos);

            var newCell = spawnPath[0];
            newCell.SetBubble(newBubble);
            model.SetCellPosition(newCell.CellPos);

            tasks.Add(newBubble.SmoothMove(newCell.Position));
            
            foreach (var bubble in bubbleList)
            {
                if (bubble.Model.Index + 1 >= spawnPath.Count)
                    continue;

                if (spawnPath[bubble.Model.Index] == null)
                {
                    Logger.Error($"Path ¿¡·¯ : {bubble.Model.Index}");
                    continue;
                }

                bubble.Model.AddIndex();

                var cell = spawnPath[bubble.Model.Index];
                cell.SetBubble(bubble);
                bubble.Model.SetCellPosition(cell.CellPos);

                tasks.Add(bubble.SmoothMove(cell.Position));
            }

            await UniTask.WhenAll(tasks);

            bubbleList.Add(newBubble);
        }
    }
}