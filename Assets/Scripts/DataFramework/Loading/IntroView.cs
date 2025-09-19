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
    }

    public void UpdateLoadingUI()
    {
        if (loadingBar == null)
            return;

        loadingBar.SetLoadingProgressText(Model.GetLoadingProgressText());
        loadingBar.SetLoadingProgress(Model.DataLoader.CurrentProgressValue);
    }

    public void ShowComplete(bool value)
    {
        loadingBar.gameObject.SafeSetActive(!value);
        buttonStart.SafeSetActive(value);
    }

    public void OnEnterGame()
    {
        Model.OnEnterGame?.Invoke();
    }
}
