using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BubbleNode : PoolableBaseUnit<BubbleNodeModel>
{
    public float Radius => bubbleCol != null ?
        bubbleCol.radius : 0;

    [SerializeField]
    private CircleCollider2D bubbleCol;

    [SerializeField]
    private SpriteRenderer bubbleSprite;

    [SerializeField]
    private SpriteRenderer fairyCover;

    private string bubblePath = string.Empty;

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
        if (Model.MoveSpeed == 0)
        {
            Logger.Error("MoveSpeed == 0");
            return;
        }

        if (posList == null || posList.Count == 0)
            return;

        for (int i = 0; i < posList.Count; i++)
            await SmoothMove(posList[i]);
    }

    private async UniTask ResolveBubbleImage()
    {
        string path = string.Empty;

        if (Model.BubbleType is BubbleType.Normal or BubbleType.Fairy)
        {
            path = string.Format(PathDefine.BUBBLE_ICON_NORMAL_FORMAT, BubbleType.Normal, Model.BubbleColor);  
        }
        else
        {
            path = string.Format(PathDefine.BUBBLE_ICON_FORMAT, Model.BubbleType);
        }

        fairyCover.gameObject.SafeSetActive(Model.BubbleType == BubbleType.Fairy);

        if (path.Equals(bubblePath))
            return;

        await bubbleSprite.SafeLoadAsync(path);
        bubblePath = path;

        bubbleSprite.sortingOrder = Model.BubbleType == BubbleType.Spawn ?
                IntDefine.BUBBLE_SORTING_ORDER_SPAWN :
                IntDefine.BUBBLE_SORTING_ORDER_NORMAL;
    }
}
