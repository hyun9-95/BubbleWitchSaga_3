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
    private Vector3 endPos;
    private Camera worldCamera;
    private List<Vector3> aimPoints = new List<Vector3>(4);
    private List<Vector3> movePath = new List<Vector3>(4);

    private BubbleNode guideBubbleNode;
    private BattleCell selectedCell;
    private float bubbleRadius;

    public async UniTask Initialize()
    {
        worldCamera = CameraManager.Instance.GetWorldCamera();

        if (lineRenderer == null)
            await InstantiateLineRenderer();

        if (guideBubbleNode == null)
        {
            guideBubbleNode = await BubbleFactory.Instance.CreateNewBubble(BubbleType.Guide);
            guideBubbleNode.gameObject.SetActive(false);
            guideBubbleNode.SetColliderEnable(false);

            bubbleRadius = guideBubbleNode.Radius * 0.5f;
        }
    }

    private async UniTask InstantiateLineRenderer()
    {
        var lineRendererGo = await AddressableManager.Instance.InstantiateAsync(PathDefine.BUBBLE_LINE_RENDERER);

        if (lineRendererGo == null)
            return;

        lineRenderer = lineRendererGo.GetComponent<LineRenderer>();
        startPos = CameraManager.Instance.GetUIWorldPos(launchTr.position);
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
        guideBubbleNode.gameObject.SafeSetActive(false);

        movePath.Clear();

        for (int i = 0; i < aimPoints.Count - 1; i++)
            movePath.Add(aimPoints[i]);

        movePath.Add(endPos);

        Model.OnLaunch(movePath);
    }

    public void OnUpdate()
    {
        

    }

    private void UpdateAimDirection(Vector2 screenPosition)
    {
        Vector2 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        Vector2 dir = (worldPosition - startPos).normalized;

        float maxRange = 999f;     
        float offset = 0.01f;        
        int maxHit = 2;                   
        float remainDistance = maxRange;
        int mask = (int)LayerFlag.World;
        bool foundCell = false;
        selectedCell = null;

        aimPoints.Clear();
        aimPoints.Add(startPos);

        Vector2 origin = startPos;

        for (int i = 0; i < maxHit && remainDistance > 1e-3f; i++)
        {
            RaycastHit2D hit = Physics2D.CircleCast(origin, bubbleRadius, dir, remainDistance, mask);

            if (hit.collider == null)
            {
                aimPoints.Add(origin + dir * remainDistance);
                break;
            }

            Vector2 centerHitPoint = origin + dir * hit.distance;
            remainDistance -= hit.distance;

            if (hit.collider.CompareTag(StringDefine.TAG_BUBBLE))
            {
                if (hit.collider.TryGetComponent<BubbleNode>(out var bubble))
                {
                    selectedCell = Model.OnFindClosestEmptyCell(bubble.Model.CellPosition, centerHitPoint);
            
                    if (selectedCell != null)
                    {
                        guideBubbleNode.SetPosition(selectedCell.Position);
                        aimPoints.Add(hit.point);
                        endPos = selectedCell.Position;
                        foundCell = true;
                    }
                }
                break;
            }

            if (hit.collider.CompareTag(StringDefine.TAG_WALL))
            {
                aimPoints.Add(centerHitPoint);

                Vector2 reflectDir = Vector2.Reflect(dir, hit.normal).normalized;

                // 재충돌 방지를 위해 조금 띄움
                origin = centerHitPoint + reflectDir * offset;

                dir = reflectDir;
                continue;
            }
        }

        lineRenderer.positionCount = aimPoints.Count;

        for (int i = 0; i < aimPoints.Count; i++)
            lineRenderer.SetPosition(i, aimPoints[i]);

        guideBubbleNode.gameObject.SafeSetActive(foundCell);
    }

}