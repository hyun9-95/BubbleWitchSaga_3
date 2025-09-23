using UnityEngine;

public class LobbyView : BaseView
{
    public LobbyViewModel Model => GetModel<LobbyViewModel>();

    public void OnStartBattle()
    {
        Model.OnStartBattle?.Invoke();
    }
}
