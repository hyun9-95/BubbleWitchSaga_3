using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleRingSlot : BaseUnit<BattleRingSlotModel>, IPointerDownHandler
{
    #region Property
    private BubbleNode CurrentBubble => Model.BubbleNodes != null && Model.BubbleNodes.Count > 0 ?
        Model.BubbleNodes[0] : null;
    #endregion

    #region Value
    [SerializeField]
    private RectTransform launchTr;

    [SerializeField]
    private RectTransform slotRectTransform;

    [SerializeField]
    private TextMeshProUGUI remainBubbleText;

    [SerializeField]
    private float rotateTime = 0.5f;

    private float radius;
    private float angleStep;
    private bool isRotating = false;

    #endregion

    #region Function
    public async UniTask InitializeSlot()
    {
        angleStep = 360f / Model.SlotCount;

        radius = Vector3.Distance(slotRectTransform.position, launchTr.position);

        for (int i = 0; i < Model.SlotCount; i++)
        {
            float angle = i * angleStep;

            BubbleNode newBubble = await SpawnNewBubble();
            newBubble.SetPosition(GetPositionFromAngle(angle));
            Model.BubbleNodes.Add(newBubble);
        }

        remainBubbleText.SafeSetText(Model.RemainBubbleCount.ToString());
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
        var bubbleNodes = Model.BubbleNodes;

        if (isRotating || bubbleNodes == null || bubbleNodes.Count == 0)
            return;

        isRotating = true;

        float targetRotation = angleStep;
        float elapsedTime = 0f;
        float duration = rotateTime;

        float[] startAngles = new float[bubbleNodes.Count];

        for (int i = 0; i < bubbleNodes.Count; i++)
            startAngles[i] = i * angleStep;

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

            if (bubbleNodes[i] != null)
                bubbleNodes[i].SetPosition(GetPositionFromAngle(finalAngle));
        }

        BubbleNode firstBubble = bubbleNodes[0];
        bubbleNodes.RemoveAt(0);

        if (firstBubble != null)
            bubbleNodes.Add(firstBubble);

        if (CurrentBubble != null)
            Model.OnChangeBubbleColor?.Invoke(CurrentBubble.Model.BubbleColor);

        isRotating = false;
    }

    private async UniTask<BubbleNode> SpawnNewBubble()
    {
        var newBubble = await BubbleFactory.Instance.CreateNewBubble(BubbleType.Normal);
        newBubble.SetColliderEnable(false);
        newBubble.Model.SetMoveSpeed(FloatDefine.BATTLE_BUBBLE_LAUNCH_SPEED);

        return newBubble;
    }

    public async UniTask RefillBubble()
    {
        if (CurrentBubble != null)
        {
            Model.OnChangeBubbleColor?.Invoke(CurrentBubble.Model.BubbleColor);
            return;
        }

        await RotateSlot();

        if (Model.RemainBubbleCount > 1)
        {
            float angle = (Model.SlotCount - 1) * angleStep;
            BubbleNode newBubble = await SpawnNewBubble();
            newBubble.SetPosition(GetPositionFromAngle(angle));
            Model.BubbleNodes.Add(newBubble);
        }
    }

    public void RefreshBubbleCount()
    {
        remainBubbleText.SafeSetText(Model.RemainBubbleCount.ToString());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isRotating)
            return;

        RotateSlot().Forget();
    }
    #endregion
}
