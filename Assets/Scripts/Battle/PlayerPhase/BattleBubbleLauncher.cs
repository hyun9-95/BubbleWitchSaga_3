using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleBubbleLauncher : BaseUnit<BattleBubbleLauncherModel>, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Property
    #endregion

    #region Value
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
    private List<Vector3> aimPoints;
    private List<Vector3> movePath;

    private BubbleNode guideBubbleNode;
    private BattleCell selectedCell;
    private CellPosition hitCellPos;
    private float bubbleRadius;

    private Vector2 disablePos = new Vector2(-500, 0);
    private bool aiming = false;
    private Color lineColor = Color.white;
    private int bubbleMask;
    #endregion

    #region Function
    public async UniTask Initialize()
    {
        worldCamera = CameraManager.Instance.GetWorldCamera();

        aimPoints = new List<Vector3>(maxHit * 2);
        movePath = new List<Vector3>(maxHit * 2);
        bubbleMask = (int)LayerFlag.Bubble;

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

    private void UpdateAimDirection(Vector2 screenPosition)
    {
        Vector2 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        Vector2 dir = (worldPosition - startPos).normalized;

        // 부채꼴 각도로 제한
        var dotValue = Vector2.Dot(dir, Vector2.up);

        if (dotValue < launchAngle)
        {
            aiming = false;
            Cancel();
            return;
        }

        // 관련 변수 초기화
        aiming = true;
        selectedCell = null;
        hitCellPos = default;
        aimPoints.Clear();
        aimPoints.Add(startPos);

        // Ray로 버블 찾기
        FindBubbleByRay(dir);

        if (lineRenderer.positionCount != aimPoints.Count)
            lineRenderer.positionCount = aimPoints.Count;

        for (int i = 0; i < aimPoints.Count; i++)
            lineRenderer.SetPosition(i, aimPoints[i]);
    }

    private void FindBubbleByRay(Vector2 lauhchDir)
    {
        float remainDistance = maxRange;
        bool foundCell = false;
        Vector2 origin = startPos;

        for (int i = 0; i < maxHit && remainDistance > 0; i++)
        {
            RaycastHit2D hit = Physics2D.CircleCast(origin, bubbleRadius, lauhchDir, remainDistance, bubbleMask);

            if (hit.collider == null)
            {
                aimPoints.Add(origin + lauhchDir * remainDistance);
                break;
            }

            Vector2 centerHitPoint = origin + lauhchDir * hit.distance;
            remainDistance -= hit.distance;

            if (hit.collider.CompareTag(StringDefine.TAG_WALL))
            {
                aimPoints.Add(centerHitPoint);

                Vector2 reflectDir = Vector2.Reflect(lauhchDir, hit.normal).normalized;

                // 재충돌 방지를 위해 조금 띄움
                origin = centerHitPoint + reflectDir * reflectOffset;

                lauhchDir = reflectDir;
                continue;
            }

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
                break;
            }
        }

        // 찾았다면 가이드버블 표시
        guideBubbleNode.gameObject.SafeSetActive(foundCell);
    }

    private void LaunchBubble()
    {
        lineRenderer.positionCount = 0;
        guideBubbleNode.SetPosition(disablePos);

        movePath.Clear();

        for (int i = 0; i < aimPoints.Count - 1; i++)
            movePath.Add(aimPoints[i]);

        movePath.Add(endPos);

        // 최종 발사
        Model.OnLaunch(movePath, hitCellPos, selectedCell.CellPos);
    }

    #region Input
    public void OnPointerDown(PointerEventData eventData)
    {
        // RingSlot 회전시 라인컬러가 바뀐다.
        if (lineColor != Model.LineColor)
        {
            lineColor = Model.LineColor;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = lineColor;
        }

        // 카메라 스크롤 고려해서 누를때마다 갱신
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

        LaunchBubble();
    }

    private void Cancel()
    {
        lineRenderer.positionCount = 0;
        guideBubbleNode.SetPosition(disablePos);
    }
    #endregion
    #endregion
}