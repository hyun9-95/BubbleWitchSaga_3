using UnityEngine;

public class BattleBoss : BaseUnit<BattleBossModel>
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private float radius = 0.57f;

    [SerializeField]
    private int range = 1;

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    [ContextMenu("Draw HexOutline (Easy)")]
    public void DrawOutline()
    {
        if (!lineRenderer || radius <= 0f) return;

        float R = radius * 3f * range;

        Vector3 center = transform.position;

        lineRenderer.useWorldSpace = true; // 월드축 고정
        lineRenderer.loop = true;
        lineRenderer.positionCount = 6;

        float angleStep = 60 * Mathf.Deg2Rad;

        for (int i = 0; i < 6; i++)
        {
            float a = angleStep * i;   
            float x = center.x + R * Mathf.Cos(a);
            float y = center.y + R * Mathf.Sin(a);

            lineRenderer.SetPosition(i, new Vector2(x, y));
        }
    }
}
