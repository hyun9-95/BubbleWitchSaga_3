using System;

public class LobbyViewModel : IBaseViewModel
{
    public Action OnStartBattle { get; private set; }

    public void SetOnStartBattle(Action onStartBattle)
    {
        OnStartBattle = onStartBattle;
    }
}
