using Cysharp.Threading.Tasks;

public class LobbyViewController : BaseController<LobbyViewModel>
{
    public override UIType UIType => UIType.LobbyView;

    public override UICanvasType UICanvasType => UICanvasType.View;

	public LobbyView View => GetView<LobbyView>();

    private bool entering = false;

    public override void Enter()
    {
        Model.SetOnStartBattle(OnStartBattle);
    }

    public void AllowClick(bool value)
    {
        View.AllowClick(value);
    }

    private void OnStartBattle()
    {
        OnStartBattleAsync().Forget();
    }

    private async UniTask OnStartBattleAsync()
    {
        if (entering)
            return;

        entering = true;
        var stageData = DataManager.Instance.GetDataById<DataBattleStage>((int)BattleStageDefine.STAGE_WILBUR);

        BattleFlowModel battleFlowModel = new BattleFlowModel();
        battleFlowModel.SetStageData(stageData);

        await FlowManager.Instance.ChangeFlow(FlowType.BattleFlow, battleFlowModel);
    }
}
