#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class IntroFlow : BaseFlow<IntroFlowModel>
{
    public override UIType ViewType => UIType.IntroView;

    public override FlowType FlowType => FlowType.IntroFlow;

    public override async UniTask LoadingProcess()
    {
        await AddressableManager.Instance.InitializeAsync();
    }

    private async UniTask ShowIntroView(LoadDataType loadDataType)
    {
        IntroController IntroController = new IntroController();
        IntroViewModel viewModel = new IntroViewModel();
        viewModel.SetLoadDataType(loadDataType);
        viewModel.SetOnEnterGame(OnEnterGame);

        IntroController.SetModel(viewModel);

        // 로딩은 기본 Resources 경로에 포함
        await UIManager.Instance.ChangeView(IntroController, false);
    }

    public override async UniTask Process()
    {
        ShowIntroView(Model.LoadDataType).Forget();
    }

    private void OnEnterGame()
    {
        Logger.Log("Loading Done.");
    }


    public override async UniTask Exit()
    {
    }
}
