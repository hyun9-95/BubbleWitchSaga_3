#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleFlow : BaseFlow<BattleFlowModel>
{
    public override UIType ViewType => UIType.BattleView;

    public override FlowType FlowType => FlowType.BattleFlow;

    private BattleScene battleScene;
    private BattleSystem battleSystem;
    private BattleViewController battleViewController;

    public override async UniTask LoadingProcess()
    {
        battleScene = loadedScene.GetRootComponent<BattleScene>();
        battleSystem = new BattleSystem();

        await battleScene.Prepare();

        await BubbleFactory.Instance.PrewarmBubbles(Model.StageData.SpawnCount * 4);

        battleViewController = PrepareBattleView();

        var stage = await PrepareBattleStage(Model.StageData, battleScene.transform, battleScene.Grid, battleViewController);
        battleSystem.AddPhaseProcessor(BattlePhase.Stage, stage);

        var player = await PrepareBattlePlayer(Model.StageData, battleScene.Grid, battleViewController);
        battleSystem.AddPhaseProcessor(BattlePhase.Player, player);

        var interaction = await PrepareBattleInteraction(battleScene.Grid, battleViewController);
        battleSystem.AddPhaseProcessor(BattlePhase.Interaction, interaction);

        await UIManager.Instance.ChangeView(battleViewController);
    }

    public override async UniTask Process()
    {
        battleSystem.StartBattle().Forget();
    }

    private BattleViewController PrepareBattleView()
    {
        var battleViewController = new BattleViewController();
        BattleViewModel viewModel = new BattleViewModel();

        battleViewController.SetModel(viewModel);

        return battleViewController;
    }

    private async UniTask<IBattlePhaseProcessor> PrepareBattleStage(DataBattleStage dataStage, Transform transform, BattleGrid grid, BattleViewController viewController)
    {
        var stagePath = dataStage.StagePath;

        var stage = await AddressableManager.Instance.
            InstantiateAddressableMonoAsync<BattleStagePhase>(stagePath, transform);

        var stageModel = new BattleStageModel();
        stageModel.SetSpawnCount(dataStage.SpawnCount);

        if (Model.StageData.Boss != BattleBossDefine.None)
        {
            var bossData = DataManager.Instance.GetDataById<DataBattleBoss>((int)Model.StageData.Boss);
            stageModel.SetBossData(bossData);
        }

        stage.SetModel(stageModel);

        await stage.Initialize(grid, viewController);

        return stage;
    }

    private async UniTask<IBattlePhaseProcessor> PrepareBattlePlayer(DataBattleStage dataStage, BattleGrid grid, BattleViewController viewController)
    {
        var battlePlayerPhase = new BattlePlayerPhase();
        var battlePlayerPhaseModel = new BattlePlayerPhaseModel();
        battlePlayerPhaseModel.SetUserBubbleCount(dataStage.UserBubbleCount);

        battlePlayerPhase.SetModel(battlePlayerPhaseModel);
        await battlePlayerPhase.Initialize(grid, viewController);

        return battlePlayerPhase;
    }

    private async UniTask<IBattlePhaseProcessor> PrepareBattleInteraction(BattleGrid grid, BattleViewController viewController)
    {
        var battleInteractionPhase = new BattleInteractionPhase();
        await battleInteractionPhase.Initialize(grid, viewController);

        battleInteractionPhase.SetScrollTask(battleScene.ScrollAsync);

        return battleInteractionPhase;
    }
}
