#pragma warning disable CS1998

using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseUnit<T> : BaseUnit where T : IBaseUnitModel
{
    public T Model { get; private set; }

    public void SetModel(T model)
    {
        Model = model;
    }
}

public abstract class BaseUnit : AddressableMono
{
    public virtual async UniTask ShowAsync()
    {
        gameObject.SafeSetActive(true);
    }
}

public class PoolableBaseUnit<T> : PoolableMono where T : IBaseUnitModel
{
    public T Model { get; private set; }

    public void SetModel(T model)
    {
        Model = model;
    }

    public virtual async UniTask ShowAsync()
    {
        gameObject.SafeSetActive(true);
    }
}

