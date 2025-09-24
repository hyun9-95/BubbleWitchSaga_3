using UnityEngine;

public class SimpleBarModel : IBaseUnitModel
{
    public float MaxGauge { get; private set; }

    public float Gauge {  get; private set; }

    public void SetMaxGauge(float value)
    {
        MaxGauge = value;
    }

    public void SetGauge(float value)
    {
        Gauge = value;
    }
}
