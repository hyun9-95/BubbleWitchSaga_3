#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleViewController : BaseController<BattleViewModel>
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

	public BattleView View => GetView<BattleView>();

    private bool firstShow = true;

    public override void Enter()
    {
        Model.BattleRingSlotModel.SetOnChangeBubbleColor(OnChangeBubbleColor);
    }

    public override async UniTask Process()
    {
        View.EnableClickBlocker(true);
        await View.ShowAsync();

        if (!firstShow)
            View.EnableClickBlocker(false);

        firstShow = false;
    }

    public async UniTask<BubbleNode> LaunchCurrentBubble(List<Vector3> path, CellPosition targetCellPos)
    {
        View.EnableClickBlocker(true);

        var currentBubble = Model.BattleRingSlotModel.ConsumeCurrentBubble();

        if (currentBubble == null)
            return null;

        Model.BattleRingSlotModel.ReduceBubbleCount();
        View.RefreshBubbleCount();

        await currentBubble.MoveAlongPath(path);
        currentBubble.Model.SetCellPos(targetCellPos);

        return currentBubble;
    }

    public void DealsFairyDamage()
    {
        int damage = IntDefine.BUBBLE_FAIRY_DAMAGE;
        Model.HpBarModel.ReduceValue(damage);
        View.RefreshHpBar();
    }

    private void OnChangeBubbleColor(BubbleColor color)
    {
        var lineColor = GetLineColor(color);
        Model.BattleBubbleLauncherModel.SetLineColor(lineColor);
    }

    private Color GetLineColor(BubbleColor bubbleColor)
    {
        return bubbleColor switch
        {
            BubbleColor.Red => Color.red,
            BubbleColor.Yellow => Color.yellow,
            BubbleColor.Blue => Color.blue,
            _ => Color.white,
        };
    }
}
