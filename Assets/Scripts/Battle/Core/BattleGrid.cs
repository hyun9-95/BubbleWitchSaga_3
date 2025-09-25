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
    private bool showGizmos = true;

    [SerializeField]
    private int rows = 10;

    [SerializeField]
    private int baseColumns = 10;

    [SerializeField]
    private float bubbleRadius = 0.5f;

    [SerializeField]
    private EdgeCollider2D leftWall;

    [SerializeField]
    private EdgeCollider2D rightWall;

    [SerializeField]
    private int startTriggerRow = 10;

    [SerializeField]
    private int bossRow = 1;

    [SerializeField]
    private int bossColumn = 5;

    private float totalWidth = 0;
    private float totalHeight = 0;
    private float bottomY = 0;
    private int triggerRow;

    private Vector3 centerWorld = Vector3.zero;
    private BoundsInt gridBounds;

    private Dictionary<CellPosition, BattleCell> cells = new();

    private List<int> rowCounts;
    private int currentMaxRow = -1;

    private float cellWidth;
    private float cellHeight;

    #region Directions
    private BattleCellDirection[] neighborDirections = new BattleCellDirection[]
    {
                BattleCellDirection.Left,
                BattleCellDirection.TopLeft,
                BattleCellDirection.TopRight,
                BattleCellDirection.Right,
                BattleCellDirection.BottomLeft,
                BattleCellDirection.BottomRight
    };

    // 윗방향 제외
    private BattleCellDirection[] launchDirection = new BattleCellDirection[]
    {
                BattleCellDirection.Left,
                BattleCellDirection.Right,
                BattleCellDirection.BottomLeft,
                BattleCellDirection.BottomRight
    };
    #endregion
    #endregion


    #region Create Cell
    private void Awake()
    {
        triggerRow = startTriggerRow;

        cellWidth = bubbleRadius * 2f;
        cellHeight = bubbleRadius * Mathf.Sqrt(3f);

        GenerateCells();
    }

    private void GenerateCells()
    {
        cells.Clear();

        CalculateGridDimensions(cellWidth, cellHeight);

        rowCounts = new List<int>(rows);

        for (int row = 0; row < rows; row++)
            AddRow(row, cellWidth, cellHeight);
    }

    public void OnSetBubble(int cellRow)
    {
        rowCounts[cellRow]++;

        currentMaxRow = Mathf.Max(cellRow, currentMaxRow);
    }

    public void OnRemoveBubble(int cellRow)
    {
        rowCounts[cellRow]--;

        if (cellRow == currentMaxRow && rowCounts[cellRow] == 0)
        {
            while (currentMaxRow >= 0 && rowCounts[currentMaxRow] == 0)
                currentMaxRow--;
        }
    }

    private void AddRow()
    {
        AddRow(rows, cellWidth, cellHeight);

        rows++;
        CalculateGridDimensions(cellWidth, cellHeight);
    }

    private void AddRow(int newRow, float width, float height)
    {
        bool oddRow = (newRow % 2 == 1);
        int columnCount = oddRow ? baseColumns + 1 : baseColumns;

        for (int column = 0; column < columnCount; column++)
        {
            float x = column * width - (oddRow ? bubbleRadius : 0f);
            float y = -newRow * height;

            Vector3 worldPos = new(x, y, 0f);
            CellPosition gridPos = new(newRow, column);

            var cell = new BattleCell(gridPos, worldPos, OnSetBubble, OnRemoveBubble);
            cells[cell.CellPos] = cell;
        }

        rowCounts.Add(0);
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
    #endregion

    public BattleCell GetCell(CellPosition cellPos)
    {
        if (cells.TryGetValue(cellPos, out var cell))
            return cell;

        return null;
    }

    public BattleCell GetDirectionCell(CellPosition rootPos, BattleCellDirection direction)
    {
        var neighborCell = GetCell(rootPos);

        if (neighborCell == null)
            return null;

        var neighborCellPos = neighborCell.CellPos;
        neighborCellPos.Move(direction);

        return GetCell(neighborCellPos);
    }

    public BattleCell GetBossCell()
    {
        return GetCell(new CellPosition(bossRow, bossColumn));
    }

    public BattleCell GetClosestEmptyCell(BubbleHitInfo hitInfo)
    {
        BattleCell hitCell = GetCell(hitInfo.HitCellPos);

        BattleCell closestCell = null;
        float closestDistance = float.MaxValue;

        foreach (var direction in launchDirection)
        {
            var neighborCellPos = hitInfo.HitCellPos;
            neighborCellPos.Move(direction);

            var neighborCell = GetCell(neighborCellPos);

            if (neighborCell == null || !neighborCell.IsEmpty)
                continue;

            float distance = Vector2.Distance(hitInfo.HitPoint, neighborCell.WorldPos);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCell = neighborCell;
            }
        }

        return closestCell;
    }

    public BattleCellDirection[] GetNeighborDirections()
    {
        return neighborDirections;
    }

    public float GetHeight(int row)
    {
        return cellHeight * row;
    }

    public bool CheckScrollDown()
    {
        return currentMaxRow > triggerRow;
    }

    public bool CheckScrollUp()
    {
        if (triggerRow <= startTriggerRow)
            return false;

        return currentMaxRow < triggerRow;
    }

    // 한줄 스크롤
    public float GetScrollDownHeight()
    {
        AddRow();
        triggerRow++;
        return GetHeight(1);
    }

    // 줄어든 만큼 다시 스크롤
    public float GetScrollUpHeight()
    {
        int prevTriggerRow = triggerRow;

        triggerRow = Mathf.Max(currentMaxRow, startTriggerRow);

        int scrollRows = prevTriggerRow - triggerRow;
        return GetHeight(scrollRows);
    }

#if UNITY_EDITOR
    private void GeneratePreviewCells()
    {
        if (Application.isPlaying)
            return;

        GenerateCells();
    }

    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            if (cells == null)
                GeneratePreviewCells();

            DrawCellGizmos();
            DrawGridBounds();
            DrawRootConnectionGizmos();
        }
    }
    private void DrawCellGizmos()
    {
        foreach (var cell in cells.Values)
        {
            Gizmos.color = !cell.IsEmpty ? Color.yellow : Color.cyan;
            Gizmos.DrawWireSphere(cell.WorldPos, bubbleRadius);
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
                var childPos = cell.WorldPos;
                var rootCell = GetCell(cell.Bubble.Model.RootPos);

                if (rootCell != null)
                {
                    var rootPos = rootCell.WorldPos;

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
