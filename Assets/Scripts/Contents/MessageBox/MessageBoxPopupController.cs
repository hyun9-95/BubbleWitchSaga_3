using Cysharp.Threading.Tasks;

public class MessageBoxPopupController : BaseController<MessageBoxPopupModel>
{
    public override UIType UIType => UIType.MessageBoxPopup;

    public override UICanvasType UICanvasType => UICanvasType.Popup;

    public MessageBoxPopup View => GetView<MessageBoxPopup>();

    public override void Enter()
    {
        Model.SetOnClickCancel(OnClickCancel);
        Model.SetOnClickConfirm(OnClickConfirm);
    }

    private void OnClickCancel()
    {
        UIManager.Instance.Back().Forget();
    }

    private void OnClickConfirm()
    {
        Model.OnConfirm();
    }
}
