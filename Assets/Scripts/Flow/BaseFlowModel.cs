using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class BaseFlowModel
{
    #region Property
    public SceneDefine SceneDefine { get; private set; }

    public string FlowBGMPath {  get; private set; }
    #endregion

    #region Value
    private Dictionary<FlowState, List<Func<UniTask>>> stateEventDic = new();
    private Dictionary<FlowState, List<Action>> stateActionDic = new();
    #endregion

    #region Function
    public void SetSceneDefine(SceneDefine define)
    {
        SceneDefine = define;
    }

    public void SetFlowBGMPath(string value)
    {
        FlowBGMPath = value;
    }

    public void AddStateEvent(FlowState state, Func<UniTask> stateEvent)
    {
        if (!stateEventDic.TryGetValue(state, out var list))
        {
            list = new List<Func<UniTask>>();
            stateEventDic[state] = list;
        }

        list.Add(stateEvent);
    }

    public void AddStateEvent(FlowState state, Action stateEvent)
    {
        if (!stateActionDic.TryGetValue(state, out var list))
        {
            list = new List<Action>();
            stateActionDic[state] = list;
        }

        list.Add(stateEvent);
    }

    public void ClearStateEvent()
    {
        stateEventDic.Clear();
    }

    public bool IsExistStateEvent(FlowState state)
    {
        return stateEventDic.ContainsKey(state) || stateActionDic.ContainsKey(state);   
    }

    public async UniTask ProcessStateEvent(FlowState state)
    {
        if (stateEventDic.TryGetValue(state, out var handlers))
        {
            foreach (var handler in handlers)
                await handler();
        }
        
        if (stateActionDic.TryGetValue(state, out var actionHandlers))
        {
            foreach (var actionHandler in actionHandlers)
                actionHandler.Invoke();
        }
    }
    #endregion
}
