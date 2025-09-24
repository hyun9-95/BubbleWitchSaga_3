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
    public BoundsInt GridBounds => gridBounds;
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
    private EdgeCollider2D leftWall;

    [SerializeField]
    private EdgeCollider2D rightWall;

    [SerializeField]
    private bool showGizmos = true;

    private float totalWidth = 0;
    private float totalHeight = 0;
    private Vector3 centerWorld = Vector3.zero;
    private BoundsInt gridBounds;

    BattleCellDirection[] battleCellDirection = new BattleCellDirection[]
    {
                BattleCellDirection.Left,
                BattleCellDirection.TopLeft,
                BattleCellDirection.TopRight,
                BattleCellDirection.Right,
                BattleCellDirection.BottomLeft,
                BattleCellDirection.BottomRight
    };

    // 윗방향 제외
    BattleCellDirection[] launchDirection = new BattleCellDirection[]
    {
                BattleCellDirection.Left,
                BattleCellDirection.Right,
                BattleCellDirection.BottomLeft,
                BattleCellDirection.BottomRight
    };
    #endregion

    private Dictionary<CellPosition, BattleCell> cells = new();

    private void Awake()
    {
        GenerateCells();
    }

    private void GenerateCells()
    {
        cells.Clear();
        float width = bubbleRadius * 2f;
        float height = bubbleRadius * Mathf.Sqrt(3f);

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

                cells[cell.CellPos] = cell;
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

        float left = centerWorld.x - totalWidth * 0.5f;
        float right = centerWorld.x + totalWidth * 0.5f;
        float bottom = centerWorld.y - totalHeight * 0.5f;
        float top = centerWorld.y + totalHeight * 0.5f;

        leftWall.points = new Vector2[]
        {
            new(left, bottom),
            new(left, top)
        };

        rightWall.points = new Vector2[]
        {
            new(right, bottom),
            new(right, top),
        };
    }

    public BattleCell GetCell(CellPosition cellPos)
    {
        if (cells.TryGetValue(cellPos, out var cell))
            return cell;

        return null;
    }

    public Dictionary<CellPosition, BattleCell> GetAllCells()
    {
        return cells;
    }

    public int GetColumnCountByRow(int row)
    {
        bool oddRow = (row % 2 == 1);
        return oddRow ? baseColumns + 1 : baseColumns;
    }

    public BattleCell GetClosestEmptyCell(BubbleHitInfo hitInfo)
    {
        BattleCell hitCell = GetCell(hitInfo.HitCellPos);
        Vector2 hitCellWorldPos = hitCell.Position;

        BattleCell closestCell = null;
        float closestDistance = float.MaxValue;

        foreach (var direction in launchDirection)
        {
            var neighborCellPos = hitInfo.HitCellPos;
            neighborCellPos.Move(direction);

            var neighborCell = GetCell(neighborCellPos);

            if (neighborCell == null || !neighborCell.IsEmpty || neighborCell.Closed)
                continue;

            float distance = Vector2.Distance(hitInfo.HitPoint, neighborCell.Position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCell = neighborCell;
            }
        }

        return closestCell;
    }

    public void AddToClosedCell(CellPosition cellPos)
    {
        closedCells.Add(cellPos);
    }

    public List<CellPosition> GetNeighborPositions(CellPosition cellPos)
    {
        var neighborPositions = new List<CellPosition>();

        foreach (var direction in battleCellDirection)
        {
            var cell = GetCell(cellPos);

            if (cell.IsEmpty)
                continue;

            var neighborCellPos = cellPos;
            neighborCellPos.Move(direction);
            neighborPositions.Add(neighborCellPos);
        }

        return neighborPositions;
    }

#if UNITY_EDITOR
    private void DrawCellGizmos()
    {
        foreach (var cell in cells.Values)
        {
            bool isInSpawnPath = testSpawnPath != null &&
                                testSpawnPath.Exists(pos => pos.row == cell.CellPos.row && pos.column == cell.CellPos.column);

            if (cell.CellPos.row == testCellPos.row && cell.CellPos.column == testCellPos.column)
            {
                Gizmos.color = Color.magenta;
            }
            else if (isInSpawnPath)
            {
                Gizmos.color = Color.magenta;
            }
            else if (closedCells.Contains(cell.CellPos))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = !cell.IsEmpty ? Color.yellow : Color.cyan;
            }

            Gizmos.DrawWireSphere(cell.Position, bubbleRadius);
        }
    }

    private void GeneratePreviewCells()
    {
        if (cells != null && Application.isPlaying)
            return;

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
