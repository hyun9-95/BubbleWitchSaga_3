using UnityEngine;

public class TransformWrapper
{
    public TransformWrapper(Transform transform)
    {
        this.transform = transform;
    }

    private Transform transform;

    public Vector3 position => transform.position;

    public Quaternion Rotation => transform.rotation;

    public int InstanceId => transform.gameObject.GetInstanceID();
}
