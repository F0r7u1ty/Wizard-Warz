using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider xSlider;
    public Slider ySlider;

    void Start()
    {
        if (SettingsManager.Instance != null)
        {
            xSlider.value = SettingsManager.Instance.xSensitivity;
            ySlider.value = SettingsManager.Instance.ySensitivity;
        }

        xSlider.onValueChanged.AddListener(OnXChanged);
        ySlider.onValueChanged.AddListener(OnYChanged);

        // If other systems change settings, update UI too:
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.OnSensitivityChanged += OnSensChanged;
    }

    void OnDestroy()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.OnSensitivityChanged -= OnSensChanged;
    }

    void OnXChanged(float v) => SettingsManager.Instance?.SetXSensitivity(v);
    void OnYChanged(float v) => SettingsManager.Instance?.SetYSensitivity(v);

    void OnSensChanged(float newX, float newY)
    {
        // keep sliders in sync if sensitivity changed elsewhere
        if (xSlider != null) xSlider.SetValueWithoutNotify(newX);
        if (ySlider != null) ySlider.SetValueWithoutNotify(newY);
    }
}
