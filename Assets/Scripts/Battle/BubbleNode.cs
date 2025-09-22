using Cysharp.Threading.Tasks;
using UnityEngine;

public class BubbleNode : PoolableBaseUnit<BubbleNodeModel>
{
    [SerializeField]
    private float moveTime = 0;

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public async UniTask SmoothMove(Vector3 pos)
    {
        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / moveTime;

            transform.position = Vector3.Lerp(transform.position, pos, progress);
            await UniTaskUtils.NextFrame();
        }

        transform.position = pos;
    }
}
