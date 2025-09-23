using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleNode : PoolableBaseUnit<BubbleNodeModel>
{
    [SerializeField]
    private Collider2D bubbleCol;

    [SerializeField]
    private SpriteRenderer bubbleSprite;

    public override async UniTask ShowAsync()
    {
        await ResolveBubbleImage();
    }

    public void SetColliderEnable(bool enable)
    {
        bubbleCol.enabled = enable;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public async UniTask SmoothMove(Vector3 pos)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, pos);
        float duration = distance / Model.MoveSpeed;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPos, pos, progress);
            await UniTaskUtils.NextFrame();
        }

        transform.position = pos;
    }

    public async UniTask MoveAlongPath(List<Vector3> posList)
    {
        if (posList == null || posList.Count == 0)
            return;

        for (int i = 0; i < posList.Count; i++)
            await SmoothMove(posList[i]);
    }

    private async UniTask ResolveBubbleImage()
    {
        string path = string.Empty;

        if (Model.BubbleType == BubbleType.Normal)
        {
            path = string.Format(PathDefine.BUBBLE_ICON_NORMAL_FORMAT, BubbleType.Normal, Model.BubbleColor);
        }
        else
        {
            path = string.Format(PathDefine.BUBBLE_ICON_FORMAT, Model.BubbleType);
        }

        await bubbleSprite.SafeLoadAsync(path);
    }
}
