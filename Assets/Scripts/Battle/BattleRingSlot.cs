using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleRingSlot : BaseUnit<BattleRingSlotModel>
{
    #region Property
    public BubbleNode CurrentBubble => uiBubbleNodes != null && uiBubbleNodes.Count > 0 ?
        uiBubbleNodes[0] : null;

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
    private float radius = 0.8f;

    private float angleStep;
    private bool isRotating = false;

    private List<BubbleNode> uiBubbleNodes;

    public async UniTask InitializeSlot()
    {
        uiBubbleNodes = new List<BubbleNode>(Model.SlotCount);
        angleStep = 360f / Model.SlotCount;

        radius = Vector3.Distance(slotRectTransform.position, launchTr.position);

        for (int i = 0; i < Model.SlotCount; i++)
        {
            float angle = i * angleStep;

            BubbleNode newBubble = await GetNewBubble();
            newBubble.SetPosition(GetPositionFromAngle(angle));
            uiBubbleNodes.Add(newBubble);
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
        if (isRotating || uiBubbleNodes == null || uiBubbleNodes.Count == 0)
            return;

        isRotating = true;

        float targetRotation = angleStep;
        float elapsedTime = 0f;
        float duration = rotateTime;

        Vector3[] startPositions = new Vector3[uiBubbleNodes.Count];
        Vector3[] targetPositions = new Vector3[uiBubbleNodes.Count];

        for (int i = 0; i < uiBubbleNodes.Count; i++)
        {
            if (uiBubbleNodes == null)
                continue;

            startPositions[i] = uiBubbleNodes[i].transform.position;
            float newAngle = (i * angleStep) + targetRotation;
            targetPositions[i] = GetPositionFromAngle(newAngle);
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            for (int i = 0; i < uiBubbleNodes.Count; i++)
            {
                if (uiBubbleNodes[i] == null)
                    continue;

                Vector3 newPos = Vector3.Lerp(startPositions[i], targetPositions[i], progress);
                uiBubbleNodes[i].SetPosition(newPos);
            }

            await UniTask.NextFrame();
        }

        for (int i = 0; i < uiBubbleNodes.Count; i++)
            uiBubbleNodes[i].SetPosition(targetPositions[i]);

        BubbleNode firstBubble = uiBubbleNodes[0];
        uiBubbleNodes.RemoveAt(0);

        if (firstBubble != null)
            uiBubbleNodes.Add(firstBubble);

        isRotating = false;
    }

    public BubbleNode GetCurrentBubble()
    {
        return CurrentBubble;
    }

    private async UniTask<BubbleNode> GetNewBubble()
    {
        var newBubble = await ObjectPoolManager.Instance.SpawnPoolableMono<BubbleNode>(PathDefine.BUBBLE_NODE_PATH);
        var bubbleNodeModel = new BubbleNodeModel();
        newBubble.SetModel(bubbleNodeModel);

        return newBubble;
    }

    public async UniTask RefillBubble()
    {
        if (CurrentBubble != null)
            return;

        await RotateSlot();

        float angle = (Model.SlotCount - 1) * angleStep;
        BubbleNode newBubble = await GetNewBubble();
        newBubble.SetPosition(GetPositionFromAngle(angle));
        uiBubbleNodes[Model.SlotCount - 1] = newBubble;
    }
}
