using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class LobbyFlow : BaseFlow<LobbyFlowModel>
{
    public override UIType ViewType => UIType.LobbyView;

    public override FlowType FlowType => FlowType.LobbyFlow;

    private LobbyViewController lobbyViewController;

    public override async UniTask LoadingProcess()
    {
        await ShowLobbyView();
    }

    public override async UniTask Process()
    {
        await UniTask.WaitUntil(() => !TransitionManager.Instance.IsPlaying);
        lobbyViewController.AllowClick(true);
    }

    private async UniTask ShowLobbyView()
    {
        lobbyViewController = new LobbyViewController();
        LobbyViewModel lobbyViewModel = new LobbyViewModel();
        lobbyViewController.SetModel(lobbyViewModel);

        await UIManager.Instance.ChangeView(lobbyViewController);
    }
}
