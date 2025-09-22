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
        
        // Phase ¡ÿ∫Ò
        await LoadBattleStage(Model.StageData, battleScene.transform);

        await ShowBattleView();
    }

    private async UniTask LoadBattleStage(DataBattleStage dataStage, Transform transform)
    {
        var stagePath = dataStage.StagePath;

        var stage = await AddressableManager.Instance.
            InstantiateAddressableMonoAsync<BattleStagePhase>(stagePath, transform);

        var stageModel = new BattleStageModel();
        stageModel.SetSpawnCount(dataStage.SpawnCount);
        stage.SetModel(stageModel);

        battleScene.AddPhaseProcessor(BattlePhase.Stage, stage);
    }

    public override async UniTask Process()
    {
        battleScene.StartBattle().Forget();
    }

    private async UniTask ShowBattleView()
    {
        BattleViewController battleController = new BattleViewController();
        BattleViewModel viewModel = new BattleViewModel();
        battleController.SetModel(viewModel);

        await UIManager.Instance.ChangeView(battleController);
    }
}
