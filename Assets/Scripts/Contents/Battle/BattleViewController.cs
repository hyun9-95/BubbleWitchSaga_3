public class BattleViewController : BaseController<BattleViewModel>
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

	public BattleView View => GetView<BattleView>();

    public override void Enter()
    {
        
    }
}
