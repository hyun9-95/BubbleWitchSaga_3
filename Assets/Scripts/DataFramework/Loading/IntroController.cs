#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroController : BaseController<IntroViewModel>
{
    public override UIType UIType => UIType.IntroView;

    public override UICanvasType UICanvasType => UICanvasType.View;

    private IntroView View => GetView<IntroView>();

    public override void Enter()
    {
        InitializeDataLoader();
        Model.SetOnEnterGame(OnEnterGame);
    }

    public override async UniTask Process()
    {
        await LoadResources();
        await LoadDatas();
    }

    private void InitializeDataLoader()
    {
        switch (Model.LoadDataType)
        {
#if UNITY_EDITOR
            case LoadDataType.Editor:
                EditorDataLoader editorData = new();
                editorData.SetLocalJsonDataPath(PathDefine.Json);
                editorData.SetOnSuccessLoadData(OnSuccessDataLoader);
                Model.SetEditorDataLoader(editorData);
                break;
#endif

            case LoadDataType.Addressable:
                AddressableDataLoader addressableDataLoader = new();
                addressableDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);

                // 뷰를 트래커로 할당해서, View가 파괴될 때 자동으로 TextAsset들을 해제한다.
                addressableDataLoader.SetAddressableTracker(View);
                Model.SetAddressableDataLoader(addressableDataLoader);
                break;
        }
    }

    private void OnSuccessDataLoader()
    {
    }

    private async UniTask LoadResources()
    {
        Model.SetLoadingState(IntroViewModel.LoadingState.ResourceLoading);
        
        View.UpdateLoadingUI();
    }

    private async UniTask LoadDatas()
    {
        Model.SetLoadingState(IntroViewModel.LoadingState.DataLoading);

        Model.DataLoader.LoadData().Forget();
        View.ShowDataLoadingProgress().Forget();

        await UniTask.WaitUntil(() => { return !Model.DataLoader.IsLoading; });

        DataManager.Instance.GenerateDataContainerByDataDic(Model.DataLoader.DicJsonByFileName);
    }

    private void OnEnterGame()
    {
        OnEnterGameAsync().Forget();
    }

    private async UniTask OnEnterGameAsync()
    {
        LobbyFlowModel lobbyFlowModel = new LobbyFlowModel();

        await FlowManager.Instance.ChangeFlow(FlowType.LobbyFlow, lobbyFlowModel);
    }
}
