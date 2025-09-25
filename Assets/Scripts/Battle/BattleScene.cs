#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
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
    private float scrollDuration = 0.3f;

    [SerializeField]
    private BattleGrid grid;

    private CancellationToken ct;

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

    private void OnDisable()
    {
        TokenPool.Cancel(GetHashCode());
    }

    public async UniTask ScrollAsync(float yPos)
    {
        var startCameraPos = battleCamera.transform.position;
        var startBgPos = background.transform.position;

        var targetCameraPos = startCameraPos;
        var targetBgPos = startBgPos;

        targetCameraPos.y += yPos;
        targetBgPos.y += yPos;

        float elapsedTime = 0f;

        var ct = TokenPool.Get(GetHashCode());
        while (!ct.IsCancellationRequested && elapsedTime < scrollDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollDuration;

            battleCamera.transform.position = Vector3.Lerp(startCameraPos, targetCameraPos, t);
            background.transform.position = Vector3.Lerp(startBgPos, targetBgPos, t);

            await UniTask.NextFrame();
        }

        battleCamera.transform.position = targetCameraPos;
        background.transform.position = targetBgPos;
    }
}
