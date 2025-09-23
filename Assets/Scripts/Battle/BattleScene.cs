#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    public BattleGrid Grid => grid;

    [SerializeField]
    private CinemachineCamera battleCamera;

    [SerializeField]
    private GameObject background;

    [SerializeField]
    private float cameraYOffset = 0f;

    [SerializeField]
    private BattleGrid grid;

    [SerializeField]
    private Transform objectContainer;

    public async UniTask Prepare()
    {
        PrepareGrid();
    }

    private void PrepareGrid()
    {
        var centerWorld = grid.CenterWorld;
        background.transform.position = centerWorld;
        battleCamera.transform.position = new Vector3(centerWorld.x, centerWorld.y + cameraYOffset, -1);
    }

}
