#pragma warning disable CS1998
using Cysharp.Threading.Tasks;

public class BattleViewController : BaseController<BattleViewModel>, IBattlePhaseProcessor
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

	public BattleView View => GetView<BattleView>();

    public BattlePhase Phase => BattlePhase.Player;

    public override void Enter()
    {
    }

    public async UniTask OnStartPhase(BattleGrid grid)
    {
        await View.ShowAsync();
    }

    public async UniTask OnEndPhase(BattleGrid grid)
    {
    }

    public async void OnProcessPhase(BattleGrid grid)
    {
    }
}
