using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

    [SerializeField]
    private float fadeTime = 0.2f;

    [SerializeField]
    private float startGravity = 10f;

    [SerializeField]
    private float startBounceHeight = 0.7f;

    [SerializeField]
    private int startBounceCount = 2;

    private string bubblePath = string.Empty;
    private Color originColor;
    private CancellationToken ct;

    private void Awake()
    {
        originColor = bubbleSprite.color;
    }

    private void OnEnable()
    {
        ct = TokenPool.Get(GetHashCode());
    }

    public override async UniTask ShowAsync()
    {
        await ResolveBubbleSprite();

        fairyCover.gameObject.SafeSetActive(Model.BubbleType == BubbleType.Fairy);
        bubbleSprite.color = originColor;
    }

    public void SetColliderEnable(bool enable)
    {
        bubbleCol.enabled = enable;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public async UniTask SmoothMove(Vector3 pos, Action onEnd = null)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, pos);
        float duration = distance / Model.MoveSpeed;
        float elapsedTime = 0;

        while (!ct.IsCancellationRequested && elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPos, pos, progress);
            await UniTask.NextFrame(ct);
        }

        if (this.CheckSafeNull())
            return;

        transform.position = pos;

        onEnd?.Invoke();
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

    /// <summary>
    /// 페이드아웃 + 완료 후 비활성화
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeOff()
    {
        if (bubbleSprite == null)
            return;

        fairyCover.gameObject.SafeSetActive(false);

        float startAlpha = originColor.a;
        float targetAlpha = 0;
        float elapsedTime = 0;

        while (!ct.IsCancellationRequested && elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / fadeTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            Color newColor = originColor;
            newColor.a = alpha;
            bubbleSprite.color = newColor;

            await UniTask.NextFrame(ct);
        }

        if (this.CheckSafeNull())
            return;

        Color finalColor = originColor;
        finalColor.a = targetAlpha;
        bubbleSprite.color = finalColor;
        gameObject.SafeSetActive(false);
    }

    /// <summary>
    /// 페이드아웃 + 중력 + 완료 후 비활성화
    /// </summary>
    /// <param name="dropPosY"></param>
    /// <returns></returns>
    public async UniTask DropFadeOff(float dropPosY)
    {
        float elapsedTime = 0;
        float velocity = 0;
        float gravityRatio = UnityEngine.Random.Range(0.5f, 1);

        float bounceHeight = startBounceHeight;
        float gravity = startGravity * gravityRatio;
        float bounceCount = startBounceCount;

        while (!ct.IsCancellationRequested && gameObject.SafeActiveSelf())
        {
            elapsedTime += Time.deltaTime;
            float deltaTime = Time.deltaTime;

            velocity += gravity * deltaTime;

            // 위치 업데이트
            Vector3 currentPos = transform.position;
            currentPos.y -= velocity * deltaTime;

            if (currentPos.y <= dropPosY && bounceCount > 0)
            {
                float bounceVelocity = Mathf.Sqrt(2 * gravity * bounceHeight);
                velocity = -bounceVelocity;
                bounceCount--;

                // 바운스 높이 점점 감소
                bounceHeight *= 0.7f; 
            }

            transform.position = currentPos;

            // 일정 횟수 이상 튕기면 페이드 시작
            if (!this.CheckSafeNull() && bounceCount == 0)
                FadeOff().Forget();

            await UniTask.NextFrame(ct);
        }
    }

    private async UniTask ResolveBubbleSprite()
    {
        if (Model.BubbleType == BubbleType.None)
        {
            bubblePath = null;
            bubbleSprite.enabled = false;
            return;
        }

        string path = string.Empty;

        if (Model.IsColorType)
        {
            path = string.Format(PathDefine.BUBBLE_ICON_NORMAL_FORMAT, BubbleType.Normal, Model.BubbleColor);  
        }
        else
        {
            path = string.Format(PathDefine.BUBBLE_ICON_FORMAT, Model.BubbleType);
        }

        if (path.Equals(bubblePath))
            return;

        await bubbleSprite.SafeLoadAsync(path);
        bubbleSprite.enabled = true;
        bubblePath = path;

        bubbleSprite.sortingOrder = Model.BubbleType == BubbleType.Spawn ?
                IntDefine.BUBBLE_SORTING_ORDER_SPAWN :
                IntDefine.BUBBLE_SORTING_ORDER_NORMAL;
    }

    public void OnRemoveFromCell()
    {
        if (Model == null)
            return;

        Model.OnRemoveFromCell?.Invoke();
        Model.SetOnRemoveFromCell(null);
    }
}
