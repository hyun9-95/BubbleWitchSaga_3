#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TransitionManager : BaseMonoManager<TransitionManager>
{
    public bool IsPlaying => isPlaying;

    [SerializeField]
    private Transition[] transitions;

    private bool isPlaying = false;
    /// <summary>
    /// In 은 Flow에서만 사용하자.
    /// </summary>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    public async UniTask In(TransitionType transitionType)
    {
        isPlaying = true;
        var transition = transitions[(int)transitionType];

        if (transition.IsPlaying)
            await UniTask.WaitUntil(() => !transition.IsPlaying);

        await transition.In();
        Logger.Success($"[Transition] In => {transitionType}");
        isPlaying = false;
    }

    public async UniTask Out(TransitionType transitionType)
    {
        isPlaying = true;
        var transition = transitions[(int)transitionType];

        if (transition.IsPlaying)
            await UniTask.WaitUntil(() => !transition.IsPlaying);

        await transition.Out();
        Logger.Success($"[Transition] Out => {transitionType}");
        isPlaying = false;
    }
}