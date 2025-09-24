using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFactory : BaseManager<BubbleFactory>
{
    private int colorCount = 0;

    private bool isInitiailizeProb = false;

    private float[] bubbleTypeProbs;

    public async UniTask PrewarmBubbles(int count)
    {
        var tasks = new UniTask<BubbleNode>[count];

        for (int i = 0; i < count; i++)
            tasks[i] = CreateNewBubble(true, true);

        BubbleNode[] bubbles = await UniTask.WhenAll(tasks);

        for (int i = 0; i < bubbles.Length; i++)
            bubbles[i].gameObject.SafeSetActive(false);
    }

    public async UniTask<BubbleNode> CreateNewBubble(bool randomType = false, bool randomColor = false)
    {
        var model = new BubbleNodeModel();

        var newBubble = await ObjectPoolManager.Instance.
            SpawnPoolableMono<BubbleNode>(PathDefine.BUBBLE_NODE);

        if (randomType)
        {
            if (!isInitiailizeProb)
                InitializeProbs();

            var type = GetRandomType();
            model.SetBubbleType(type);
        }

        if (model.IsColorType && randomColor)
        {
            var color = GetRandomColor();
            model.SetBubbleColor(color);
        }

        newBubble.SetModel(model);

        await newBubble.ShowAsync();
        Naming(newBubble);

        return newBubble;
    }

    public async UniTask<BubbleNode> CreateNewBubble(BubbleType type)
    {
        var model = new BubbleNodeModel();

        var newBubble = await ObjectPoolManager.Instance.
            SpawnPoolableMono<BubbleNode>(PathDefine.BUBBLE_NODE);

        model.SetBubbleType(type);

        if (model.IsColorType)
        {
            var color = GetRandomColor();
            model.SetBubbleColor(color);
        }

        newBubble.SetModel(model);

        await newBubble.ShowAsync();
        Naming(newBubble);

        return newBubble;
    }

    private void InitializeProbs()
    {
        // Empty 부터는 고정 버블이라 스폰 X
        int typeCount = (int)BubbleType.Empty;

        bubbleTypeProbs = new float[typeCount];

        var bubbleProbData = DataManager.Instance.GetDataById<DataBalance>((int)BalanceDefine.BUBBLE_SPAWN_PROB);

        if (bubbleProbData == null || bubbleTypeProbs.Length != bubbleProbData.ValueCount)
        {
            Logger.Error($"BUBBLE_SPAWN_PROB");
            return;
        }

        for (int i = 0; i < bubbleTypeProbs.Length; i++)
            bubbleTypeProbs[i] = bubbleProbData.Values[i] / 1000;

        isInitiailizeProb = true;
    }

    private BubbleType GetRandomType()
    {
        float roll = Random.value;
        float currentRate = 0;

        for (BubbleType type = BubbleType.Normal; type < BubbleType.Empty; type++)
        {
            currentRate += bubbleTypeProbs[(int)type];

            if (roll <= currentRate)
                return type;
        }

        return BubbleType.Normal;
    }

    private BubbleColor GetRandomColor()
    {
        if (colorCount == 0)
            colorCount = System.Enum.GetValues(typeof(BubbleColor)).Length;

        int random = Random.Range(0, colorCount);

        return (BubbleColor)random;
    }

    private void Naming(BubbleNode node)
    {
#if UNITY_EDITOR
        if (node == null || node.Model == null)
            return;

        node.gameObject.name = node.Model.BubbleType.ToString();
#endif
    }
}
