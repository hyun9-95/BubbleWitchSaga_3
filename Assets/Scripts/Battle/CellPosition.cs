using System;
using UnityEngine;

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

    public bool IsOdd()
    {
        return (row % 2 == 1);
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
