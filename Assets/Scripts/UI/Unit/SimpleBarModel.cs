using UnityEngine;

public class SimpleBarModel : IBaseUnitModel
{
    public int MaxValue { get; private set; }

    public int Value {  get; private set; }

    public void SetMaxValue(int value)
    {
        MaxValue = value;
    }

    public void SetValue(int value)
    {
        Value = value;
    }

    public void ReduceValue(int value)
    {
        Value -= value;
    }

    public float GetGauge()
    {
        return  Mathf.Clamp01((float)Value / MaxValue);
    }
}
