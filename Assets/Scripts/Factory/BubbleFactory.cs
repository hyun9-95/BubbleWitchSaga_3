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

        if (model.BubbleType == BubbleType.Normal && randomColor)
        {
            var color = GetRandomColor();
            model.SetBubbleColor(color);
        }

        newBubble.SetModel(model);

        await newBubble.ShowAsync();
        
        return newBubble;
    }

    public async UniTask<BubbleNode> CreateNewBubble(BubbleType type)
    {
        var model = new BubbleNodeModel();

        var newBubble = await ObjectPoolManager.Instance.
            SpawnPoolableMono<BubbleNode>(PathDefine.BUBBLE_NODE);

        model.SetBubbleType(type);

        if (type == BubbleType.Normal)
        {
            var color = GetRandomColor();
            model.SetBubbleColor(color);
        }

        newBubble.SetModel(model);

        await newBubble.ShowAsync();

        return newBubble;
    }

    private void InitializeProbs()
    {
        int typeCount = System.Enum.GetValues(typeof(BubbleType)).Length;
        typeCount -= 2; // Guide, Spawn 타입 제외함

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

        for (BubbleType type = BubbleType.Normal; type < BubbleType.Guide; type++)
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
}
