#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxPopup : BaseView
{
    public MessageBoxPopupModel Model => GetModel<MessageBoxPopupModel>();

    [SerializeField]
    private TextMeshProUGUI messageText;

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private Button cancelButton;

    public override async UniTask ShowAsync()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (messageText != null && !string.IsNullOrEmpty(Model.MessageText))
            messageText.text = Model.MessageText;

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        if (Model.MessageType == MessageBoxType.OneButton)
        {
            if (confirmButton != null)
                confirmButton.gameObject.SafeSetActive(true);

            if (cancelButton != null)
                cancelButton.gameObject.SafeSetActive(false);
        }
        else if (Model.MessageType == MessageBoxType.TwoButton)
        {
            if (confirmButton != null)
                confirmButton.gameObject.SafeSetActive(true);

            if (cancelButton != null)
                cancelButton.gameObject.SafeSetActive(true);
        }
    }

    public void OnClickConfirm()
    {
        Model.OnClickConfirm();
    }

    public void OnClickCancel()
    {
        Model.OnClickCancel();
    }
}
