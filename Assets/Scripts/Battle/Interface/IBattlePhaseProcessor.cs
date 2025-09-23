using Cysharp.Threading.Tasks;

public interface IBattlePhaseProcessor
{
    public BattlePhase Phase { get; }

    public UniTask Initialize(BattleGrid grid);

    public UniTask OnStartPhase(IBattlePhaseParam param);

    public UniTask OnProcess();

    public UniTask OnEndPhase();

    public BattleNextPhaseInfo OnNextPhase();
}
