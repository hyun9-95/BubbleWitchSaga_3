using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleBubbleLauncher : BaseUnit<BattleBubbleLauncherModel>, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform launchTr;

    private LineRenderer lineRenderer;
    private Vector2 startPos;
    private Camera worldCamera;
    private BattleGrid battleGrid;
    private List<Vector3> aimPoints = new List<Vector3>(4);

    public async UniTask Initialize()
    {
        worldCamera = CameraManager.Instance.GetWorldCamera();

        if (lineRenderer == null)
            await InstantiateLineRenderer();
    }

    private async UniTask InstantiateLineRenderer()
    {
        var lineRendererGo = await AddressableManager.Instance.InstantiateAsync(PathDefine.BUBBLE_LINE_RENDERER);

        if (lineRendererGo == null)
            return;

        lineRenderer = lineRendererGo.GetComponent<LineRenderer>();
        startPos = CameraManager.Instance.GetUIWorldPos(launchTr.position);
        battleGrid = FindAnyObjectByType<BattleGrid>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateAimDirection(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateAimDirection(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lineRenderer.positionCount = 0;
    }

    public void OnUpdate()
    {
        

    }

    private void UpdateAimDirection(Vector3 screenPosition)
    {
        Vector2 worldPosition =
            worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));

        Vector2 dir = (worldPosition - startPos).normalized;
        float remainDistance = 999f;

        // 벽 1회 + 버블 1회 가능
        int maxHit = 2; 
        aimPoints.Clear();
        aimPoints.Add(startPos);

        Vector2 origin = startPos;

        for (int i = 0; i < maxHit; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, remainDistance, (int)LayerFlag.World);

            if (hit.collider == null)
            {
                // 맞은게 없다면 직선 경로
                aimPoints.Add(origin + dir * remainDistance);
                break;
            }

            aimPoints.Add(hit.point);
            remainDistance -= hit.distance;

            if (hit.collider.CompareTag(StringDefine.TAG_BUBBLE))
            {
                if (hit.collider.TryGetComponent<BubbleNode>(out var bubble))
                {
                    var cell = battleGrid.GetCell(bubble.Model.CellPosition);
                    var closest = battleGrid.GetClosestEmptyCell(cell.CellPos, hit.point);
                }

                break;
            }

            bool hitWall = hit.collider.CompareTag(StringDefine.TAG_WALL);

            if (hitWall)
            {
                dir = Vector2.Reflect(dir, hit.normal).normalized;
                origin = hit.point + dir * 0.01f;
            }
        }

        lineRenderer.positionCount = aimPoints.Count;

        for (int i = 0; i < aimPoints.Count; i++)
            lineRenderer.SetPosition(i, aimPoints[i]);
    }
}