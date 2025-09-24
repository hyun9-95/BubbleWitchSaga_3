#pragma warning disable CS1998
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SimpleBar : BaseUnit<SimpleBarModel>
{
    [SerializeField]
    private Image bar;

    public override async UniTask ShowAsync()
    {
        UpdateBar();
    }

    public void RefreshBar()
    {
        UpdateBar();
    }
    
    private void UpdateBar()
    {
        var amount = Mathf.Clamp01(Model.MaxGauge / Model.Gauge);
        bar.fillAmount = amount;
    }
}
