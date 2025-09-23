#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleViewController : BaseController<BattleViewModel>
{
    public override UIType UIType => UIType.BattleView;

    public override UICanvasType UICanvasType => UICanvasType.View;

	public BattleView View => GetView<BattleView>();

    public override void Enter()
    {
    }

    public override async UniTask Process()
    {
        await View.ShowAsync();
    }

    public async UniTask<BubbleNode> LaunchCurrentBubble(List<Vector3> path)
    {
        return await View.LaunchCurrentRingSlot(path);
    }

    public void EnableClickBlocker(bool value)
    {
        View.EnableClickBlocker(value);
    }
}
