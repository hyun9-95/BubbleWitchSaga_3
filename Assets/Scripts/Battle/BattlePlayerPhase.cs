#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattlePlayerPhase : BaseUnit<BattlePlayerPhaseModel>, IBattlePhaseProcessor
{
    public BattlePhase Phase => throw new System.NotImplementedException();

    private bool isInitialized = false;

    [SerializeField]
    private BattleRingSlot ringSlot;

    public async UniTask OnStartPhase(BattleGrid grid)
    {
        if (!isInitialized)
        {
            await ringSlot.InitializeSlot();
            isInitialized = true;
        }

        await ringSlot.RefillBubble();
    }

    public async UniTask OnEndPhase(BattleGrid grid)
    {
    }

    public void OnProcessPhase(BattleGrid grid)
    {

    }
}
