using UnityEngine;
using System;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public float xSensitivity = 1f;
    public float ySensitivity = 1f;

    private const string X_KEY = "sensitivity_x";
    private const string Y_KEY = "sensitivity_y";

    // Event fired when sensitivities change: (newX, newY)
    public event Action<float, float> OnSensitivityChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    void Load()
    {
        if (PlayerPrefs.HasKey(X_KEY)) xSensitivity = PlayerPrefs.GetFloat(X_KEY);
        if (PlayerPrefs.HasKey(Y_KEY)) ySensitivity = PlayerPrefs.GetFloat(Y_KEY);
    }

    public void SetXSensitivity(float v)
    {
        xSensitivity = v;
        PlayerPrefs.SetFloat(X_KEY, v);
        PlayerPrefs.Save();
        OnSensitivityChanged?.Invoke(xSensitivity, ySensitivity);
    }

    public void SetYSensitivity(float v)
    {
        ySensitivity = v;
        PlayerPrefs.SetFloat(Y_KEY, v);
        PlayerPrefs.Save();
        OnSensitivityChanged?.Invoke(xSensitivity, ySensitivity);
    }

    // optional convenience to set both at once
    public void SetSensitivities(float x, float y)
    {
        xSensitivity = x;
        ySensitivity = y;
        PlayerPrefs.SetFloat(X_KEY, x);
        PlayerPrefs.SetFloat(Y_KEY, y);
        PlayerPrefs.Save();
        OnSensitivityChanged?.Invoke(xSensitivity, ySensitivity);
    }
}
