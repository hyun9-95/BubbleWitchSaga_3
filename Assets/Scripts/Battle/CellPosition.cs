using System;

[Serializable]
public struct CellPosition
{
    public int row;
    public int column;
    public CellPosition(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public bool IsEmpty => row == 0 && column == 0;

    public bool IsOdd()
    {
        return (row % 2 == 1);
    }

    public bool Equals(CellPosition other)
    {
        return other.row == row && other.column == column;
    }
    
    public void Move(BattleCellDirection direction)
    {
        switch (direction)
        {
            case BattleCellDirection.Left:
                column--;
                break;

            case BattleCellDirection.TopLeft:
                if (IsOdd())
                {
                    row--;
                    column--;
                }
                else
                {
                    row--;
                }
                break;

            case BattleCellDirection.TopRight:
                if (IsOdd())
                {
                    row--;
                }
                else
                {
                    row--;
                    column++;
                }
                break;

            case BattleCellDirection.Right:
                column++;
                break;

            case BattleCellDirection.BottomLeft:
                if (IsOdd())
                {
                    row++;
                    column--;
                }
                else
                {
                    row++;
                }
                break;

            case BattleCellDirection.BottomRight:
                if (IsOdd())
                {
                    row++;
                }
                else
                {
                    row++;
                    column++;
                }
                break;
        }
    }
}
