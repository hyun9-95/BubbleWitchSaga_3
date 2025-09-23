#pragma warning disable CS0162
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : BaseMonoManager<ObjectPoolManager>
{
    [SerializeField]
    private Transform poolRoot;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, Transform> poolParentDictionary = new Dictionary<string, Transform>();
    private Dictionary<int, Vector3> originScaleDic = new Dictionary<int, Vector3>();

    private Transform GetPoolParent(string key)
    {
        if (!poolParentDictionary.TryGetValue(key, out Transform parent))
        {
            parent = new GameObject(key).transform;
            parent.SetParent(poolRoot);
            poolParentDictionary[key] = parent;
        }

        return parent;
    }

    private async UniTask<GameObject> Spawn(string address, Vector3 position = default, Quaternion rotation = default)
    {
        if (string.IsNullOrEmpty(address))
        {
            Logger.Error("ObjectPoolManager Spawn Failed.. Address is Empty!!");
            return null;
        }

        if (!poolDictionary.ContainsKey(address))
            poolDictionary[address] = new Queue<GameObject>();

        GameObject go;

        if (poolDictionary[address].Count > 0)
        {
            go = poolDictionary[address].Dequeue();
            go.transform.SetPositionAndRotation(position, rotation);

            if (originScaleDic.TryGetValue(go.GetInstanceID(), out Vector3 originScale))
                go.transform.localScale = originScale;

            go.SetActive(true);
        }
        else
        {
            go = await AddressableManager.Instance.InstantiateAsync(address, GetPoolParent(address));

            if (go == null)
                return null;

            go.transform.SetPositionAndRotation(position, rotation);
            originScaleDic[go.GetInstanceID()] = go.transform.localScale;
        }

        return go;
    }

    public async UniTask<T> SpawnPoolableMono<T>(string address, Vector3 position = default, Quaternion rotation = default)
        where T : PoolableMono
    {
        var go = await Spawn(address, position, rotation);

        if (go == null)
            return null;

        if (go.TryGetComponent<T>(out var poolableMono))
        {
            if (!poolableMono.IsInitialized)
                poolableMono.SetAddress(address);

            return poolableMono;
        }

        return null;
    }

    public async UniTask Prewarm(string address, int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var go = await Spawn(address);
            if (go != null)
            {
                go.SetActive(false);
                ReturnToPool(go, address);
            }
        }
    }

    public void Clear()
    {
        poolDictionary.Clear();

        foreach (var parent in poolParentDictionary.Values)
        {
            if (parent != null)
                GameObject.Destroy(parent.gameObject);
        }

        poolParentDictionary.Clear();
        originScaleDic.Clear();
    }

    public void ReturnToPool(GameObject go, string address)
    {
        if (go.CheckSafeNull())
            return;

        if (string.IsNullOrEmpty(address))
            return;

        if (!poolDictionary.ContainsKey(address))
            return;

        poolDictionary[address].Enqueue(go);
    }
}