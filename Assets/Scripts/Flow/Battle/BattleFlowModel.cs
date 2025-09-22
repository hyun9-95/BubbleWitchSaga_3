using Cysharp.Threading.Tasks;

public class BattleFlowModel : BaseFlowModel
{
    public DataBattleStage StageData { get; private set; }

    public void SetStageData(DataBattleStage stageData)
    {
        StageData = stageData;
    }
}
