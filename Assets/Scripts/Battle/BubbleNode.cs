using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
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

    [Header("Fade")]
    [SerializeField]
    private float fadeTime = 0.2f;

    [Header("Floating")]
    [SerializeField]
    private float floatingTime = 0.7f;

    [SerializeField]
    private float floatingScale = 0.25f;

    [SerializeField]
    private float floatingWave = 2f;

    [Header("BounceMove")]
    [SerializeField]
    private float bounceMoveTime = 0.5f;

    [SerializeField]
    private float maxSpeedMultiplier = 4f;

    [Header("Drop")]
    [SerializeField]
    private float startGravity = 10f;

    [SerializeField]
    private float bounceMultiplier = 0.7f;

    [SerializeField]
    private int startBounceCount = 2;

    private string bubblePath = string.Empty;
    private Color originColor;

    private void Awake()
    {
        originColor = bubbleSprite.color;
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

        var ct = TokenPool.Get(GetHashCode());
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

    /// <summary>
    /// 체공 => 타겟과 반대 방향으로 1회 바운스 => 점차 타겟 방향 이동
    /// </summary>
    public async UniTask FairyMove(Vector3 targetPos, Action onArrive)
    {
        await FloatingAsync();
        await BounceMoveTargetAsync(targetPos);

        onArrive?.Invoke();
    }

    /// <summary>
    /// 체공
    /// </summary>
    /// <returns></returns>
    private async UniTask FloatingAsync()
    {
        Vector2 baseAirPos = transform.position;
        float elapsedTime = 0;

        var ct = TokenPool.Get(GetHashCode());

        while (!ct.IsCancellationRequested && elapsedTime < floatingTime)
        {
            elapsedTime += Time.deltaTime;

            // 다른 주기를 줘서 다른 움직임 구현
            float xPos = Mathf.Sin(elapsedTime * floatingWave + 1) * floatingScale; 
            float yPos = Mathf.Cos(elapsedTime * floatingWave) * floatingScale; 

            var airPos = new Vector3(
                baseAirPos.x + xPos,
                baseAirPos.y + yPos,
                transform.position.z
            );

            transform.position = airPos;
            await UniTask.NextFrame(ct);
        }
    }

    /// <summary>
    /// 타겟과 반대 방향으로 1회 바운스 => 점차 타겟 방향 이동
    /// </summary>
    private async UniTask BounceMoveTargetAsync(Vector3 targetPos)
    {
        float elapsedTime = 0f;
        float elapsedBounceTime = 0f;
        float startMoveSpeed = 1f;
        float maxMoveSpeed = Model.MoveSpeed * maxSpeedMultiplier;

        Vector3 initialDir = (targetPos - transform.position).normalized;
        Vector3 bounceDir = -initialDir;

        var ct = TokenPool.Get(GetHashCode());
        while (!ct.IsCancellationRequested && transform.IsOverDistance(targetPos, 0.1f))
        {
            // 지정한 시간에 최고속도 도달
            elapsedTime += Time.deltaTime;
            float speedProgress = Mathf.Clamp01(elapsedBounceTime / bounceMoveTime);
            float speed = Mathf.Lerp(startMoveSpeed,
                                     maxMoveSpeed,
                                     speedProgress);

            Vector3 dir;

            if (elapsedTime < bounceMoveTime)
            {
                elapsedBounceTime += Time.deltaTime;

                // 뒤로 이동하다가 점차 타겟 방향으로 전환
                float bounceProgress = elapsedTime / bounceMoveTime;
                Vector3 targetDir = (targetPos - transform.position).normalized;
                dir = Vector3.Lerp(bounceDir, targetDir, bounceProgress);
                transform.position += speed * Time.deltaTime * dir;
            }
            else
            {
                // 타겟 방향으로 이동
                dir = (targetPos - transform.position).normalized;
                transform.position += speed * Time.deltaTime * dir;
            }
            await UniTask.NextFrame(ct);
        }
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

        var ct = TokenPool.Get(GetHashCode());
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
        // 위 + 좌우 랜덤 속도
        Vector2 velocity = new Vector2(
            UnityEngine.Random.Range(-2f, 2f), 
            UnityEngine.Random.Range(2f, 4f)   
        );

        float gravity = startGravity * UnityEngine.Random.Range(0.5f, 1f);
        float bounceCount = startBounceCount;
        float bounceStrength = bounceMultiplier;

        var ct = TokenPool.Get(GetHashCode());
        while (!ct.IsCancellationRequested && gameObject.SafeActiveSelf())
        {
            float deltaTime = Time.deltaTime;

            // Y축 가속
            velocity.y -= gravity * deltaTime;

            // X축 감속
            velocity.x *= 0.995f; // 프레임마다 0.5%

            Vector3 currentPos = transform.position;
            currentPos.x += velocity.x * deltaTime;
            currentPos.y += velocity.y * deltaTime;

            if (currentPos.y - bubbleCol.radius <= dropPosY && bounceCount > 0 && velocity.y < 0)
            {
                // 위로 바운스
                float bouncePosY = -velocity.y * bounceStrength;
                velocity.y = bouncePosY;

                // 다음 바운스는 파워 감소
                bounceStrength *= bounceMultiplier; 
                velocity.x *= bounceMultiplier;

                bounceCount--;

                // 바닥 위치
                currentPos.y = dropPosY + bubbleCol.radius;
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
