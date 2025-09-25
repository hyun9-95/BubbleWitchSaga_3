#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class FlowManager : BaseManager<FlowManager>
{
    private FlowFactory flowFactory = new FlowFactory();

    public FlowType CurrentFlowType => currentFlow.FlowType;
    private BaseFlow currentFlow;
    private BaseFlow prevFlow;

    public async UniTask ChangeFlow(FlowType flowType, BaseFlowModel baseFlowModel = null)
    {
        Logger.Log($"Change Flow {flowType}");

        var newFlow = flowFactory.Create(flowType);
        newFlow.SetModel(baseFlowModel);

        if (newFlow == null)
            return;

        StopBgm(baseFlowModel.FlowBGMPath).Forget();

        // Transition In
        await TransitionManager.Instance.In(newFlow.TransitionType);
        await ProcessStateEvent(FlowState.TranstionIn, baseFlowModel);

        if (currentFlow != null)
        {
            var prevType = currentFlow.FlowType;
            prevFlow = currentFlow;
            await prevFlow.Exit();
            await UIManager.Instance.ClearCurrentView();

            currentFlow = null;
            await CleanUpAsync();

            Logger.Log($"Exit Prev Flow {prevType} => For Change Flow {flowType}");
            await ProcessStateEvent(FlowState.ExitPrevFlow, baseFlowModel);
        }

        currentFlow = newFlow;
        await currentFlow.LoadScene();

        if (prevFlow != null)
            await prevFlow.UnloadScene();

        await currentFlow.LoadingProcess();
        await ProcessStateEvent(FlowState.LoadingProcess, baseFlowModel);

        await currentFlow.Process();
        await ProcessStateEvent(FlowState.Process, baseFlowModel);

        if (SoundManager.Instance.IsPlayingSoloSound(SoundType.Bgm))
            await UniTask.WaitWhile(() => SoundManager.Instance.IsPlayingSoloSound(SoundType.Bgm));

        await PlayBgmAsync(baseFlowModel.FlowBGMPath);


        await UniTask.WaitWhile(() => TransitionManager.Instance.IsPlaying);

        // Transition Out
        if (baseFlowModel.IsExistStateEvent(FlowState.TransitionOut))
        {
            await TransitionManager.Instance.Out(newFlow.TransitionType);
            await ProcessStateEvent(FlowState.TransitionOut, baseFlowModel);
        }
        else
        {
            TransitionManager.Instance.Out(newFlow.TransitionType).Forget();
        }

        baseFlowModel.ClearStateEvent();
    }

    private async UniTask ProcessStateEvent(FlowState state, BaseFlowModel flowModel)
    {
        await flowModel.ProcessStateEvent(state);
    }

    private async UniTask StopBgm(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        
        // 이미 동일 BGM 재생중일 경우 그대로 재생
        if (SoundManager.Instance.IsPlayingSoloSound(SoundType.Bgm, path))
            return;

        await SoundManager.Instance.StopCurrentSoloSound(SoundType.Bgm);
    }

    private async UniTask PlayBgmAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        await SoundManager.Instance.PlaySoloSound(SoundType.Bgm, path);
    }

    private async UniTask CleanUpAsync()
    {
        // 풀링해놨던 팩토리, 매니저등을 Clear 해준다.
        ObjectPoolManager.Instance.Clear();
        SoundManager.Instance.Clear();

        AddressableManager.Instance.ReleaseAllHandles();
        await Resources.UnloadUnusedAssets();
    }
}