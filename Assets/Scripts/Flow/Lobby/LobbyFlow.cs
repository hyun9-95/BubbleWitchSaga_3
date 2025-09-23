using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class LobbyFlow : BaseFlow<LobbyFlowModel>
{
    public override UIType ViewType => UIType.LobbyView;

    public override FlowType FlowType => FlowType.LobbyFlow;

    public override async UniTask Process()
    {
        await ShowLobbyView();
    }

    private async UniTask ShowLobbyView()
    {
        LobbyViewController lobbyViewController = new LobbyViewController();
        LobbyViewModel lobbyViewModel = new LobbyViewModel();
        lobbyViewController.SetModel(lobbyViewModel);

        await UIManager.Instance.ChangeView(lobbyViewController);
    }
}
