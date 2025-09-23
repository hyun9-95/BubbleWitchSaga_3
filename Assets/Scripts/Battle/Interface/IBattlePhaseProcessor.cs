using Cysharp.Threading.Tasks;

public interface IBattlePhaseProcessor
{
    public BattlePhase Phase { get; }

    public UniTask Initialize(BattleGrid grid);

    public UniTask OnStartPhase();

    public void OnProcessPhase();

    public UniTask OnEndPhase();
}
