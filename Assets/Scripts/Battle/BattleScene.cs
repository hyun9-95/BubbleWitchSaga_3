#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera battleCamera;

    [SerializeField]
    private GameObject background;

    [SerializeField]
    private float cameraYOffset = 0f;

    [SerializeField]
    private BattleGrid grid;

    private BattlePhase currentPhase;
    private Dictionary<BattlePhase, IBattlePhaseProcessor> processorDic = new();

    public async UniTask Prepare()
    {
        PrepareGrid();
    }

    private void PrepareGrid()
    {
        grid.BuildGrid();

        var centerWorld = grid.CenterWorld;
        background.transform.position = centerWorld;
        battleCamera.transform.position = new Vector3(centerWorld.x, centerWorld.y + cameraYOffset, -1);
    }

    public void AddPhaseProcessor(BattlePhase phase, IBattlePhaseProcessor processor)
    {
        processorDic.Add(phase, processor);
    }

    public async UniTask StartBattle()
    {
        currentPhase = BattlePhase.Stage;
        OnBattle().Forget();
    }

    public async UniTask OnBattle()
    {
        while (currentPhase != BattlePhase.End)
        {
            var battlePhaseProcessor = processorDic[currentPhase];
            await battlePhaseProcessor.OnStartPhase(grid);

            while (currentPhase == battlePhaseProcessor.Phase)
            {
                battlePhaseProcessor.OnProcessPhase(grid);
                await UniTaskUtils.NextFrame();
            }

            await battlePhaseProcessor.OnEndPhase(grid);
        }
    }
}
