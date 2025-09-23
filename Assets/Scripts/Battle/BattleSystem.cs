#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BattleSystem
{
    private BattlePhase currentPhase;
    private Dictionary<BattlePhase, IBattlePhaseProcessor> processorDic = new();

    public void AddPhaseProcessor(BattlePhase phase, IBattlePhaseProcessor processor)
    {
        processorDic.Add(phase, processor);
    }

    public async UniTask StartBattle()
    {
        currentPhase = BattlePhase.Stage;
        OnBattle().Forget();
    }

    public async UniTask OnBattle()
    {
        IBattlePhaseParam battlePhaseParam = null;

        while (currentPhase != BattlePhase.End)
        {
            var battlePhaseProcessor = processorDic[currentPhase];

            Logger.Success($"Current Phase => {currentPhase}");

            await battlePhaseProcessor.OnStartPhase(battlePhaseParam);
            await battlePhaseProcessor.OnProcess();
            await battlePhaseProcessor.OnEndPhase();

            var battleNextPhaseInfo = battlePhaseProcessor.OnNextPhase();
            battlePhaseParam = battleNextPhaseInfo.BattlePhaseParam;

            Logger.Success($"End Phase => {battlePhaseProcessor.Phase}");

            currentPhase = battleNextPhaseInfo.BattlePhase;
        }
    }
}