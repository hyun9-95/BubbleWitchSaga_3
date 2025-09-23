#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleFlow : BaseFlow<BattleFlowModel>
{
    public override UIType ViewType => UIType.BattleView;

    public override FlowType FlowType => FlowType.BattleFlow;

    private BattleScene battleScene;

    public override async UniTask LoadingProcess()
    {
        battleScene = loadedScene.GetRootComponent<BattleScene>();
        
        await battleScene.Prepare();

        await BubbleFactory.Instance.PrewarmBubbles(Model.StageData.SpawnCount * 2);

        await PrepareBattleStage(Model.StageData, battleScene.transform, battleScene.Grid);
        await PreparBattlePlayer(Model.StageData, battleScene.Grid);
    }

    private async UniTask PrepareBattleStage(DataBattleStage dataStage, Transform transform, BattleGrid grid)
    {
        var stagePath = dataStage.StagePath;

        var stage = await AddressableManager.Instance.
            InstantiateAddressableMonoAsync<BattleStagePhase>(stagePath, transform);

        var stageModel = new BattleStageModel();
        stageModel.SetSpawnCount(dataStage.SpawnCount);
        stage.SetModel(stageModel);

        await stage.Initialize(grid);

        battleScene.AddPhaseProcessor(BattlePhase.Stage, stage);
    }

    public override async UniTask Process()
    {
        battleScene.StartBattle().Forget();
    }

    private async UniTask PreparBattlePlayer(DataBattleStage dataStage, BattleGrid grid)
    {
        var battlePlayerPhase = new BattlePlayerPhase();
        var battlePlayerPhaseModel = new BattlePlayerPhaseModel();
        battlePlayerPhaseModel.SetUserBubbleCount(Model.StageData.UserBubbleCount);

        battlePlayerPhase.SetModel(battlePlayerPhaseModel);
        await battlePlayerPhase.Initialize(battleScene.Grid);

        battleScene.AddPhaseProcessor(BattlePhase.Player, battlePlayerPhase);
    }
}
