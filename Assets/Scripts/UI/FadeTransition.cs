using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class FadeTransition : Transition
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float duration = 1;

    public override async UniTask In()
    {
        gameObject.SafeSetActive(true);
        await Fade(0, 1);
    }

    public override async UniTask Out()
    {
        await Fade(1, 0);
        gameObject.SafeSetActive(false);
    }

    private async UniTask Fade(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0;
        canvasGroup.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / duration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            canvasGroup.alpha = alpha;

            await UniTaskUtils.NextFrame();
        }

        canvasGroup.alpha = targetAlpha;
    }
}
