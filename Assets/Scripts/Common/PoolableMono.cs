public class PoolableMono : AddressableMono
{
    public bool IsInitialized => !string.IsNullOrEmpty(address);

    private string address;

    protected virtual void OnDisable()
    {
        TokenPool.Cancel(GetHashCode());
        ReturnPool();
    }

    public void SetAddress(string address)
    {
        this.address = address;
    }

    private void ReturnPool()
    {
        if (ObjectPoolManager.Instance.CheckSafeNull())
            return;

        ObjectPoolManager.Instance.ReturnToPool(gameObject, address);
    }
}
