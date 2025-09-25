using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : BaseMonoManager<GameManager>
{
    #region Property
    #endregion
    [SerializeField]
    private LoadDataType loadDataType;

    [SerializeField]
    private int setWidth = 1080;

    [SerializeField]
    private int setHeight = 1920;

    private void Awake()
    {
        SetInstance(this);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetResolution();

        IntroFlowModel introFlowModel = new IntroFlowModel();
        introFlowModel.SetLoadDataType(loadDataType);

        FlowManager.Instance.ChangeFlow(FlowType.IntroFlow, introFlowModel).Forget();
    }

    private void SetResolution()
    {
#if !UNITY_EDITOR
        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)((float)deviceHeight / deviceWidth * setWidth), true);

        float targetAspect = (float)setWidth / setHeight;
        float deviceAspect = (float)deviceWidth / deviceHeight;

        if (targetAspect < deviceAspect)
        {
            float newWidth = targetAspect / deviceAspect;
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = deviceAspect / targetAspect;
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
#endif
    }
}