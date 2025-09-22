using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    #region Property
    public int Rows => rows;
    public int BaseColumnsCount => baseColumns;
    public float BubbleRadius => bubbleRadius;
    public Vector3 CenterWorld => centerWorld;
    public bool IsReady => cells.Count > 0;
    #endregion

    #region Value
    [SerializeField]
    private int rows = 10;

    [SerializeField]
    private int baseColumns = 10;

    [SerializeField]
    private float bubbleRadius = 0.5f;

    [SerializeField]
    private List<CellPosition> closedCells;

    [SerializeField]
    public CellPosition testCellPos;

    [SerializeField]
    public List<CellPosition> testSpawnPath;

    [SerializeField]
    private bool showGizmos = true;

    private float totalWidth = 0;
    private float totalHeight = 0;
    private Vector3 centerWorld = Vector3.zero;
    #endregion

    private Dictionary<CellPosition, BattleCell> cells = new();

    public void BuildGrid()
    {
        GenerateCells();
    }

    private void GenerateCells()
    {
        cells.Clear();
        float width = bubbleRadius * 2f;
        float height = bubbleRadius * 2f;

        CalculateGridDimensions(width, height);

        for (int row = 0; row < rows; row++)
        {
            bool oddRow = (row % 2 == 1);
            int cols = oddRow ? baseColumns + 1 : baseColumns;

            for (int column = 0; column < cols; column++)
            {
                float x = column * width - (oddRow ? bubbleRadius : 0f);
                float y = -row * height;

                Vector3 worldPos = new Vector3(x, y, 0f);
                CellPosition gridPos = new CellPosition(row, column);

                var cell = new BattleCell(gridPos, worldPos);

                if (closedCells != null && closedCells.Contains(gridPos))
                    cell.SetClosed(true);

                cells[cell.GridPos] = cell;
            }
        }
    }

    private void CalculateGridDimensions(float width, float height)
    {
        // 홀수 행이 가장 넓음 (baseColumns + 1개 셀)
        totalWidth = width * (baseColumns + 1);
        totalHeight = rows * height;

        float centerX = (totalWidth / 2f) - width; 
        float centerY = -(totalHeight) / 2f;
        centerY += bubbleRadius;

        centerWorld = new Vector3(centerX, centerY, 0f);
    }


    public BattleCell GetCell(CellPosition cellPos)
    {
        if (cells.TryGetValue(cellPos, out var cell))
            return cell;

        return null;
    }

    public int GetColumnCountByRow(int row)
    {
        bool oddRow = (row % 2 == 1);
        return oddRow ? baseColumns + 1 : baseColumns;
    }

    public List<CellPosition> GetSpawnPath()
    {
        int spawnCount = 16;
        int maxColumnCount = 10;

        var direction = BattleCellDirection.Right;
        var spawnPath = new List<CellPosition>();
        spawnPath.Clear();
        int maxColumnSpawn = 4;

        CellPosition currentCellPos = testCellPos;
        bool movingRight = direction == BattleCellDirection.Right;

        int currentAddColumnCount = 0;
        int maxColumnIndex = maxColumnCount - 1;

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
                spawnPath.Add(currentCellPos);

                nextDirection = nextDirection == BattleCellDirection.BottomLeft ?
                    BattleCellDirection.BottomRight :
                    BattleCellDirection.BottomLeft;

                currentCellPos.Move(nextDirection);
                spawnPath.Add(currentCellPos);

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

                spawnPath.Add(currentCellPos);
                currentAddColumnCount++;
            }
        }

        return spawnPath;
    }

#if UNITY_EDITOR
    private void DrawCellGizmos()
    {
        foreach (var cell in cells.Values)
        {
            bool isInSpawnPath = testSpawnPath != null &&
                                testSpawnPath.Exists(pos => pos.row == cell.GridPos.row && pos.column == cell.GridPos.column);

            if (cell.GridPos.row == testCellPos.row && cell.GridPos.column == testCellPos.column)
            {
                Gizmos.color = Color.magenta;
            }
            else if (isInSpawnPath)
            {
                Gizmos.color = Color.magenta;
            }
            else
            {
                Gizmos.color = cell.Closed ? Color.red : Color.cyan;
            }

            Gizmos.DrawWireSphere(cell.Position, bubbleRadius);
        }
    }

    private void GeneratePreviewCells()
    {
        cells.Clear();
        GenerateCells();
    }

    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            GeneratePreviewCells();
            DrawCellGizmos();
            DrawGridBounds();
        }
    }

    private void DrawGridBounds()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centerWorld, new Vector3(totalWidth, totalHeight, 0f));
    }
#endif
}
