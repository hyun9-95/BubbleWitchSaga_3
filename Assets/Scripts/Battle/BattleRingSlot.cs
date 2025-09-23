using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class BattleRingSlot : BaseUnit<BattleRingSlotModel>, IPointerDownHandler
{
    #region Property
    public BubbleNode CurrentBubble => bubbleNodes != null && bubbleNodes.Count > 0 ?
        bubbleNodes[0] : null;

    #endregion
    [SerializeField]
    private RectTransform launchTr;

    [SerializeField]
    private RectTransform slotRectTransform;

    [SerializeField]
    private TextMeshProUGUI remainBubbleText;

    [SerializeField]
    private float rotateTime = 0.5f;

    [SerializeField]
    private float launchSpeed = 10f;

    private float radius;
    private float angleStep;
    private bool isRotating = false;

    private List<BubbleNode> bubbleNodes;

    public async UniTask InitializeSlot()
    {
        bubbleNodes = new List<BubbleNode>(Model.SlotCount);
        angleStep = 360f / Model.SlotCount;

        radius = Vector3.Distance(slotRectTransform.position, launchTr.position);

        for (int i = 0; i < Model.SlotCount; i++)
        {
            float angle = i * angleStep;

            BubbleNode newBubble = await SpawnNewBubble();
            newBubble.SetPosition(GetPositionFromAngle(angle));
            bubbleNodes.Add(newBubble);
        }

        remainBubbleText.SafeSetText(Model.RemainBubbleCount.ToString());
    }

    public void Refresh()
    {

    }

    private Vector3 GetPositionFromAngle(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float x = Mathf.Sin(radian) * radius;
        float y = Mathf.Cos(radian) * radius;

        Vector3 uiPosition = transform.position + new Vector3(x, y, 0);

        var angleWorldPos = CameraManager.Instance.GetUIWorldPos(uiPosition);

        return angleWorldPos;
    }

    public async UniTask RotateSlot()
    {
        if (isRotating || bubbleNodes == null || bubbleNodes.Count == 0)
            return;

        isRotating = true;

        float targetRotation = angleStep;
        float elapsedTime = 0f;
        float duration = rotateTime;

        float[] startAngles = new float[bubbleNodes.Count];

        for (int i = 0; i < bubbleNodes.Count; i++)
        {
            startAngles[i] = i * angleStep;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            float currentRotation = Mathf.Lerp(0f, targetRotation, progress);

            for (int i = 0; i < bubbleNodes.Count; i++)
            {
                if (bubbleNodes[i] == null)
                    continue;

                float currentAngle = startAngles[i] + currentRotation;
                Vector3 newPos = GetPositionFromAngle(currentAngle);
                bubbleNodes[i].SetPosition(newPos);
            }

            await UniTask.NextFrame();
        }

        for (int i = 0; i < bubbleNodes.Count; i++)
        {
            float finalAngle = startAngles[i] + targetRotation;
            bubbleNodes[i].SetPosition(GetPositionFromAngle(finalAngle));
        }

        BubbleNode firstBubble = bubbleNodes[0];
        bubbleNodes.RemoveAt(0);

        if (firstBubble != null)
            bubbleNodes.Add(firstBubble);

        isRotating = false;
    }

    private async UniTask<BubbleNode> SpawnNewBubble()
    {
        var newBubble = await BubbleFactory.Instance.CreateNewBubble();
        newBubble.SetColliderEnable(false);
        newBubble.Model.SetMoveSpeed(launchSpeed);

        return newBubble;
    }

    public async UniTask RefillBubble()
    {
        if (CurrentBubble != null)
            return;

        await RotateSlot();

        float angle = (Model.SlotCount - 1) * angleStep;
        BubbleNode newBubble = await SpawnNewBubble();
        newBubble.SetPosition(GetPositionFromAngle(angle));
        bubbleNodes[Model.SlotCount - 1] = newBubble;

        Model.ReduceSpawnCount();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isRotating)
            return;

        RotateSlot().Forget();
    }
}
