using Cysharp.Threading.Tasks;

public interface IBattlePhaseProcessor
{
    public BattlePhase Phase { get; }

    public UniTask OnStartPhase(BattleGrid grid);

    public void OnProcessPhase(BattleGrid grid);

    public UniTask OnEndPhase(BattleGrid grid);
}
