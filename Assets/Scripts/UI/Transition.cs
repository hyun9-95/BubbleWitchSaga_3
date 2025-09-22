using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Transition : MonoBehaviour
{
    public bool IsPlaying => isPlaying;

    protected bool isPlaying;

    public abstract UniTask In();

    public abstract UniTask Out();
}
