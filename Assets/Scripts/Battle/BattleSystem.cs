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

        while (!CheckEnd())
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

        ShowResult();
    }

    private void ShowResult()
    {
        var msg = currentPhase == BattlePhase.Win ? "Win!" : "Defeat";

        var messageBoxModel = new MessageBoxPopupModel();
        messageBoxModel.SetMessageType(MessageBoxType.TwoButton);
        messageBoxModel.SetOnConfirm(BackToLobby);
        messageBoxModel.SetMessageText(msg);

        var msgBoxController = new MessageBoxPopupController();
        msgBoxController.SetModel(messageBoxModel);

        UIManager.Instance.OpenPopup(msgBoxController).Forget();
    }

    private void BackToLobby()
    {
        LobbyFlowModel lobbyFlowModel = new LobbyFlowModel();

        FlowManager.Instance.ChangeFlow(FlowType.LobbyFlow, lobbyFlowModel).Forget();
    }

    private bool CheckEnd()
    {
        return currentPhase is BattlePhase.Win or BattlePhase.Defeat;
    }
}