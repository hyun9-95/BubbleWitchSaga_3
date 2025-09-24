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
    public float DropPosY => bottomY + bubbleRadius;
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
    private float bottomY = 0;

    private Vector3 centerWorld = Vector3.zero;
    private BoundsInt gridBounds;

    BattleCellDirection[] neighborDirections = new BattleCellDirection[]
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

        bottomY = bottom;
    }

    public BattleCell GetCell(CellPosition cellPos)
    {
        if (cells.TryGetValue(cellPos, out var cell))
            return cell;

        return null;
    }

    public BattleCell GetDirectionCell(CellPosition rootPos, BattleCellDirection direction)
    {
        var neighborCellPos = GetCell(rootPos).CellPos;
        neighborCellPos.Move(direction);

        return GetCell(neighborCellPos);
    }

    public BattleCell GetBossCell()
    {
        return GetCell(new CellPosition(1, 5));
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

    public BattleCellDirection[] GetNeighborDirections()
    {
        return neighborDirections;
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
            DrawRootConnectionGizmos();
        }
    }

    private void DrawGridBounds()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centerWorld, new Vector3(totalWidth, totalHeight, 0f));
    }

    private void DrawRootConnectionGizmos()
    {
        if (!Application.isPlaying || !IsReady)
            return;

        foreach (var cellPair in cells)
        {
            var cell = cellPair.Value;

            // 버블이 있고 RootPos를 가진 경우
            if (!cell.IsEmpty && cell.Bubble != null && !cell.Bubble.Model.RootPos.IsEmpty)
            {
                var childPos = cell.Position;
                var rootCell = GetCell(cell.Bubble.Model.RootPos);

                if (rootCell != null)
                {
                    var rootPos = rootCell.Position;

                    // 루트에서 자식으로 파란색 선 그리기
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(rootPos, childPos);

                    // 화살표 표시 (자식 쪽에)
                    Vector3 direction = (childPos - rootPos).normalized;
                    float arrowSize = bubbleRadius * 0.3f;

                    Vector3 arrowHead = childPos - direction * arrowSize;
                    Vector3 right = Vector3.Cross(direction, Vector3.forward) * arrowSize * 0.5f;

                    Gizmos.DrawLine(childPos, arrowHead + right);
                    Gizmos.DrawLine(childPos, arrowHead - right);
                }
            }
        }
    }
#endif
}
