using UnityEngine;

public readonly struct BubbleHitInfo
{
    public BubbleHitInfo(CellPosition hitCellPos, Vector2 hitPoint, Vector2 originPoint)
    {
        HitCellPos = hitCellPos;
        HitPoint = hitPoint;
        LaunchPos = originPoint;
    }

    public readonly CellPosition HitCellPos;
    public readonly Vector2 HitPoint;
    public readonly Vector2 LaunchPos;
}
