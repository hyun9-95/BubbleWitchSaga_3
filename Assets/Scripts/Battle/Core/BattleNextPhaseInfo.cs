public class BattleNextPhaseInfo
{
    public BattleNextPhaseInfo(BattlePhase phase, IBattlePhaseParam param = null)
    {
        BattlePhase = phase;
        BattlePhaseParam = param;
    }

    public readonly BattlePhase BattlePhase;

    public readonly IBattlePhaseParam BattlePhaseParam;
}
