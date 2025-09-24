using System;

public interface IObserver
{
    public void HandleMessage(Enum observerMessage, IObserverParam observerParam);
}