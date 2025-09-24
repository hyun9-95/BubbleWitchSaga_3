using UnityEngine;

public class LobbyView : BaseView
{
    public LobbyViewModel Model => GetModel<LobbyViewModel>();

    [SerializeField]
    private ClickBlocker blocker;

    [SerializeField]
    private GameObject touchToStart;

    private void OnEnable()
    {
        AllowClick(false);
    }

    public void OnStartBattle()
    {
        AllowClick(false);
        Model.OnStartBattle?.Invoke();
    }

    public void AllowClick(bool value)
    {
        blocker.SafeSetActive(!value);
        touchToStart.SafeSetActive(value);
    }
}
