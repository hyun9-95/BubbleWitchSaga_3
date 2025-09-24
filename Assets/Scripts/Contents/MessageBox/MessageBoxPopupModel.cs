using System;

public class MessageBoxPopupModel : IBaseViewModel
{
    #region Property
    public string MessageText { get; private set; }
    public MessageBoxType MessageType { get; private set; }
    public Action OnConfirm { get; private set; }
    public Action OnCancel { get; private set; }
    public Action OnClickConfirm { get; private set; }
    public Action OnClickCancel { get; private set; }
    #endregion

    #region Function
    public void SetMessageText(string messageText)
    {
        MessageText = messageText;
    }

    public void SetMessageType(MessageBoxType messageType)
    {
        MessageType = messageType;
    }

    public void SetOnConfirm(Action onConfirm)
    {
        OnConfirm = onConfirm;
    }

    public void SetOnCancel(Action onCancel)
    {
        OnCancel = onCancel;
    }

    public void SetOnClickConfirm(Action onClickConfirm)
    {
        OnClickConfirm = onClickConfirm;
    }

    public void SetOnClickCancel(Action onClickCancel)
    {
        OnClickCancel = onClickCancel;
    }
    #endregion
}
