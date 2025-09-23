using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroView : BaseView
{
    private IntroViewModel Model => GetModel<IntroViewModel>();

    [SerializeField]
    private GameObject buttonStart;

    [SerializeField]
    private LoadingBar loadingBar;

    public async UniTask ShowDataLoadingProgress()
    {
        while (Model.DataLoader.IsLoading)
        {
            UpdateLoadingUI();
            await UniTaskUtils.NextFrame(TokenPool.Get(GetHashCode()));
        }

        loadingBar.SetLoadingProgress(1);
        Model.OnEnterGame?.Invoke();
    }

    public void UpdateLoadingUI()
    {
        if (loadingBar == null)
            return;

        loadingBar.SetLoadingProgressText(Model.GetLoadingProgressText());
        loadingBar.SetLoadingProgress(Model.DataLoader.CurrentProgressValue);
    }
}
