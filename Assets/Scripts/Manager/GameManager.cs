using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : BaseMonoManager<GameManager>
{
    #region Property
    #endregion
    [SerializeField]
    private LoadDataType loadDataType;

    private void Awake()
    {
        SetInstance(this);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        IntroFlowModel introFlowModel = new IntroFlowModel();
        introFlowModel.SetLoadDataType(loadDataType);

        FlowManager.Instance.ChangeFlow(FlowType.IntroFlow, introFlowModel).Forget();
    }
}