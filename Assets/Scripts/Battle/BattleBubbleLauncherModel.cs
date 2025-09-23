using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleBubbleLauncherModel : IBaseUnitModel
{
    public Func<CellPosition, Vector2, BattleCell> OnFindClosestEmptyCell {  get; private set; }
    public Action<List<Vector3>> OnLaunch { get; private set; }

    public void SetOnFindClosestEmptyCell(Func<CellPosition, Vector2, BattleCell> func)
    {
        OnFindClosestEmptyCell = func;
    }

    public void SetOnLaunch(Action<List<Vector3>> action)
    {
        OnLaunch = action;
    }
}