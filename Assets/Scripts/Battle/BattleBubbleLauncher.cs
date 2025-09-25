using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class BattleBubbleLauncher : BaseUnit<BattleBubbleLauncherModel>, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform launchTr;

    [SerializeField]
    private int maxHit = 2;

    [SerializeField]
    private float reflectOffset = 0.01f;

    [SerializeField]
    private float launchAngle = 0.2f;

    [SerializeField]
    private float maxRange = 100f;

    private LineRenderer lineRenderer;
    private Vector2 startPos;
    private Vector3 endPos;
    private Camera worldCamera;
    private List<Vector3> aimPoints = new List<Vector3>(4);
    private List<Vector3> movePath = new List<Vector3>(4);

    private BubbleNode guideBubbleNode;
    private BattleCell selectedCell;
    private CellPosition hitCellPos;
    private float bubbleRadius;


    private Vector2 disablePos = new Vector2(0, -500);
    private bool aiming = false;
    private Color lineColor = Color.white;
    private int worldMask;

    public async UniTask Initialize()
    {
        worldCamera = CameraManager.Instance.GetWorldCamera();

        aimPoints = new List<Vector3>(maxHit * 2);
        movePath = new List<Vector3>(maxHit * 2);
        worldMask = (int)LayerFlag.World;

        if (lineRenderer == null)
            await InstantiateLineRenderer();

        if (guideBubbleNode == null)
        {
            guideBubbleNode = await BubbleFactory.Instance.CreateNewBubble(BubbleType.Empty);
            guideBubbleNode.SetPosition(disablePos);
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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (lineColor != Model.LineColor)
        {
            lineColor = Model.LineColor;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = lineColor;
        }

        startPos = CameraManager.Instance.GetUIWorldPos(launchTr.position);
        UpdateAimDirection(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateAimDirection(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!aiming || selectedCell == null)
            return;

        lineRenderer.positionCount = 0;
        guideBubbleNode.SetPosition(disablePos);

        movePath.Clear();

        for (int i = 0; i < aimPoints.Count - 1; i++)
            movePath.Add(aimPoints[i]);

        movePath.Add(endPos);

        Model.OnLaunch(movePath, hitCellPos, selectedCell.CellPos);
    }

    private void Cancel()
    {
        lineRenderer.positionCount = 0;
        guideBubbleNode.SetPosition(disablePos);
    }

    private void UpdateAimDirection(Vector2 screenPosition)
    {
        Vector2 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        Vector2 dir = (worldPosition - startPos).normalized;

        var dotValue = Vector2.Dot(dir, Vector2.up);

        if (dotValue < launchAngle)
        {
            aiming = false;
            Cancel();
            return;
        }

        aiming = true;

        float remainDistance = maxRange;
        bool foundCell = false;
        selectedCell = null;
        hitCellPos = default;

        aimPoints.Clear();
        aimPoints.Add(startPos);

        Vector2 origin = startPos;

        for (int i = 0; i < maxHit && remainDistance > 1e-3f; i++)
        {
            RaycastHit2D hit = Physics2D.CircleCast(origin, bubbleRadius, dir, remainDistance, worldMask);

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
                    hitCellPos = bubble.Model.CellPos;
                    var hitInfo = new BubbleHitInfo(hitCellPos, centerHitPoint, startPos);
                    selectedCell = Model.OnFindClosestEmptyCell(hitInfo);
            
                    if (selectedCell != null)
                    {
                        guideBubbleNode.SetPosition(selectedCell.WorldPos);
                        aimPoints.Add(hit.point);
                        endPos = selectedCell.WorldPos;
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
                origin = centerHitPoint + reflectDir * reflectOffset;

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