using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{

    public Slider slider;

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SetAllSliderValues(float min, float max, float startValue)
    {
        slider.maxValue = max;
        slider.minValue = min;

        slider.value = startValue;
    }
}
