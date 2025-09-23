using UnityEngine;

public class BattleBubbleLauncherModel : IBaseUnitModel
{
    public BoundsInt WallBounds { get; private set; }

    public void SetWallBounds(BoundsInt boundsInt)
    {
        WallBounds = boundsInt;
    }
}