using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleBubbleLauncherModel : IBaseUnitModel
{
    public Func<BubbleHitInfo, BattleCell> OnFindClosestEmptyCell {  get; private set; }
    public Action<List<Vector3>, CellPosition, CellPosition> OnLaunch { get; private set; }
    public Color LineColor { get; private set; }

    public void SetOnFindClosestEmptyCell(Func<BubbleHitInfo, BattleCell> func)
    {
        OnFindClosestEmptyCell = func;
    }

    public void SetOnLaunch(Action<List<Vector3>, CellPosition, CellPosition> action)
    {
        OnLaunch = action;
    }

    public void SetLineColor(Color lineColor)
    {
        LineColor = lineColor;
    }
}